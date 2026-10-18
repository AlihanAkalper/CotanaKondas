using System.Collections.Generic;

public enum AfetTürü
{
    Yok = 0,
    Yangın = 1,
    Sel = 2,
    Deprem = 3
}

public static class DisasterDatabase
{
    public static readonly Dictionary<(AfetTürü, Materyal), float> MaterialMultipliers = new()
    {
        {(AfetTürü.Yangın, Materyal.Odun), 2f},
        {(AfetTürü.Yangın, Materyal.Tuğla), 1f},
        {(AfetTürü.Yangın, Materyal.Çelik), 2f},
        {(AfetTürü.Sel, Materyal.Odun), 2f},
        {(AfetTürü.Sel, Materyal.Tuğla), 2f},
        {(AfetTürü.Sel, Materyal.Çelik), 2f},
        {(AfetTürü.Deprem, Materyal.Odun), 2f},
        {(AfetTürü.Deprem, Materyal.Tuğla), 2f},
        {(AfetTürü.Deprem, Materyal.Çelik), 2f}
    };
}

public class RegionDisasterConfig
{
    public Dictionary<AfetTürü, float> DisasterChances { get; set; } = new();
}

public static class RegionDisasterDatabase
{
    // Oyun içi olasılıklar; gerçek risk istatistikleri değildir.
    public static readonly Dictionary<Ev_numaraları, RegionDisasterConfig> RegionConfigs = new()
    {
        { Ev_numaraları.Marmara, Config(35, 20, 15) },
        { Ev_numaraları.Ege, Config(30, 45, 15) },
        { Ev_numaraları.İçanadolu, Config(15, 20, 25) },
        { Ev_numaraları.Karadeniz, Config(20, 15, 60) },
        { Ev_numaraları.Doğuanadolu, Config(45, 15, 20) },
        { Ev_numaraları.Güneydoğu, Config(25, 45, 15) },
        { Ev_numaraları.Akdeniz, Config(20, 50, 30) },
        { Ev_numaraları.Kıbrıs, Config(20, 45, 15) }
    };
    private static RegionDisasterConfig Config(float earthquake, float fire, float flood) => new()
    {
        DisasterChances = new()
        {
            { AfetTürü.Deprem, earthquake },
            { AfetTürü.Yangın, fire },
            { AfetTürü.Sel, flood }
        }
    };
}
