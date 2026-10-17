using Godot;
using System;
using System.Collections.Generic;



public class DisasterEvent
{
    public Ev_numaraları Region { get; set; }
    public AfetTürü DisasterType { get; set; }
    public float DamageTaken { get; set; }
    public float RemainingHealth { get; set; }
}

public partial class DisasterManager : Node
{

    public static DisasterManager Instance { get; private set; }

    [Signal] public delegate void DisasterOccurredEventHandler(int regionInt, int disasterTypeInt);

    private Random _random = new();

    public override void _Ready()
    {
        Instance = this;
    }

    public List<DisasterEvent> ProcessDailyDisasters()
    {
        var data = GameManager.Instance.SaveData;
        int currentDay = data.CurrentDay;
        List<DisasterEvent> occurredEvents = new();

        // 1. KURAL: İlk 5 gün boyunca asla afet olmaz
        if (currentDay <= 5)
        {
            GD.Print($"Gün {currentDay}: Güvenli dönem (İlk 30 gün afet yok).");
            return occurredEvents;
        }

        // 2. KURAL: Kümülatif Global Zar
        float globalRoll = (float)(_random.NextDouble() * 100.0);
        GD.Print($"Gün {currentDay} Afet İhtimali: %{data.CumulativeDisasterChance:F1} | Atılan Zar: {globalRoll:F1}");

        if (globalRoll <= data.CumulativeDisasterChance)
        {
            GD.Print("Global afet koşulları sağlandı! Şehirler kontrol ediliyor...");

            // 3. KURAL: Her inşa edilmiş bölgenin kendi bağımsız olasılığı
            foreach (var house in data.Houses)
            {
                if (!house.IsBuilt || house.Health <= 0) continue;

                if (!RegionDisasterDatabase.RegionConfigs.ContainsKey(house.Type)) continue;

                var config = RegionDisasterDatabase.RegionConfigs[house.Type];

                foreach (var (afet, sans) in config.DisasterChances)
                {
                    float regionRoll = (float)(_random.NextDouble() * 100.0);
                    if (regionRoll <= sans)
                    {
                        // Bu şehirde afet patlak verdi
                        var evnt = ApplyDamage(house, afet);
                        occurredEvents.Add(evnt);
                        break; // Aynı şehre bir günde 1 afet
                    }
                }
            }

            // Eğer afet vurduysa global şansı tekrar taban değer olan %15'e çek
            if (occurredEvents.Count > 0)
            {
                data.CumulativeDisasterChance = 15f;
            }
            else
            {
                // Global tuttu ama şans eseri hiçbir şehir vurulmadıysa +1 artmaya devam etsin
                data.CumulativeDisasterChance += 1f;
            }
        }
        else
        {
            // O gün afet olmadı -> Birikimli oran sonraki gün için %1 artar
            data.CumulativeDisasterChance += 1f;
            GD.Print($"Bugün sakin geçti. Yarınki afet şansı: %{data.CumulativeDisasterChance:F1}");
        }

        GameManager.Instance.SaveGame();
        return occurredEvents;
    }

    private DisasterEvent ApplyDamage(HouseSaveData house, AfetTürü afet)
    {
        float baseDamage = 35f;
        float multiplier = 1.0f;

        if (DisasterDatabase.MaterialMultipliers.ContainsKey((afet, house.material)))
        {
            multiplier = DisasterDatabase.MaterialMultipliers[(afet, house.material)];
        }

        float damage = baseDamage * multiplier;
        house.Health = Mathf.Clamp(house.Health - damage, 0f, 100f);

        return new DisasterEvent
        {
            Region = house.Type,
            DisasterType = afet,
            DamageTaken = damage,
            RemainingHealth = house.Health
        };
    }

    public override void _Process(double delta)
	{
	}

    public async void OnNextDayButtonPressed()
    {
        GameManager.Instance.SaveData.CurrentDay++;

        // Simülasyonu çalıştır
        List<DisasterEvent> disasters = DisasterManager.Instance.ProcessDailyDisasters();

        HUD.Instance.UpdateHUD();

        if (disasters.Count > 0)
        {
            // Birden fazla afet varsa sırayla her birine götürür
            foreach (var ev in disasters)
            {
                GD.Print($"DİKKAT! {ev.Region} bölgesinde {ev.DisasterType} başladı!");

                // 1. O bölgeyi seç ve 2. Haritaya (İnşaat / Bölge Sahnesine) geç
                GameManager.Instance.SelectedRegion = ev.Region;
                GetTree().ChangeSceneToFile("res://Scenes/BuildingMap.tscn");

                // 2. Sahne yüklendikten sonra animasyonu oynatması için bekleme/tetikleme
                // (2. haritadaki script ev.DisasterType türüne göre deprem sarsıntısı, 
                // alev partikülü veya sel suyu animasyonunu oynatır)
                break;
            }
        }
        else
        {
            GD.Print("Huzurlu bir gün başladı.");
        }
    }
}
