using Godot;
using System;
using System.Collections.Generic;

public static class MaterialPrices
{
    public static readonly Dictionary<Materyal, int> UnitPrices = new()
    {
        { Materyal.Odun,  100 },
        { Materyal.Tuğla, 250 },
        { Materyal.Çelik, 600 }
    };
}
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
public class HouseSaveData // laod save yapıldığında neler kaydedilecekse
{
    public Ev_numaraları Type { get; set; }
    public bool IsBuilt { get; set; } = false;
    public Materyal material { get; set; } = Materyal.Alınmadı;
    public int Price { get; set; }
    public int MaterialAmount { get; set; } = 0;

    public float Health { get; set; } = ConstructionRules.BuildingHealth;
    public int ConstructionVersion { get; set; }
    public int MaterialLevel { get; set; }
    public List<FloorSaveData> Floors { get; set; } = new();

}

[Serializable]
public class GameSaveData
{
    public List<DisasterEvent> PendingDisasters { get; set; } = new();
    public int CurrentDay { get; set; } = 1;
    public int Money { get; set; } = 200000;
    public float CumulativeDisasterChance { get; set; } = 15f;

    public List<HouseSaveData> Houses { get; set; } = new();
    public List<PerkType> PurchasedPerks { get; set; } = new();

    public GameSaveData()
    {
        int[] housePrices = new int[8] { 150000, 300000, 600000, 1000000, 1800000, 3000000, 5000000, 8500000 };

        for (int i = 0; i < 8; i++)
        {
            // İleride BinaSaveData'ya geçilecekse buraya entegre edilebilir:
            // var binaKayit = new Bina.BinaSaveData();

            Houses.Add(new HouseSaveData
            {
                Type = (Ev_numaraları)i,
                IsBuilt = false,
                ConstructionVersion = 1,
                material = Materyal.Alınmadı,
                Price = housePrices[i],
            });
        }
    }
}

public class FloorSaveData
{
    public float Health { get; set; } = ConstructionRules.FloorHealth;
}

