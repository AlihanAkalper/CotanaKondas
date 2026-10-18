using System;
using System.Collections.Generic;
using System.Linq;

// Bölgesel tedarik, yükseltme ve bakım dengesi.
public static class ConstructionRules
{
    public const int MaxFloors = 3;
    public const int MaxMaterialLevel = 5;
    public const float FloorHealth = 100f;
    public const float DurabilityPerLevel = .15f, IncomePerLevel = .15f;
    public const float UpgradePriceGrowth = 1.45f, RepairCostFraction = .50f;
    public static readonly Dictionary<Materyal, int> BasePrices = new()
    { { Materyal.Odun, 10000 }, { Materyal.Tuğla, 16000 }, { Materyal.Çelik, 24000 } };
    // Sıra: odun, tuğla, çelik. Her bölge/malzeme bağımsız ayarlanabilir.
    public static readonly Dictionary<Ev_numaraları, float[]> RegionPrices = new()
    {
        { Ev_numaraları.Marmara, new[] { 1.05f, .90f, .80f } }, // Sanayi ve liman tedariki.
        { Ev_numaraları.Ege, new[] { .95f, .80f, .95f } }, // Kıyı lojistiği ve yerel yapı malzemesi.
        { Ev_numaraları.İçanadolu, new[] { 1.25f, .75f, 1.05f } }, // İç bölge; tuğla avantajı.
        { Ev_numaraları.Karadeniz, new[] { .65f, 1.10f, 1.15f } }, // Orman avantajı, dağlık taşıma.
        { Ev_numaraları.Doğuanadolu, new[] { 1.25f, 1.15f, 1.35f } }, // Uzaklık ve dağlık taşıma.
        { Ev_numaraları.Güneydoğu, new[] { 1.40f, .85f, 1.15f } }, // Odun taşıma maliyeti.
        { Ev_numaraları.Akdeniz, new[] { .90f, .95f, .90f } }, // Limanlar ve kıyı tedariki.
        { Ev_numaraları.Kıbrıs, new[] { 1.45f, 1.30f, 1.40f } } // Ada taşımacılığı.
    };
    public static int MaterialPrice(Ev_numaraları region, Materyal material)
    {
        int index = material == Materyal.Odun ? 0 : material == Materyal.Tuğla ? 1 : 2;
        return (int)Math.Ceiling(BasePrices[material] * RegionPrices[region][index]);
    }
    public static float Durability(HouseSaveData h) => 1f + Math.Max(0, h.MaterialLevel - 1) * DurabilityPerLevel;
    public static float IncomeBonus(HouseSaveData h) => 1f + Math.Max(0, h.MaterialLevel - 1) * IncomePerLevel;
    public static int UpgradePrice(HouseSaveData h) => (int)Math.Ceiling(MaterialPrice(h.Type, h.material) * Math.Pow(UpgradePriceGrowth, h.MaterialLevel));
    public static int FloorPrice(HouseSaveData h, int floor = -1) =>
        MaterialPrice(h.Type, h.material) * ((floor < 0 ? h.Floors.Count : floor) + 1);
    public static int RepairPrice(HouseSaveData h, int floor)
    {
        // Tamamen yıkılan kat, aynı kattaki yeni inşaatın tam bedeliyle yeniden yapılır.
        if (floor >= 0 && h.Floors[floor].Health <= 0) return FloorPrice(h, floor);
        float missing = FloorHealth - h.Floors[floor].Health;
        return (int)Math.Ceiling(FloorPrice(h, floor) * RepairCostFraction * missing / FloorHealth);
    }
    public static int DailyIncome(HouseSaveData h) => !h.IsBuilt ? 0 :
        (int)Math.Floor(RegionDatabase.Regions[h.Type].MinRentIncome * h.Floors.Count(f => f.Health > 0 && f.HasTenant) * IncomeBonus(h));
    public static bool Purchase(GameSaveData save, HouseSaveData h, string action, Materyal material = Materyal.Alınmadı, int floor = -1)
    {
        if (save.IsFinished || h == null || !h.IsBuilt || !save.Houses.Contains(h)) return false;
        int cost;
        switch (action)
        {
            case "select":
                if (h.material != Materyal.Alınmadı || !BasePrices.ContainsKey(material)) return false;
                cost = MaterialPrice(h.Type, material); break;
            case "upgrade":
                if (h.material == Materyal.Alınmadı || h.MaterialLevel >= MaxMaterialLevel) return false;
                cost = UpgradePrice(h); break;
            case "floor":
                if (h.material == Materyal.Alınmadı || h.Floors.Count >= Math.Min(h.MaterialLevel, MaxFloors)) return false;
                if (h.Floors.Any(f => f.Health <= 0)) return false;
                cost = FloorPrice(h); break;
            case "repair":
                if (h.material == Materyal.Alınmadı || floor < 0 || floor >= h.Floors.Count) return false;
                if (floor >= 0 && h.Floors.Take(floor).Any(f => f.Health <= 0)) return false;
                cost = RepairPrice(h, floor);
                if (cost <= 0) return false;
                break;
            default: return false;
        }
        if (save.Money < cost) return false;
        save.Money -= cost;
        if (action == "select") { h.material = material; h.MaterialLevel = 1; }
        if (action == "upgrade") h.MaterialLevel++;
        if (action == "floor")
        {
            var floorData = new FloorSaveData();
            h.Floors.Add(floorData);
        }
        if (action == "repair") h.Floors[floor].Health = FloorHealth;
        return true;
    }
    public static float CalculateDamage(HouseSaveData h, AfetTürü disaster, IEnumerable<PerkType> perks)
    {
        if (disaster == AfetTürü.Yok) return 0;
        float baseDamage = disaster == AfetTürü.Deprem ? 50f : 35f;
        float materialMultiplier = DisasterDatabase.MaterialMultipliers.TryGetValue((disaster, h.material), out var m) ? m : 1f;
        return baseDamage * materialMultiplier * PerkDatabase.GetDamageMultiplier(disaster, perks) / Durability(h);
    }

