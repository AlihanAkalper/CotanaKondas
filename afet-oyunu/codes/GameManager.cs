using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class GameManager : Node
{


    public void AdvanceToNextDay()
    {
        SaveData.CurrentDay++;
        int totalDailyIncome = 0;

        foreach (var house in SaveData.Houses)
        {
            if (house.IsBuilt && house.Health > 0)
            {
                if (RegionDatabase.Regions.ContainsKey(house.Type))
                {
                    int rent = RegionDatabase.Regions[house.Type].MinRentIncome;
                    totalDailyIncome += rent;
                }
            }
        }

        SaveData.Money += totalDailyIncome;
        GD.Print($"Gün {SaveData.CurrentDay} başladı! Toplanan Kira: {totalDailyIncome:N0} TL | Yeni Bakiye: {SaveData.Money:N0} TL");

        var occurredDisasters = DisasterManager.Instance.ProcessDailyDisasters();

        if (occurredDisasters.Count > 0)
        {
            // İlk afeti al ve o bölgeye git
            var disaster = occurredDisasters[0];

            SelectedRegion = disaster.Region;
            ActiveDisaster = disaster.DisasterType;

            GD.Print($"[DİKKAT] {disaster.Region} bölgesinde {disaster.DisasterType} çıktı! Oraya yönlendiriliyorsunuz...");

            SaveGame();

            // 2. Harita / Bölge sahnesine geçiş yap
            GetTree().ChangeSceneToFile("res://scenes/şehir.tscn");
        }
        else
        {
            // Afet yoksa sadece HUD'ı güncelle ve oyunu kaydet
            HUD.Instance?.UpdateHUD();
            SaveGame();
        }
    }
    public static GameManager Instance { get; private set; }

    public AfetTürü ActiveDisaster { get; set; } = AfetTürü.Yok;

    public GameSaveData SaveData { get; private set; } = new();
    private string _savePath;

    public override void _Ready()
    {
        Instance = this;
        _savePath = ProjectSettings.GlobalizePath("user://").PathJoin("savegame.json");
    }

    public bool BuyAndBuildHouse(Ev_numaraları type, Materyal material, int amount)
    {
        var house = SaveData.Houses.Find(h => h.Type == type);
        if (house == null || house.IsBuilt)
        {
            GD.Print("Bu ev zaten yapılmış veya bulunamadı!");
            return false;
        }

        // 1. Para kontrolü

        // Malzemenin birim fiyatı ve toplam inşa maliyeti
        int unitPrice = MaterialPrices.UnitPrices[material];
        int materialCost = unitPrice * amount;
        int totalCost = house.Price + materialCost;

        // Bakiye kontrolü
        if (SaveData.Money < totalCost)
        {
            GD.PrintErr($"Yetersiz bakiye! Gereken: {totalCost:N0} TL, Mevcut: {SaveData.Money:N0} TL");
            return false;
        }

        // Parayı doğrudan kasadan düş
        SaveData.Money -= totalCost;

        // Evi kaydet
        house.IsBuilt = true;
        house.material = material;
        house.MaterialAmount = amount;
        house.Health = 100f;

        GD.Print($"{type} başarıyla inşa edildi!");
        GD.Print($"Harcanan: {house.Price:N0} TL (Arsa) + {materialCost:N0} TL ({amount}x {material}) = Toplam {totalCost:N0} TL");

        SaveGame();
        HUD.Instance?.UpdateHUD();
        return true;
    }

    public void SaveGame()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(SaveData, options);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Kayıt hatası: {ex.Message}");
        }
    }

    public bool LoadGame()
    {
        if (!File.Exists(_savePath)) return false;

        try
        {
            string json = File.ReadAllText(_savePath);
            SaveData = JsonSerializer.Deserialize<GameSaveData>(json);
            return true;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Yükleme hatası: {ex.Message}");
            return false;
        }
    }
    public Ev_numaraları SelectedRegion { get; set; } = Ev_numaraları.Marmara;
}
