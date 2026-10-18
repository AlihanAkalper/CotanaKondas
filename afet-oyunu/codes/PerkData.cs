using Godot;
using System;
using static PerkData;
using System.Collections.Generic;

public partial class PerkData : Node
{

    public enum PerkType
    {
        AfetPlanKitapçığı = 0,
        DepremÇantası = 1,
        AfetSigortası = 2,
        DumanDedektörü = 3,
        YangınSöndürmeSistemi = 4,
        ElektrikTesisaıKontrolü = 5,
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
        public float DamageMultiplier { get; set; } // Örn: 0.9f

        public PerkInfo(PerkType type, string name, string desc, int price, AfetTürü disaster, float multiplier)
        {
            Type = type;
            Name = name;
            Description = desc;
            Price = price;
            AffectedDisaster = disaster;
            DamageMultiplier = multiplier;
        }
    }

    public static class PerkDatabase
    {
        public static readonly Dictionary<PerkType, PerkInfo> Perks = new()
    {
        // 1. GENEL / TÜM AFETLER
        {
            PerkType.AfetPlanKitapçığı,
            new PerkInfo(
                PerkType.AfetPlanKitapçığı,
                "Afet Plan Kitapçığı",
                "Tüm afetlerde panik kaybını önler, hasarı %5 azaltır.",
                10000,
                AfetTürü.Yok, // Tüm afetlere uygulanır
                0.95f
            )
        },
        {
            PerkType.AfetSigortası,
            new PerkInfo(
                PerkType.AfetSigortası,
                "Afet Sigortası",
                "Afet hasarını doğrudan %25 sübvanse eder.",
                80000,
                AfetTürü.Yok, // Tüm afetlere uygulanır
                0.75f
            )
        },

        // 2. DEPREM PERKLERİ
        {
            PerkType.DepremÇantası,
            new PerkInfo(
                PerkType.DepremÇantası,
                "Deprem Çantası",
                "Deprem sonrası ilk müdahaleyi hızlandırır, hasarı %10 azaltır.",
                15000,
                AfetTürü.Deprem,
                0.90f
            )
        },

        // 3. YANGIN PERKLERİ
        {
            PerkType.DumanDedektörü,
            new PerkInfo(
                PerkType.DumanDedektörü,
                "Duman Dedektörü",
                "Yangını erken fark ettirir, yangın hasarını %10 azaltır.",
                20000,
                AfetTürü.Yangın,
                0.90f
            )
        },
        {
            PerkType.YangınSöndürmeSistemi,
            new PerkInfo(
                PerkType.YangınSöndürmeSistemi,
                "Yangın Söndürme Sistemi (Sprinkler)",
                "Otomatik su fıskiyeleri yangın hasarını %25 azaltır.",
                60000,
                AfetTürü.Yangın,
                0.75f
            )
        },
        {
            PerkType.ElektrikTesisaıKontrolü,
            new PerkInfo(
                PerkType.ElektrikTesisaıKontrolü,
                "Elektrik Tesisatı Kontrolü",
                "Kısa devre yangınlarını önler, yangın hasarını %15 azaltır.",
                35000,
                AfetTürü.Yangın,
                0.85f
            )
        },

        // 4. SEL PERKLERİ
        {
            PerkType.SelUyarıSistemi,
            new PerkInfo(
                PerkType.SelUyarıSistemi,
                "Erken Sel Uyarı Sistemi",
                "Bölgesel sel uyarısıyla önlem aldırır, hasarı %10 azaltır.",
                25000,
                AfetTürü.Sel,
                0.90f
            )
        },
        {
            PerkType.SuBorularıGeliştirme,
            new PerkInfo(
                PerkType.SuBorularıGeliştirme,
                "Su Boruları & Tesisat Geliştirme",
                "Geri basma ve su patlamalarını önler, sel hasarını %20 azaltır.",
                50000,
                AfetTürü.Sel,
                0.80f
            )
        },
        {
            PerkType.MazgalTemizleme,
            new PerkInfo(
                PerkType.MazgalTemizleme,
                "Mazgal & Drenaj Temizliği",
                "Su tahliyesini hızlandırır, sel hasarını %15 azaltır.",
                30000,
                AfetTürü.Sel,
                0.85f
            )
        }
    };
    }
}