    public static float ApplyDamage(HouseSaveData h, AfetTürü disaster, IEnumerable<PerkType> perks)
    {
        float damage = CalculateDamage(h, disaster, perks);
        float before = h.Floors.Sum(f => f.Health);
        bool lowerFloorDestroyed = false;
        for (int i = 0; i < h.Floors.Count; i++)
        {
            // Oyun dengesi: üst katlar deprem/yangında, alt katlar selde daha çok etkilenir.
            // Alt katın hasarını da azaltarak eşzamanlı toplu yıkımı seyrekleştir.
            float heightMultiplier = disaster switch
            {
                AfetTürü.Deprem => .80f + .20f * i,
                AfetTürü.Yangın => .90f + .10f * i,
                AfetTürü.Sel => Math.Max(.10f, 1f - .15f * i),
                _ => 1f
            };
            h.Floors[i].Health = lowerFloorDestroyed ? 0 :
                Math.Clamp(h.Floors[i].Health - damage * heightMultiplier, 0, FloorHealth);
            if (h.Floors[i].Health <= 0)
            {
                h.Floors[i].WasDestroyed = true;
                lowerFloorDestroyed = true;
            }
        }
        if (before > 0 && h.Floors.All(f => f.Health <= 0)) h.WasDestroyed = true;
        return before - h.Floors.Sum(f => f.Health);
    }
    public static void Migrate(GameSaveData save)
    {
        // Eski ortak satın alımların bölgesi bilinmiyor; bedellerini bir kez iade et.
        if (save.LegacyPurchasedPerks != null)
        {
            foreach (var perk in save.LegacyPurchasedPerks.Distinct())
                if (PerkDatabase.Perks.TryGetValue(perk, out var info)) save.Money += info.Price;
            save.LegacyPurchasedPerks = null;
        }
        save.PendingDisasters ??= new();
        save.DialogueAnswers ??= new();
        foreach (var h in save.Houses)
        {
            h.PurchasedPerks ??= new();
            if (RegionDatabase.Regions.TryGetValue(h.Type, out var region))
                h.Price = region.BasePlotPrice;
            if (h.ConstructionVersion < 1)
            {
                if (h.material != Materyal.Alınmadı)
                {
                    h.MaterialLevel = 1;
                    h.Floors = new() { new FloorSaveData { Health = Math.Clamp(h.LegacyHealth ?? FloorHealth, 0, FloorHealth) } };
                }
                h.ConstructionVersion = 1;
            }
            h.Floors ??= new();
            h.LegacyHealth = null;
            if (h.IsBuilt && h.Floors.Count > 0 && h.Floors.All(f => f.Health <= 0)) h.WasDestroyed = true;
            foreach (var floor in h.Floors)
            {
                floor.MigrateTenant();
                if (floor.Health <= 0) floor.WasDestroyed = true;
                TenantRules.ChangeAwareness(floor, 0);
            }
        }
        GameEndRules.Evaluate(save);
    }
}

