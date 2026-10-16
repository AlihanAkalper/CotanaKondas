using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

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
        if (SaveData.Money < house.Price)
        {
            GD.Print("Yetersiz para!");
            return false;
        }

        // 4. Evin bilgilerini doldur
        house.IsBuilt = true;
        house.material = material;

        GD.Print($"{type} inşa edildi! Kullanılan: {material}");

        SaveGame();
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
}
