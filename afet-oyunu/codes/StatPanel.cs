using Godot;
using System;

public partial class StatPanel : Control
{
	[Export] private Label _lblArsaAd;
    [Export] private Label _lblArsaÜcreti;
    [Export] private Label _lblPahalıMateryal;
    [Export] private Label _lblUcuzMateryal;
    [Export] private Label _lblMinKira;
    [Export] private Label _lblİnşaEdildi;

    public override void _Ready()
	{
        Visible = false;
    }

    public void ShowStats(Ev_numaraları region)
    {
        var info = RegionDatabase.Regions[region];
        var houseData = GameManager.Instance.SaveData.Houses.Find(h => h.Type == region);

        _lblArsaAd.Text = info.Name;
        _lblArsaÜcreti.Text = $"Arsa Fiyatı: {info.BasePlotPrice:N0} TL";
        _lblPahalıMateryal.Text = $"Pahalı Malzeme: {info.MostExpensiveMaterial}";
        _lblUcuzMateryal.Text = $"Ucuz Malzeme: {info.MostExpensiveMaterial}";
        _lblMinKira.Text = $"Min. Kira Getirisi: {info.MinRentIncome:N0} TL/Gün";

        if (houseData != null && houseData.IsBuilt)
            _lblİnşaEdildi.Text = $"Durum: İnşa Edildi ({houseData.material})";
        else
            _lblİnşaEdildi.Text = "Durum: Satılık Boş Arsa";

        Visible = true;
    }

    public void HideStats()
    {
        Visible = false;
    }
    public override void _Process(double delta)
    {
    }
}
