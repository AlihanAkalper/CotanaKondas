using Godot;
using System;
using System.Collections.Generic;

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

    public int InvestedWood { get; set; } = 0;
    public int InvestedBricks { get; set; } = 0;
    public int InvestedSteel { get; set; } = 0;
}

[Serializable]
public class GameSaveData
{
    public int CurrentDay { get; set; } = 1;
    public int Money { get; set; } = 200000;

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
