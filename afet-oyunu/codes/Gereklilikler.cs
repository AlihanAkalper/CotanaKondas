using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum Ev_numaraları
{
    Marmara = 0,
    Ege = 1,
    İçanadolu = 2,
    Karadeniz = 3,
    Doğuanadolu = 4,
    Güneydoğu = 5,
    Akdeniz = 6,
    Kıbrıs = 7
}



public enum Materyal
{
    Alınmadı = 0,
    Çelik = 1,
    Tuğla = 2,
    Odun = 3
}

[Serializable]
public class HouseSaveData
{
    public bool WasDestroyed { get; set; }
    public Ev_numaraları Type { get; set; }
    public bool IsBuilt { get; set; } = false;
    public Materyal material { get; set; } = Materyal.Alınmadı;
    public int Price { get; set; }

    // Yalnızca kat sistemi öncesindeki kayıtları dönüştürmek için okunur.
    [JsonPropertyName("Health")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? LegacyHealth { get; set; }
    public int ConstructionVersion { get; set; }
    public int MaterialLevel { get; set; }
    public List<FloorSaveData> Floors { get; set; } = new();

}

[Serializable]
public class GameSaveData
{
    public bool IsFinished { get; set; }
    public string EndReason { get; set; } = "";
    public int LastDialogueRollDay { get; set; }
    public List<TenantDialogueAnswer> DialogueAnswers { get; set; } = new();
    public List<DisasterEvent> PendingDisasters { get; set; } = new();
    public int CurrentDay { get; set; } = 1;
    public int Money { get; set; } = 1000000;
    public float CumulativeDisasterChance { get; set; } = 15f;

    public List<HouseSaveData> Houses { get; set; } = new();
    public List<PerkType> PurchasedPerks { get; set; } = new();

    public GameSaveData()
    {
        foreach (var region in Enum.GetValues<Ev_numaraları>())
        {
            Houses.Add(new HouseSaveData
            {
                Type = region,
                ConstructionVersion = 1,
                Price = RegionDatabase.Regions[region].BasePlotPrice,
            });
        }
    }
}

public class FloorSaveData
{
    public bool WasDestroyed { get; set; }
    public float Health { get; set; } = ConstructionRules.FloorHealth;
    public TenantSaveData Tenant { get; set; } = new();

    // Eski kayıtların kat üzerinde tuttuğu kiracı alanlarını yüklerken taşı.
    [JsonExtensionData] public Dictionary<string, JsonElement> LegacyTenantFields { get; set; }

    [JsonIgnore] public bool HasTenant => Tenant != null;
    [JsonIgnore] public int Awareness { get => Tenant?.Awareness ?? 0; set { if (Tenant != null) Tenant.Awareness = value; } }
    [JsonIgnore] public bool LeavesOnNextDisaster { get => Tenant?.LeavesOnNextDisaster ?? false; set { if (Tenant != null) Tenant.LeavesOnNextDisaster = value; } }
    [JsonIgnore] public int PendingQuestionId { get => Tenant?.PendingQuestionId ?? -1; set { if (Tenant != null) Tenant.PendingQuestionId = value; } }
    [JsonIgnore] public int[] QuestionAnswers { get => Tenant?.QuestionAnswers ?? Array.Empty<int>(); set { if (Tenant != null) Tenant.QuestionAnswers = value; } }

    public void MigrateTenant()
    {
        if (LegacyTenantFields == null) return;
        if (LegacyTenantFields.TryGetValue("HasTenant", out var occupied) && !occupied.GetBoolean())
            Tenant = null;
        else if (LegacyTenantFields.ContainsKey("Awareness"))
            Tenant = JsonSerializer.Deserialize<TenantSaveData>(JsonSerializer.Serialize(LegacyTenantFields));
        LegacyTenantFields = null;
    }
}

public class TenantSaveData
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public int Awareness { get; set; } = Random.Shared.Next(50, 101);
    public bool LeavesOnNextDisaster { get; set; }
    public int PendingQuestionId { get; set; } = -1;
    public int[] QuestionAnswers { get; set; } = Array.Empty<int>();
}

