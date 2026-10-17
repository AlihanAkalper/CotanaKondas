using Godot;
using System;
using System.Collections.Generic;


public class RegionInfo
{
	public string Name { get; set; }
	public int BasePlotPrice { get; set; }
	public string MostExpensiveMaterial { get; set; }
	public int MinRentIncome {  get; set; }
	public RegionInfo(string name, int basePlotPrice, string mostExpensiveMaterial, int minRentIncome)
    {
        Name = name;
        BasePlotPrice = basePlotPrice;
        MostExpensiveMaterial = mostExpensiveMaterial;
        MinRentIncome = minRentIncome;
    }
}

public static class RegionDatabase
{
    public static readonly Dictionary<Ev_numaraları, RegionInfo> Regions = new()
    {
        { Ev_numaraları.Marmara,     new RegionInfo("Marmara", 150000, "Çelik", 1500) },
        { Ev_numaraları.Ege,         new RegionInfo("Ege", 300000, "Tuğla", 2800) },
        { Ev_numaraları.İçanadolu,   new RegionInfo("İç Anadolu", 600000, "Çelik", 5500) },
        { Ev_numaraları.Karadeniz,   new RegionInfo("Karadeniz", 1000000, "Odun", 9000) },
        { Ev_numaraları.Doğuanadolu, new RegionInfo("Doğu Anadolu", 1800000, "Çelik", 16000) },
        { Ev_numaraları.Güneydoğu,   new RegionInfo("Güneydoğu Anadolu", 3000000, "Tuğla", 27000) },
        { Ev_numaraları.Akdeniz,     new RegionInfo("Akdeniz", 5000000, "Çelik", 45000) },
        { Ev_numaraları.Kıbrıs,      new RegionInfo("Kıbrıs", 8500000, "Çelik", 80000) }
    };
}
