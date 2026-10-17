using Godot;
using System;

public partial class BuildingMap : Node2D
{
    [Export] private DisasterAnimationManager _disasterAnimManager;
    [Export] private TextureButton _plotButton; // Boş arsa tık alanı (veya TextureButton)
    [Export] private Texture2D _emptyPlotTexture;
    [Export] private Texture2D _woodHouseTexture;
    [Export] private Texture2D _brickHouseTexture;
    [Export] private Texture2D _steelHouseTexture;

    [Export] private PlotConfirmDialog _confirmDialog; // Yeni onay paneli
    [Export] private BuildMenu _buildMenu;             // Malzeme menüsü
    [Export] private TextureButton _backButton;

    private Ev_numaraları _currentRegion;

    public override void _Ready()
    {
        _currentRegion = GameManager.Instance.SelectedRegion;

        if (_backButton != null)
        {
            _backButton.Pressed += () => OnBackToMapButtonPressed();
        }

        if (_plotButton != null)
        {
            _plotButton.Pressed += OnPlotClicked;
        }

        RefreshScene();

        // Afet kontrolü
        if (GameManager.Instance.ActiveDisaster != AfetTürü.Yok)
        {
            AfetTürü currentDisaster = GameManager.Instance.ActiveDisaster;

            GD.Print($"Şehir Sahnesi: {currentDisaster} tespit edildi. 0.3 saniye sonra animasyon başlayacak.");

            // Sahnenin oturması için 0.3 saniye bekleyip animasyonu başlat
            GetTree().CreateTimer(0.3f).Timeout += () =>
            {
                _disasterAnimManager?.PlayDisasterAnimation(currentDisaster);
            };

            // Afeti tüket (tekrar tekrar oynamasın)
            GameManager.Instance.ActiveDisaster = AfetTürü.Yok;
        }
    }

    private void OnPlotClicked()
    {
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == _currentRegion);

        // Eğer ev henüz yapılmamışsa onay penceresini aç
        if (house != null && !house.IsBuilt)
        {
            _confirmDialog?.Open(_currentRegion);
        }
        else
        {
            GD.Print("Bu arsa zaten inşa edilmiş!");
        }
    }

    public void OpenBuildMenu()
    {
        _buildMenu?.Open(_currentRegion);
    }

    public void RefreshScene()
    {
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == _currentRegion);
        if (house == null || _plotButton == null) return;

        if (house.IsBuilt)
        {
            // Ev inşa edildiyse ilgili malzeme görselini bas
            _plotButton.TextureNormal = house.material switch
            {
                Materyal.Odun => _woodHouseTexture,
                Materyal.Tuğla => _brickHouseTexture,
                Materyal.Çelik => _steelHouseTexture,
                _ => _emptyPlotTexture
            };
        }
        else
        {
            // Boş arsa görseli
            _plotButton.TextureNormal = _emptyPlotTexture;
        }
    }

    public void OnBackToMapButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }
}