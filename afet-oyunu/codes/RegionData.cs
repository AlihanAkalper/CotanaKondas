using Godot;
using System;
using System.Collections.Generic;


public class RegionInfo
{
	public string Name { get; set; }
	public int BasePlotPrice { get; set; }
	public string MostExpensiveMaterial { get; set; }
	// Taban gelir, düşük arsa fiyatlı bölgelerde pahalı yapı malzemesini erişilebilir tutar.
	public int MinRentIncome => 2500 + (int)Math.Round(BasePlotPrice * 0.012m);
	public RegionInfo(string name, int basePlotPrice, string mostExpensiveMaterial)
    {
        Name = name;
        BasePlotPrice = basePlotPrice;
        MostExpensiveMaterial = mostExpensiveMaterial;
    }
}

public static class RegionDatabase
{
    public static readonly Dictionary<Ev_numaraları, RegionInfo> Regions = new()
    {
        // Bölgesel pahalılığı temsil eden sabit oyun değerleri; piyasa fiyatı değildir.
        // Marmara ve turistik kıyılar pahalı, doğu bölgeleri daha erişilebilir.
        { Ev_numaraları.Marmara,     new RegionInfo("Marmara", 300000, "Çelik") },
        { Ev_numaraları.Ege,         new RegionInfo("Ege", 270000, "Tuğla") },
        { Ev_numaraları.İçanadolu,   new RegionInfo("İç Anadolu", 180000, "Çelik") },
        { Ev_numaraları.Karadeniz,   new RegionInfo("Karadeniz", 160000, "Odun") },
        { Ev_numaraları.Doğuanadolu, new RegionInfo("Doğu Anadolu", 100000, "Çelik") },
        { Ev_numaraları.Güneydoğu,   new RegionInfo("Güneydoğu Anadolu", 125000, "Tuğla") },
        { Ev_numaraları.Akdeniz,     new RegionInfo("Akdeniz", 250000, "Çelik") },
        { Ev_numaraları.Kıbrıs,      new RegionInfo("Kıbrıs", 230000, "Çelik") }
    };
}
