using Godot;

public partial class HUD : CanvasLayer
{
    public static HUD Instance { get; private set; }

    [Export] private Label _moneyLabel;
    [Export] private Label _dateLabel;

    public override void _Ready()
    {
        Instance = this;
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        if (GameManager.Instance?.SaveData == null) return;

        // "Para: " kelimesi kaldırıldı, sadece tutar yazılıyor:
        if (_moneyLabel != null)
        {
            _moneyLabel.Text = $"{GameManager.Instance.SaveData.Money:N0} TL";
        }

        if (_dateLabel != null)
        {
            _dateLabel.Text = $"Gün: {GameManager.Instance.SaveData.CurrentDay}";
        }
    }
}