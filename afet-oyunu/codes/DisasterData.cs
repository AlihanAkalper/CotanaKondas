using Godot;
using System;
using System.Collections.Generic;

public enum AfetTürü
{
	Yok = 0,
	Yangın = 1,
	Sel = 2,
	Deprem = 3,
	Heyelan = 4,
	Çığ = 5
}
// çarpanları burada ayarlıyoruz buradan değişikliği yapacaz
public static class DisasterDatabase
{
	public static readonly Dictionary<(AfetTürü, Materyal), float> MaterialMultipliers = new()
	{
		//yangın
		{(AfetTürü.Yangın, Materyal.Odun), 2.0f },
        {(AfetTürü.Yangın, Materyal.Tuğla), 1.0f },
        {(AfetTürü.Yangın, Materyal.Çelik), 2.0f },  


		//sel
		{(AfetTürü.Sel, Materyal.Odun), 2.0f },
        {(AfetTürü.Sel, Materyal.Tuğla), 2.0f },
        {(AfetTürü.Sel, Materyal.Çelik), 2.0f },
		

		//deprem
		{(AfetTürü.Deprem, Materyal.Odun), 2.0f },
        {(AfetTürü.Deprem, Materyal.Tuğla), 2.0f },
        {(AfetTürü.Deprem, Materyal.Çelik), 2.0f },


		//heyelan
		{(AfetTürü.Heyelan, Materyal.Odun), 2.0f },
        {(AfetTürü.Heyelan, Materyal.Tuğla), 2.0f },
        {(AfetTürü.Heyelan, Materyal.Çelik), 2.0f },


		//çığ
		{(AfetTürü.Çığ, Materyal.Odun), 2.0f },
        {(AfetTürü.Çığ, Materyal.Tuğla), 2.0f },
        {(AfetTürü.Çığ, Materyal.Çelik), 2.0f },

    };
}


//--------------------------------------------------------------------------------------------------

public class RegionDisasterConfig
{
    // Afet Türü ve gerçekleşme ihtimali (%0 - %100)
    public Dictionary<AfetTürü, float> DisasterChances { get; set; } = new();
}

public static class RegionDisasterDatabase
{
    public static readonly Dictionary<Ev_numaraları, RegionDisasterConfig> RegionConfigs = new()
    {
        // Marmara: Deprem (%35), Yangın (%20), Sel (%15)
        {
            Ev_numaraları.Marmara, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Deprem, 35f },
                    { AfetTürü.Yangın, 20f },
                    { AfetTürü.Sel,    15f }
                }
            }
        },

        // İç Anadolu: Sadece Sel (%40)
        {
            Ev_numaraları.İçanadolu, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Sel, 40f }
                }
            }
        },

        // Karadeniz: Heyelan (%50), Sel (%60)
        {
            Ev_numaraları.Karadeniz, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Heyelan, 50f },
                    { AfetTürü.Sel,     60f }
                }
            }
        },

        // Doğu Anadolu: Çığ (%45), Deprem (%30)
        {
            Ev_numaraları.Doğuanadolu, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Çığ,    45f },
                    { AfetTürü.Deprem, 30f }
                }
            }
        },

        // Akdeniz: Yangın (%60)
        {
            Ev_numaraları.Ege, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Yangın, 60f }
                }
            }
        },

        {
            Ev_numaraları.Güneydoğu, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Yangın, 60f }
                }
            }
        },

        {
            Ev_numaraları.Kıbrıs, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Yangın, 60f }
                }
            }
        },
        { 
        Ev_numaraları.Akdeniz, new RegionDisasterConfig
            {
                DisasterChances = new()
                {
                    { AfetTürü.Heyelan, 50f },
                    { AfetTürü.Sel,     60f }
                }
            }
        },

        
    };
}
