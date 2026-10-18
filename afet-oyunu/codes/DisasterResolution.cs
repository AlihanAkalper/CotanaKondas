using System.Collections.Generic;
using System.Linq;

public class FloorDisasterResult
{
    public int FloorNumber { get; set; }
    public float BeforeHealth { get; set; }
    public float RemainingHealth { get; set; }
    public float DamageTaken { get; set; }
}

public class DisasterEvent
{
    public int Day { get; set; }
    public Ev_numaraları Region { get; set; }
    public AfetTürü DisasterType { get; set; }
    public float ProtectionPercent { get; set; }
    public float DurabilityMultiplier { get; set; }
    public int IncomeBefore { get; set; }
    public int IncomeAfter { get; set; }
    public int DepartedTenants { get; set; }
    public List<FloorDisasterResult> Floors { get; set; } = new();
    public List<string> AppliedPerks { get; set; } = new();
}

public static class DisasterResolution
{
    // Hasarı bir kez uygular; raporlar sonradan değişen canlı veriye bağlı kalmaz.
    public static DisasterEvent Resolve(GameSaveData save, HouseSaveData house, AfetTürü type)
    {
        var beforeFloors = house.Floors.Select(f => f.Health).ToArray();

        int incomeBefore = ConstructionRules.DailyIncome(house);
        ConstructionRules.ApplyDamage(house, type, save.PurchasedPerks);
        int departed = type == AfetTürü.Yok ? 0 : TenantRules.DepartAfterDisaster(house);
        var report = new DisasterEvent
        {
            Day = save.CurrentDay, Region = house.Type, DisasterType = type,
            DepartedTenants = departed,
            ProtectionPercent = (1f - PerkDatabase.GetDamageMultiplier(type, save.PurchasedPerks)) * 100f,
            DurabilityMultiplier = ConstructionRules.Durability(house),
            IncomeBefore = incomeBefore, IncomeAfter = ConstructionRules.DailyIncome(house)
        };
        for (int i = 0; i < beforeFloors.Length; i++)
            report.Floors.Add(new FloorDisasterResult
            {
                FloorNumber = i + 1, BeforeHealth = beforeFloors[i],
                RemainingHealth = house.Floors[i].Health,
                DamageTaken = beforeFloors[i] - house.Floors[i].Health
            });
        foreach (var perk in (save.PurchasedPerks ?? new()).Distinct())
            if (PerkDatabase.Perks.TryGetValue(perk, out var info) &&
                (info.AffectedDisaster == AfetTürü.Yok || info.AffectedDisaster == type))
                report.AppliedPerks.Add(info.Name);
        return report;
    }
}
