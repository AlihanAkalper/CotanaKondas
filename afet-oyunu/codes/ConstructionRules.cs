using System;
using System.Collections.Generic;
using System.Linq;

// Geçici denge değerleri: fiyat, gelir, dayanıklılık ve hasar burada düzenlenir.
public static class ConstructionRules
{
    public const int MaxFloors = 5;
    public const float BuildingHealth = 300f, FloorHealth = 100f;
    public const float DurabilityPerLevel = .20f, IncomePerLevel = .15f;
    public const float UpgradePriceGrowth = 1.5f, RepairCostFraction = .4f;
    public static readonly Dictionary<Materyal, int> BasePrices = new()
    { { Materyal.Odun, 5000 }, { Materyal.Tuğla, 8000 }, { Materyal.Çelik, 12000 } };
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
    public static int FloorPrice(HouseSaveData h) => MaterialPrice(h.Type, h.material) * (h.Floors.Count + 1);
    public static int RepairPrice(HouseSaveData h, int floor = -1)
    {
        float missing = floor < 0 ? BuildingHealth - h.Health : FloorHealth - h.Floors[floor].Health;
        return (int)Math.Ceiling(MaterialPrice(h.Type, h.material) * RepairCostFraction * missing / FloorHealth);
    }
    public static int DailyIncome(HouseSaveData h) => !h.IsBuilt || h.Health <= 0 ? 0 :
        (int)Math.Floor(RegionDatabase.Regions[h.Type].MinRentIncome * h.Floors.Count(f => f.Health > 0) * IncomeBonus(h));
    public static bool Purchase(GameSaveData save, HouseSaveData h, string action, Materyal material = Materyal.Alınmadı, int floor = -1)
    {
        if (h == null || !h.IsBuilt || !save.Houses.Contains(h)) return false;
        int cost;
        switch (action)
        {
            case "select":
                if (h.material != Materyal.Alınmadı || !BasePrices.ContainsKey(material)) return false;
                cost = MaterialPrice(h.Type, material); break;
            case "upgrade":
                if (h.material == Materyal.Alınmadı || h.MaterialLevel >= MaxFloors || h.Health <= 0) return false;
                cost = UpgradePrice(h); break;
            case "floor":
                if (h.material == Materyal.Alınmadı || h.Health <= 0 || h.Floors.Count >= Math.Min(h.MaterialLevel, MaxFloors)) return false;
                cost = FloorPrice(h); break;
            case "repair":
                if (h.material == Materyal.Alınmadı || floor < -1 || floor >= h.Floors.Count) return false;
                cost = RepairPrice(h, floor);
                if (cost <= 0) return false;
                break;
            default: return false;
        }
        if (save.Money < cost) return false;
        save.Money -= cost;
        if (action == "select") { h.material = material; h.MaterialLevel = 1; }
        if (action == "upgrade") h.MaterialLevel++;
        if (action == "floor") h.Floors.Add(new FloorSaveData());
        if (action == "repair") { if (floor < 0) h.Health = BuildingHealth; else h.Floors[floor].Health = FloorHealth; }
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
        float before = h.Health;
        h.Health = Math.Clamp(h.Health - damage, 0, BuildingHealth);
        for (int i = 0; i < h.Floors.Count; i++)
        {
            float heightMultiplier = disaster == AfetTürü.Sel ? 1f - .15f * i : 1f;
            h.Floors[i].Health = Math.Clamp(h.Floors[i].Health - damage * heightMultiplier, 0, FloorHealth);
        }
        return before - h.Health;
    }
    public static void Migrate(GameSaveData save)
    {
        save.PurchasedPerks ??= new();
        save.PendingDisasters ??= new();
        foreach (var h in save.Houses)
        {
            if (h.ConstructionVersion < 1)
            {
                h.Health = Math.Clamp(h.Health * 3f, 0, BuildingHealth);
                if (h.material != Materyal.Alınmadı)
                {
                    h.MaterialLevel = 1;
                    h.Floors = new() { new FloorSaveData { Health = h.Health / 3f } };
                }
                h.ConstructionVersion = 1;
            }
            h.Floors ??= new();
        }
    }
}

