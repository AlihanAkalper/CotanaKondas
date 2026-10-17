using Godot;
using System;
using System.Collections.Generic;


public static class MaterialPrices
{
    public static readonly Dictionary<Materyal, int> UnitPrices = new()
    {
        { Materyal.Odun,  100 },  // 1 adet Odun = 100 TL
        { Materyal.Tuğla, 250 },  // 1 adet Tuğla = 250 TL
        { Materyal.Çelik, 600 }   // 1 adet Çelik = 600 TL
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

    public float Health { get; set; } = 100f;

}

[Serializable]
public class GameSaveData
{
    public int CurrentDay { get; set; } = 1;
    public int Money { get; set; } = 200000;

    public float CumulativeDisasterChance { get; set; } = 15f;

    public List<HouseSaveData> Houses { get; set; } = new();

    public GameSaveData()
    {

        int[] housePrices = new int[8] { 150000, 300000, 600000, 1000000, 1800000, 3000000, 5000000, 8500000 };//deişebilir

        for (int i = 0; i < 8; i++)
        {
            Houses.Add(new HouseSaveData
            {
                Type = (Ev_numaraları)i,
                IsBuilt = false, 
                material = Materyal.Alınmadı,
                Price = housePrices[i],
            });
        }
    }
}
