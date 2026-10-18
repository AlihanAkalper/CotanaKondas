using Godot;
using System.Linq;

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
        var materials = ConstructionRules.BasePrices.Keys.OrderBy(m => ConstructionRules.MaterialPrice(region, m)).ToArray();
        _lblPahalıMateryal.Text = $"Pahalı Malzeme: {materials[^1]}";
        _lblUcuzMateryal.Text = $"Ucuz Malzeme: {materials[0]}";
        int dailyIncome = houseData == null ? 0 : ConstructionRules.DailyIncome(houseData);
        _lblMinKira.Text = $"Günlük Kira Getirisi: {dailyIncome:N0} TL\n" +
            $"Kat Başına Temel Kira: {info.MinRentIncome:N0} TL/Gün";

        if (houseData != null && houseData.IsBuilt)
            _lblİnşaEdildi.Text = $"Durum: {houseData.material} • {houseData.Floors.Count}/{ConstructionRules.MaxFloors} kat";
        else
            _lblİnşaEdildi.Text = "Durum: Satılık Boş Arsa";

        Visible = true;
    }

    public void HideStats()
    {
        Visible = false;
    }
}

