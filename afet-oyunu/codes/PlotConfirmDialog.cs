using Godot;
using System;

public partial class PlotConfirmDialog : Control
{
    [Export] private Label _messageLabel;
    [Export] private Button _yesButton;
    [Export] private Button _cancelButton;

    private Ev_numaraları _region;
    private int _price;

    public override void _Ready()
    {
        Visible = false;

        _yesButton.Pressed += OnYesPressed;
        _cancelButton.Pressed += () => Visible = false;
    }

    public void Open(Ev_numaraları region)
    {
        _region = region;
        var info = RegionDatabase.Regions[region];
        _price = info.BasePlotPrice;

        _messageLabel.Text = $"{info.Name} arsasını {_price:N0} TL karşılığında satın almak istediğinize emin misiniz?";
        Visible = true;
    }

    private void OnYesPressed()
    {
        // 1. Para kontrolü
        if (GameManager.Instance.SaveData.Money < _price)
        {
            _messageLabel.Text = "Yetersiz bakiye! Bu arsayı alamazsınız.";
            return;
        }

        // 2. Parayı kasadan düş
        GameManager.Instance.SaveData.Money -= _price;

        // 3. Evi 'inşa edildi/satın alındı' olarak işaretle
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == _region);
        if (house != null)
        {
            house.IsBuilt = true;
        }

        // 4. Hangi bölgeye gideceğimizi GameManager'a aktar ve oyunu kaydet
        GameManager.Instance.SelectedRegion = _region;
        GameManager.Instance.SaveGame();

        // 5. Pop-up'ı kapat
        Visible = false;

        // 6. 2. Haritaya (Şehir / Arsa Sahnesine) geçiş yap:
        GetTree().ChangeSceneToFile("res://scenes/şehir.tscn");
    }
}