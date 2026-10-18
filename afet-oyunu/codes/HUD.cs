using Godot;

public partial class HUD : CanvasLayer
{
    public static HUD Instance { get; private set; }
    private TenantDialoguePanel _dialoguePanel;
    private Label _lossLabel;

    [Export] private Label _moneyLabel;
    [Export] private Label _dateLabel;
    [Export] private Button _mainMenuButton;

    public override void _Ready()
    {
        Instance = this;
        var lossPanel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        AddChild(lossPanel);
        lossPanel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterBottom);
        lossPanel.OffsetLeft = -165; lossPanel.OffsetRight = 165;
        lossPanel.OffsetTop = -50; lossPanel.OffsetBottom = -12;
        var lossStyle = new StyleBoxFlat {
            BgColor = new Color(.06f, .09f, .13f, .92f),
            ContentMarginLeft = 14, ContentMarginRight = 14,
            ContentMarginTop = 8, ContentMarginBottom = 8,
            CornerRadiusTopLeft = 6, CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6, CornerRadiusBottomRight = 6
        };
        lossPanel.AddThemeStyleboxOverride("panel", lossStyle);
        _lossLabel = new Label {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        _lossLabel.AddThemeFontSizeOverride("font_size", 16);
        lossPanel.AddChild(_lossLabel);
        var help = new Button { Text = "Nasıl oynanır?" };
        AddChild(help);
        help.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomLeft);
        help.OffsetLeft = 12; help.OffsetRight = 160;
        help.OffsetTop = -50; help.OffsetBottom = -12;
        var instructions = new AcceptDialog {
            Title = "Nasıl oynanır?", Exclusive = true,
            DialogText =
                "• Arsa al, çekiç menüsünden malzeme seç ve kat yap.\n" +
                "• Günleri ilerlet, kiracılardan kira kazan.\n" +
                "• Ek özelliklerle afetlere hazırlan, hasarlı katları onar.\n" +
                "• Soru işaretine tıklayıp kiracıların farkındalığını artır.\n\n" +
                "Toplam 10 kiracı kaybedersen oyun biter."
        };
        AddChild(instructions);
        help.Pressed += () => instructions.PopupCentered();
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

    public bool DialogueOpen => GodotObject.IsInstanceValid(_dialoguePanel) && !_dialoguePanel.IsQueuedForDeletion();

    public void ShowTenantDialogue(Ev_numaraları region, int floorIndex)
    {
        var game = GameManager.Instance;
        if (game == null || game.SaveData.IsFinished || game.HasPendingDisasters || DialogueOpen) return;
        var house = game.SaveData.Houses.Find(h => h.Type == region);
        if (house == null || floorIndex < 0 || floorIndex >= house.Floors.Count ||
            !TenantRules.CanTalk(house, house.Floors[floorIndex])) return;
        TenantRules.PrepareQuestion(house.Floors[floorIndex], System.Random.Shared);
        game.SaveGame();
        _dialoguePanel = new TenantDialoguePanel { Region = region, FloorIndex = floorIndex };
        AddChild(_dialoguePanel);
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
    }

    public void UpdateHUD()
    {
        if (GameManager.Instance?.SaveData == null) return;
        var save = GameManager.Instance.SaveData;
        if (_lossLabel != null)
            _lossLabel.Text = $"Kiracı kaybı: {save.LostTenants}/{GameEndRules.TenantLossLimit}";

        // "Para: " kelimesi kaldırıldı, sadece tutar yazılıyor:
        if (_moneyLabel != null)
        {
            _moneyLabel.Text = $"{GameManager.Instance.SaveData.Money:N0} TL";
        }

        if (_dateLabel != null)
        {
            _dateLabel.Text = $"Gün: {save.CurrentDay}";
        }
    }
}

