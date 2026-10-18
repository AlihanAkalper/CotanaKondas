using Godot;

public partial class DisasterResultPanel : CanvasLayer
{
    private Control _root;
    private PanelContainer _panel;
    private Label _title;

    public void Begin(DisasterEvent report, int remaining)
    {
        Layer = 20;
        _root = new Control { MouseFilter = Control.MouseFilterEnum.Stop };
        AddChild(_root);
        _root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        _title = new Label
        {
            Text = $"{RegionDatabase.Regions[report.Region].Name} — {report.DisasterType}",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _root.AddChild(_title);
        _title.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.TopWide);
        _title.OffsetTop = 70;
        _title.AddThemeFontSizeOverride("font_size", 26);

        _panel = new PanelContainer { Visible = false };
        _root.AddChild(_panel);
        _panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
        _panel.OffsetLeft = -340; _panel.OffsetRight = 340;
        _panel.OffsetTop = -240; _panel.OffsetBottom = 280;
        var background = new StyleBoxFlat { BgColor = new Color(.08f, .10f, .14f, .98f) };
        background.ContentMarginLeft = background.ContentMarginRight = 18;
        background.ContentMarginTop = background.ContentMarginBottom = 14;
        _panel.AddThemeStyleboxOverride("panel", background);
        var column = new VBoxContainer();
        column.AddThemeConstantOverride("separation", 10);
        _panel.AddChild(column);
        column.AddChild(new Label { Text = $"{RegionDatabase.Regions[report.Region].Name} — {report.DisasterType} | Gün {report.Day} | Bekleyen: {remaining}" });
        var scroll = new ScrollContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        column.AddChild(scroll);
        var rows = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        rows.AddThemeConstantOverride("separation", 8);
        scroll.AddChild(rows);
        void Line(string text) => rows.AddChild(new Label
        { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart });
        foreach (var floor in report.Floors)
        {
            string status =
                floor.RemainingHealth <= 0 ? "Yıkıldı — yeniden inşa gerekiyor" : "Kullanılabilir";
            Line($"{floor.FloorNumber}. kat: {floor.BeforeHealth:0.#} → {floor.RemainingHealth:0.#}/100   (−{floor.DamageTaken:0.#})\n{status}");
        }
        Line($"Malzeme dayanıklılığı: ×{report.DurabilityMultiplier:0.00}");
        Line($"Hazırlık koruması: %{report.ProtectionPercent:0.#}");
        Line(report.AppliedPerks.Count > 0 ? "Etkili hazırlıklar: " + string.Join(", ", report.AppliedPerks) : "Bu afete karşı satın alınmış hazırlık yok.");
        Line($"Günlük gelir: {report.IncomeBefore:N0} → {report.IncomeAfter:N0} TL");
        if (report.DepartedTenants > 0)
            Line($"Kat yıkımı veya farkındalık kaybı nedeniyle {report.DepartedTenants} kiracı ayrıldı. Boş katlardan kira alınmıyor.");
        Line("Onarım için şehirdeki Bina / Malzeme menüsünü kullanabilirsiniz.");
        var next = new Button { Text = remaining > 1 ? "Sonraki afeti göster" :
            GameManager.Instance.SaveData.IsFinished ? "Sonuçları göster" : "Tamam — Haritaya dön" };
        next.Pressed += () => { next.Disabled = true; GameManager.Instance.AcknowledgeDisaster(); };
        column.AddChild(next);
        var menu = new Button { Text = "Kaydet ve ana menüye dön" };
        menu.Pressed += () =>
        {
            GameManager.Instance.SaveGame();
            GetTree().ChangeSceneToFile("res://scenes/GirişEkranı.tscn");
        };
        column.AddChild(menu);
    }

    public void ShowResults()
    {
        if (!IsInsideTree()) return;
        _title.Hide();
        _panel.Show();
    }
}

