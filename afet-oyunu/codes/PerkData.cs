using Godot;
using System;
using System.Collections.Generic;

public enum PerkType
{
    AfetPlanKitapçığı = 0,
    DepremÇantası = 1,
    AfetSigortası = 2,
    DumanDedektörü = 3,
    YangınSöndürmeSistemi = 4,
    ElektrikTesisatıKontrolü = 5,
    SelUyarıSistemi = 6,
    SuBorularıGeliştirme = 7,
    MazgalTemizleme = 8
}

public class PerkInfo
{
    public PerkType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public AfetTürü AffectedDisaster { get; set; }
    public float DamageMultiplier { get; set; }
    public string IconPath { get; set; } // Perk ikon görselinin yolu

    public PerkInfo(PerkType type, string name, string desc, int price, AfetTürü disaster, float multiplier, string iconPath)
    {
        Type = type;
        Name = name;
        Description = desc;
        Price = price;
        AffectedDisaster = disaster;
        DamageMultiplier = multiplier;
        IconPath = iconPath;
    }
}

public static class PerkDatabase
{
    public static float GetDamageMultiplier(AfetTürü disaster, IEnumerable<PerkType> purchasedPerks)
    {
        if (disaster == AfetTürü.Yok || purchasedPerks == null) return 1f;

        float multiplier = 1f;
        // Kayıtta yinelenen bir geliştirme varsa etkisini yalnızca bir kez uygula.
        foreach (var perkType in new HashSet<PerkType>(purchasedPerks))
        {
            if (!Perks.TryGetValue(perkType, out var perk)) continue;

            // Yok, geliştirme tanımlarında tüm afetleri temsil eder.
            if (perk.AffectedDisaster == AfetTürü.Yok || perk.AffectedDisaster == disaster)
                multiplier *= perk.DamageMultiplier;
        }

        return multiplier;
    }

    // Asset dosya yollarını projenizdeki res:// yollarına göre düzenleyebilirsiniz
    public static readonly Dictionary<PerkType, PerkInfo> Perks = new()
    {
        {
            PerkType.AfetPlanKitapçığı,
            new PerkInfo(PerkType.AfetPlanKitapçığı, "Afet Plan Kitapçığı", "Hasarı %5 azaltır.", 10000, AfetTürü.Yok, 0.95f, "res://assets/afetKitap.png")
        },
        {
            PerkType.DepremÇantası,
            new PerkInfo(PerkType.DepremÇantası, "Deprem Çantası", "Deprem hasarını %10 azaltır.", 15000, AfetTürü.Deprem, 0.90f, "res://assets/depremCanta.png")
        },
        {
            PerkType.AfetSigortası,
            new PerkInfo(PerkType.AfetSigortası, "Afet Sigortası", "Tüm afet hasarlarını %25 karşılar.", 80000, AfetTürü.Yok, 0.75f, "res://assets/sigorta.png")
        },
        {
            PerkType.DumanDedektörü,
            new PerkInfo(PerkType.DumanDedektörü, "Duman Dedektörü", "Yangın hasarını %10 azaltır.", 20000, AfetTürü.Yangın, 0.90f, "res://assets/dumanDedektort.png")
        },
        {
            PerkType.YangınSöndürmeSistemi,
            new PerkInfo(PerkType.YangınSöndürmeSistemi, "Yangın Söndürme Sistemi", "Yangın hasarını %25 azaltır.", 60000, AfetTürü.Yangın, 0.75f, "res://assets/yanginTupu.png")
        },
        {
            PerkType.ElektrikTesisatıKontrolü,
            new PerkInfo(PerkType.ElektrikTesisatıKontrolü, "Elektrik Tesisatı Kontrolü", "Yangın hasarını %15 azaltır.", 35000, AfetTürü.Yangın, 0.85f, "res://assets/kontrolKalem.png")
        },
        {
            PerkType.SelUyarıSistemi,
            new PerkInfo(PerkType.SelUyarıSistemi, "Sel Uyarı Sistemi", "Sel hasarını %10 azaltır.", 25000, AfetTürü.Sel, 0.90f, "res://assets/SelUyariSistem.png")
        },
        {
            PerkType.SuBorularıGeliştirme,
            new PerkInfo(PerkType.SuBorularıGeliştirme, "Su Boruları Geliştirme", "Sel hasarını %20 azaltır.", 50000, AfetTürü.Sel, 0.80f, "res://assets/suBoruGelistir.png")
        },
        {
            PerkType.MazgalTemizleme,
            new PerkInfo(PerkType.MazgalTemizleme, "Mazgal Temizleme", "Sel hasarını %15 azaltır.", 30000, AfetTürü.Sel, 0.85f, "res://assets/mazgal.png")
        }
    };
}

