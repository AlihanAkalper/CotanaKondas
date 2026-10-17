using Godot;
using System;

public partial class BuildingMap : Node2D
{
    // Inspector'dan bağlayabileceğimiz afet animasyon yöneticisi
    [Export] private DisasterAnimationManager _disasterAnimManager;

    public override void _Ready()
    {
        // 1. Oyuncunun 1. haritada tıkladığı (veya afetin olduğu) bölgeyi al
        var region = GameManager.Instance.SelectedRegion;

        // 2. O bölgeye ait kayıtlı evi veritabanından çek
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == region);

        GD.Print($"2. Harita Açıldı: {region} bölgesi inceleniyor.");

        // 3. Eğer GameManager'da kayıtlı bir aktif afet varsa animasyonu başlat
        if (GameManager.Instance.ActiveDisaster != AfetTürü.Yok)
        {
            GD.Print($"DİKKAT: Bu bölgede {GameManager.Instance.ActiveDisaster} afeti yaşanıyor!");

            // Az önce yazdığımız animasyonu oynatır (deprem, yangın veya sel)
            _disasterAnimManager?.PlayDisasterAnimation(GameManager.Instance.ActiveDisaster);

            // Animasyon bir kere oynatıldıktan sonra durumu sıfırla ki her girdiğinde tekrar etmesin
            GameManager.Instance.ActiveDisaster = AfetTürü.Yok;
        }
    }

    // Haritadan tekrar Türkiye haritasına dönmek için bir buton fonksiyonu
    public void OnBackToMapButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/TurkeyMap.tscn");
    }
}