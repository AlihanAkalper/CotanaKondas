using Godot;
using System;

public partial class HUD : CanvasLayer
{
    public static HUD Instance { get; set; }

    [Export] private Label _moneyLabel;
    [Export] private Label _dateLabel;


    public override void _Ready()
    {
        Instance = this;
        UpdateHUD();
    }


    public void UpdateHUD()
    {
		if(GameManager.Instance?.SaveData == null) return ;
		_moneyLabel.Text = $"Para: {GameManager.Instance.SaveData.Money:N0} TL";
        _dateLabel.Text = $"Gün: {GameManager.Instance.SaveData.CurrentDay}";
	}
}
