using Godot;

public partial class HUD : CanvasLayer
{
    public static HUD Instance { get; private set; }

    [Export] private Label _moneyLabel;
    [Export] private Label _dateLabel;
    [Export] private Button _mainMenuButton;

    public override void _Ready()
    {
        Instance = this;
        UpdateHUD();
        if (_mainMenuButton != null)
            _mainMenuButton.Pressed += ReturnToMainMenu;
    }

    private void ReturnToMainMenu()
    {
        if (GameManager.Instance.HasPendingDisasters) return;
        GameManager.Instance.SaveGame();

        GetTree().ChangeSceneToFile("res://scenes/GirişEkranı.tscn");
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
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

