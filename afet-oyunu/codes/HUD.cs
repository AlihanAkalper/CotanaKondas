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
            Title = "Oyun kuralları", Exclusive = true,
            DialogText =
                "Arsa al, malzeme seç ve kat inşa et. Çekiç inşaat menüsünü açar.\n" +
                "Dolu ve sağlam her kat, günlük arsa bedelinin %1,5'i kadar kira getirir.\n" +
                "Malzeme seviyesi yükseldikçe dayanıklılık ve kira artar.\n\n" +
                "Her kiracı 50–100 arası farkındalıkla gelir; farkındalık her gün 2 azalır.\n" +
                "50'nin altındaki uygun kiracılardan birine günlük %10 şansla soru atanır.\n" +
                "Konuşmak için soru işaretine tıkla. Pozitif +15, nötr +5, negatif −10 etkiler.\n" +
                "Upgrade menüsündeki her yeni hazırlık mevcut kiracılara +10 farkındalık verir.\n" +
                "Farkındalık sıfıra ulaşırsa kiracı kendi binasını etkileyen sonraki afette ayrılır.\n" +
                "Sonradan farkındalık artsa da ayrılma kararı değişmez.\n" +
                "Boş ve sağlam daireye her yeni gün %20 şansla kiracı gelir.\n\n" +
                "6 farklı kat veya 3 farklı bina yıkılırsa oyun biter. Gün sınırı yoktur.\n" +
                "Onarım geçmiş yıkım sayısını azaltmaz; aynı yapı tekrar sayılmaz."
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
            _lossLabel.Text = $"Yıkım: {GameEndRules.DestroyedFloors(save)}/{GameEndRules.FloorLimit} kat • {GameEndRules.DestroyedBuildings(save)}/{GameEndRules.BuildingLimit} bina";

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

