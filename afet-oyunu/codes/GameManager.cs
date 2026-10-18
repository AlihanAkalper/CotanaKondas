using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class GameManager : Node
{


    public void AdvanceToNextDay()
    {
        if (HasPendingDisasters) { OpenCurrentGame(); return; }
        SaveData.CurrentDay++;
        int totalDailyIncome = 0;

        foreach (var house in SaveData.Houses)
        {
            totalDailyIncome += ConstructionRules.DailyIncome(house);
        }
        SaveData.Money += totalDailyIncome;
        GD.Print($"Gün {SaveData.CurrentDay} başladı! Toplanan Kira: {totalDailyIncome:N0} TL | Yeni Bakiye: {SaveData.Money:N0} TL");

        var occurredDisasters = DisasterManager.Instance.ProcessDailyDisasters();

        SaveData.PendingDisasters.AddRange(occurredDisasters);
        SaveGame();
        if (HasPendingDisasters) OpenCurrentGame();
        else HUD.Instance?.UpdateHUD();
    }
    public static GameManager Instance { get; private set; }
    public bool HasPendingDisasters => SaveData.PendingDisasters.Count > 0;
    public DisasterEvent CurrentDisaster => HasPendingDisasters ? SaveData.PendingDisasters[0] : null;

    public void OpenCurrentGame()
    {
        if (HasPendingDisasters)
        {
            SelectedRegion = CurrentDisaster.Region;
            GetTree().ChangeSceneToFile("res://scenes/şehir.tscn");
        }
        else GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }

    public void AcknowledgeDisaster()
    {
        if (!HasPendingDisasters) return;
        SaveData.PendingDisasters.RemoveAt(0);
        SaveGame();
        OpenCurrentGame();
    }
    public GameSaveData SaveData { get; set; } = new();
    private string _savePath;

    public override void _Ready()
    {
        Instance = this;
        _savePath = ProjectSettings.GlobalizePath("user://").PathJoin("savegame.json");
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
            var loaded = JsonSerializer.Deserialize<GameSaveData>(json);
            if (loaded == null) return false;
            ConstructionRules.Migrate(loaded);
            SaveData = loaded;
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


