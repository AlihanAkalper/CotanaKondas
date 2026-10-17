using Godot;
using System;

public partial class TextureButton : Godot.TextureButton
{
	[Export] public Ev_numaraları RegionType;
	[Export] private StatPanel _statPanel;
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Pressed += OnPressed;

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
	public override void _Process(double delta)
	{
	}
}
