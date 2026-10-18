using Godot;
using System;
using System.Collections.Generic;

public partial class DisasterManager : Node
{
    public static DisasterManager Instance { get; private set; }
    private readonly Random _random = new();
    public override void _Ready() => Instance = this;

    public List<DisasterEvent> ProcessDailyDisasters()
    {
        var data = GameManager.Instance.SaveData;
        var events = new List<DisasterEvent>();
        if (data.CurrentDay <= 5) return events;
        if (_random.NextDouble() * 100 < data.CumulativeDisasterChance)
        {
            foreach (var house in data.Houses)
            {
                if (!house.IsBuilt || house.Health <= 0 || house.Floors.Count == 0) continue;
                if (!RegionDisasterDatabase.RegionConfigs.TryGetValue(house.Type, out var config)) continue;
                // Her afet için bağımsız zar; birden fazlası tutarsa bölgeye tek birini rastgele seç.
                var candidates = new List<AfetTürü>();
                foreach (var (type, chance) in config.DisasterChances)
                    if (_random.NextDouble() * 100 < chance) candidates.Add(type);
                if (candidates.Count > 0)
                    events.Add(DisasterResolution.Resolve(data, house, candidates[_random.Next(candidates.Count)]));
            }
        }
        data.CumulativeDisasterChance = events.Count > 0 ? 15f : Math.Min(100f, data.CumulativeDisasterChance + 1f);
        return events;
    }

    // Eski düğme bağlantıları da tek gün ilerletme akışını kullansın.
    public void OnNextDayButtonPressed() => GameManager.Instance.AdvanceToNextDay();
}
