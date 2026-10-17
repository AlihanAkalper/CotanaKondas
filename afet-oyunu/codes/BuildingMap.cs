using Godot;
using System;

public partial class BuildingMap : Node2D
{
    [Export] private DisasterAnimationManager _disasterAnimManager;
    [Export] private Sprite2D _plotOrHouseSprite;
    [Export] private Texture2D _emptyPlotTexture;
    [Export] private Texture2D _woodHouseTexture;
    [Export] private Texture2D _brickHouseTexture;
    [Export] private Texture2D _steelHouseTexture;
    [Export] private Button _buyPlotButton;
    [Export] private BuildMenu _buildMenu;

    public override void _Ready()
    {
        var region = GameManager.Instance.SelectedRegion;

        if (_buyPlotButton != null && _buildMenu != null)
        {
            _buyPlotButton.Pressed += () => _buildMenu.Open(region);
        }

        RefreshScene();

        // Aktif afet kontrolü (Enum adını projene göre kontrol et: AfetTuru.None veya AfetTürü.Yok)
        if (GameManager.Instance.ActiveDisaster != AfetTürü.Yok)
        {
            GD.Print($"DİKKAT: Bu bölgede {GameManager.Instance.ActiveDisaster} afeti yaşanıyor!");
            _disasterAnimManager?.PlayDisasterAnimation(GameManager.Instance.ActiveDisaster);
            GameManager.Instance.ActiveDisaster = AfetTürü.Yok;
        }
    }

    public void RefreshScene()
    {
        var region = GameManager.Instance.SelectedRegion;
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == region);

        if (house == null || _plotOrHouseSprite == null) return;

        if (house.IsBuilt)
        {
            if (_buyPlotButton != null) _buyPlotButton.Visible = false;

            _plotOrHouseSprite.Texture = house.material switch
            {
                Materyal.Odun => _woodHouseTexture,
                Materyal.Tuğla => _brickHouseTexture,
                Materyal.Çelik => _steelHouseTexture,
                _ => _emptyPlotTexture
            };
        }
        else
        {
            if (_buyPlotButton != null) _buyPlotButton.Visible = true;
            _plotOrHouseSprite.Texture = _emptyPlotTexture;
        }
    }

    public void OnBackToMapButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/TurkeyMap.tscn");
    }
}