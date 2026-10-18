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
    // Asset dosya yollarını projenizdeki res:// yollarına göre düzenleyebilirsiniz
    public static readonly Dictionary<PerkType, PerkInfo> Perks = new()
    {
        {
            PerkType.AfetPlanKitapçığı,
            new PerkInfo(PerkType.AfetPlanKitapçığı, "Afet Plan Kitapçığı", "Hasarı %5 azaltır.", 10000, AfetTürü.Yok, 0.95f, "res://assets/perks/kitapcik.png")
        },
        {
            PerkType.DepremÇantası,
            new PerkInfo(PerkType.DepremÇantası, "Deprem Çantası", "Deprem hasarını %10 azaltır.", 15000, AfetTürü.Deprem, 0.90f, "res://assets/perks/deprem_cantasi.png")
        },
        {
            PerkType.AfetSigortası,
            new PerkInfo(PerkType.AfetSigortası, "Afet Sigortası", "Tüm afet hasarlarını %25 karşılar.", 80000, AfetTürü.Yok, 0.75f, "res://assets/perks/sigorta.png")
        },
        {
            PerkType.DumanDedektörü,
            new PerkInfo(PerkType.DumanDedektörü, "Duman Dedektörü", "Yangın hasarını %10 azaltır.", 20000, AfetTürü.Yangın, 0.90f, "res://assets/perks/duman_dedektoru.png")
        },
        {
            PerkType.YangınSöndürmeSistemi,
            new PerkInfo(PerkType.YangınSöndürmeSistemi, "Yangın Söndürme Sistemi", "Yangın hasarını %25 azaltır.", 60000, AfetTürü.Yangın, 0.75f, "res://assets/perks/yangin_tupu.png")
        },
        {
            PerkType.ElektrikTesisatıKontrolü,
            new PerkInfo(PerkType.ElektrikTesisatıKontrolü, "Elektrik Tesisatı Kontrolü", "Yangın hasarını %15 azaltır.", 35000, AfetTürü.Yangın, 0.85f, "res://assets/perks/elektrik.png")
        },
        {
            PerkType.SelUyarıSistemi,
            new PerkInfo(PerkType.SelUyarıSistemi, "Sel Uyarı Sistemi", "Sel hasarını %10 azaltır.", 25000, AfetTürü.Sel, 0.90f, "res://assets/perks/sel_uyari.png")
        },
        {
            PerkType.SuBorularıGeliştirme,
            new PerkInfo(PerkType.SuBorularıGeliştirme, "Su Boruları Geliştirme", "Sel hasarını %20 azaltır.", 50000, AfetTürü.Sel, 0.80f, "res://assets/perks/su_borulari.png")
        },
        {
            PerkType.MazgalTemizleme,
            new PerkInfo(PerkType.MazgalTemizleme, "Mazgal Temizleme", "Sel hasarını %15 azaltır.", 30000, AfetTürü.Sel, 0.85f, "res://assets/perks/mazgal.png")
        }
    };
}