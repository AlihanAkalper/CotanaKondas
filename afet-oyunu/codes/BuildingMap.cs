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
    [Export] private TextureButton _perkButton;
    [Export] private PerkMenu _perkMenu;
    [Export] private BaseButton _constructionButton;

    [Export] private PlotConfirmDialog _confirmDialog; // Yeni onay paneli
    [Export] private BuildMenu _buildMenu;             // Malzeme menüsü
    [Export] private TextureButton _backButton;

    private Ev_numaraları _currentRegion;
    private static readonly string[] BackgroundPaths =
    {
        "res://assets/akdeniz.png",
        "res://assets/ege.png",
        "res://assets/ucuncuBolge.png"
    };

    public override void _Ready()
    {
        _currentRegion = GameManager.Instance.SelectedRegion;
        ApplyRegionBackground();

        if (_backButton != null)
        {
            _backButton.Pressed += () => OnBackToMapButtonPressed();
        }

        if (_plotButton != null)
        {
            _plotButton.Pressed += OnPlotClicked;
        }
        if (_perkButton != null)
        {
            _perkButton.Pressed += () => { if (GameManager.Instance.HasPendingDisasters) return; _buildMenu?.Hide(); if(_perkMenu.Visible) _perkMenu.Hide(); else _perkMenu?.Open(); };
        }

        if (_constructionButton != null)
            _constructionButton.Pressed += () => { _perkMenu?.Hide(); if(_buildMenu.Visible) _buildMenu.Hide(); else OpenBuildMenu(); };

        RefreshScene();

        if (GameManager.Instance.HasPendingDisasters)
        {
            var report = GameManager.Instance.CurrentDisaster;
            var results = new DisasterResultPanel();
            AddChild(results);
            results.Begin(report, GameManager.Instance.SaveData.PendingDisasters.Count);
            if (_disasterAnimManager != null)
            {
                _disasterAnimManager.Completed += results.ShowResults;
                _disasterAnimManager.PlayDisasterAnimation(report.DisasterType);
            }
            else results.ShowResults();
        }
    }
    private void ApplyRegionBackground()
    {
        var game = GameManager.Instance;
        var house = game.SaveData.Houses.Find(h => h.Type == _currentRegion);
        var background = GetNodeOrNull<Sprite2D>("Sprite2D");
        if (house == null || background == null) return;

        if (house.BackgroundIndex < 0 || house.BackgroundIndex >= BackgroundPaths.Length)
        {
            house.BackgroundIndex = Random.Shared.Next(BackgroundPaths.Length);
            game.SaveGame();
        }

        // Görseller farklı boyutta olsa da mevcut arka plan alanını kaplasın.
        Vector2 displaySize = background.Texture.GetSize() * background.Scale;
        background.Texture = GD.Load<Texture2D>(BackgroundPaths[house.BackgroundIndex]);
        background.Scale = displaySize / background.Texture.GetSize();
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
        if (GameManager.Instance.HasPendingDisasters) return;
        _buildMenu?.Open(_currentRegion);
    }

    public void RefreshScene()
    {
        GetNodeOrNull<Bina>("Bina")?.Refresh();
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
        if (GameManager.Instance.HasPendingDisasters) return;
        GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }
}


