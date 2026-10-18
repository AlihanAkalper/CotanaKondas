using Godot;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

public partial class GameManager : Node
{


    public void AdvanceToNextDay()
    {
        if (SaveData.PendingDaySummary != null) { OpenCurrentGame(); return; }
        GameEndRules.Evaluate(SaveData);
        if (SaveData.IsFinished) { SaveGame(); OpenCurrentGame(); return; }
        if (HasPendingDisasters) { OpenCurrentGame(); return; }
        var summary = new DaySummaryData { BalanceBefore = SaveData.Money };
        int tenantsBefore = SaveData.Houses.Sum(h => h.Floors.Count(f => f.HasTenant));
        int lossesBefore = SaveData.LostTenants;
        SaveData.CurrentDay++;
        TenantRules.AdvanceDay(SaveData, Random.Shared);
        summary.NewTenants = SaveData.Houses.Sum(h => h.Floors.Count(f => f.HasTenant)) - tenantsBefore;
        int totalDailyIncome = 0;

        foreach (var house in SaveData.Houses)
        {
            int income = ConstructionRules.DailyIncome(house);
            totalDailyIncome += income;
            if (house.IsBuilt)
                summary.RegionIncome.Add($"{RegionDatabase.Regions[house.Type].Name}: +{income:N0} TL");
        }
        SaveData.Money += totalDailyIncome;
        GD.Print($"Gün {SaveData.CurrentDay} başladı! Toplanan Kira: {totalDailyIncome:N0} TL | Yeni Bakiye: {SaveData.Money:N0} TL");

        var occurredDisasters = DisasterManager.Instance.ProcessDailyDisasters();

        SaveData.PendingDisasters.AddRange(occurredDisasters);
        summary.Day = SaveData.CurrentDay;
        summary.RentIncome = totalDailyIncome;
        summary.BalanceAfter = SaveData.Money;
        summary.LostTenants = SaveData.LostTenants - lossesBefore;
        summary.DisasterCount = occurredDisasters.Count;
        var usableFloors = SaveData.Houses.Where(h => h.IsBuilt).SelectMany(h => h.Floors)
            .Where(f => f.Health > 0).ToArray();
        summary.OccupiedFloors = usableFloors.Count(f => f.HasTenant);
        summary.EmptyFloors = usableFloors.Count(f => !f.HasTenant);
        summary.WaitingQuestions = SaveData.Houses.Sum(h => h.Floors.Count(f => TenantRules.CanTalk(h, f)));
        SaveData.PendingDaySummary = summary;
        GameEndRules.Evaluate(SaveData);
        SaveGame();
        OpenCurrentGame();
    }
    public static GameManager Instance { get; private set; }
    public bool HasPendingDisasters => SaveData.PendingDisasters.Count > 0;
    public DisasterEvent CurrentDisaster => HasPendingDisasters ? SaveData.PendingDisasters[0] : null;

    public void OpenCurrentGame()
    {
        if (SaveData.PendingDaySummary != null)
            GetTree().ChangeSceneToFile("res://scenes/DaySummary.tscn");
        else if (HasPendingDisasters)
        {
            SelectedRegion = CurrentDisaster.Region;
            GetTree().ChangeSceneToFile("res://scenes/şehir.tscn");
        }
        else if (SaveData.IsFinished) GetTree().ChangeSceneToFile("res://scenes/GameEnd.tscn");
        else GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }

    public void AcknowledgeDaySummary(bool advanceDay = false)
    {
        if (SaveData.PendingDaySummary == null) return;
        SaveData.PendingDaySummary = null;
        if (advanceDay && !HasPendingDisasters && !SaveData.IsFinished)
        {
            AdvanceToNextDay();
            return;
        }
        SaveGame();
        OpenCurrentGame();
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


