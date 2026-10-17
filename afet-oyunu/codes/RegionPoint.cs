using Godot;
using System;

public partial class RegionPoint : TextureButton
{
	[Export] public Ev_numaraları RegionType;
	[Export] private StatPanel _statPanel;

    [ExportGroup("Durum Dokuları")]
    [Export] private Texture2D _forSaleTexture;  
    [Export] private Texture2D _purchasedTexture;
    public override void _Ready()
	{

        if (_statPanel == null)
        {
            _statPanel = GetTree().Root.FindChild("StatPanel", true, false) as StatPanel;
        }

        MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Pressed += OnPressed;

        UpdatePointTexsture();
	}

	private void OnMouseEntered()
	{
		_statPanel?.ShowStats(RegionType);
	}

	private void OnMouseExited()
	{
		_statPanel?.HideStats();
	}

	private void OnPressed()
	{
        GD.Print($"{RegionType} seçildi. 2. Haritaya (İnşaat Sahnesine) geçiliyor...");
        GameManager.Instance.SelectedRegion = RegionType;
        GetTree().ChangeSceneToFile("res://Scenes/BuildingMap.tscn"); // 2. Harita yolu
    }

	private void UpdatePointTexsture()
	{
        if (GameManager.Instance?.SaveData == null) return;

        var houseData = GameManager.Instance.SaveData.Houses.Find(h => h.Type == RegionType);

        if (houseData != null && houseData.IsBuilt)
        {
            // Arsa alınmış / yapılmışsa
            if (_purchasedTexture != null)
                TextureNormal = _purchasedTexture;
        }
        else
        {
            // Arsa henüz alınmamış / satılıksa
            if (_forSaleTexture != null)
                TextureNormal = _forSaleTexture;
        }
    }
	public override void _Process(double delta)
	{
	}
}
