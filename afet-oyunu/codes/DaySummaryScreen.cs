using Godot;
using System.Collections.Generic;

public class DaySummaryData
{
    public int Day { get; set; }
    public int BalanceBefore { get; set; }
    public int RentIncome { get; set; }
    public int BalanceAfter { get; set; }
    public int NewTenants { get; set; }
    public int LostTenants { get; set; }
    public int OccupiedFloors { get; set; }
    public int EmptyFloors { get; set; }
    public int WaitingQuestions { get; set; }
    public int DisasterCount { get; set; }
    public List<string> RegionIncome { get; set; } = new();
}

public partial class DaySummaryScreen : Control
{
    public override void _Ready()
    {
        var game = GameManager.Instance;
        var report = game.SaveData.PendingDaySummary;
        if (report == null) { game.OpenCurrentGame(); return; }

        var background = new ColorRect { Color = new Color("101923") };
        AddChild(background);
        background.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        var panel = new PanelContainer();
        AddChild(panel);
        panel.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        panel.AnchorLeft = .18f; panel.AnchorRight = .82f;
        panel.AnchorTop = .06f; panel.AnchorBottom = .94f;
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat {
            BgColor = new Color("1c2b36"),
            ContentMarginLeft = 24, ContentMarginRight = 24,
            ContentMarginTop = 20, ContentMarginBottom = 20
        });
        var column = new VBoxContainer();
        column.AddThemeConstantOverride("separation", 14);
        panel.AddChild(column);
        var title = new Label { Text = $"GÜN {report.Day} — GÜNLÜK ÖZET",
            HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        column.AddChild(title);
        var scroll = new ScrollContainer {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };
        column.AddChild(scroll);
        var rows = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        rows.AddThemeConstantOverride("separation", 12);
        scroll.AddChild(rows);
        void Line(string text, bool highlight = false)
        {
            var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
            label.AddThemeFontSizeOverride("font_size", highlight ? 24 : 18);
            if (highlight) label.AddThemeColorOverride("font_color", new Color("a8e4ac"));
            rows.AddChild(label);
        }
        Line($"Toplanan kira: +{report.RentIncome:N0} TL", true);
        Line($"Önceki bakiye: {report.BalanceBefore:N0} TL\nGüncel bakiye: {report.BalanceAfter:N0} TL");
        if (report.RegionIncome.Count > 0)
            Line("Bölgelere göre kira\n" + string.Join("\n", report.RegionIncome));
        else Line("Henüz kira getiren bir arsanız yok. Haritadan arsa alıp kat inşa edebilirsiniz.");
        Line($"Yeni taşınan kiracı: {report.NewTenants}\nBugün ayrılan kiracı: {report.LostTenants}");
        Line($"Gün sonu kiracılı kat: {report.OccupiedFloors}\nKiracı bekleyen kullanılabilir kat: {report.EmptyFloors}");
        Line(report.DisasterCount > 0
            ? $"Bugün {report.DisasterCount} afet yaşandı. Devam ettiğinizde sırayla gösterilecek."
            : "Bugün afet yaşanmadı.");
        if (report.WaitingQuestions > 0)
            Line($"{report.WaitingQuestions} kiracı konuşmak istiyor. Şehirdeki soru işaretlerine tıklayabilirsiniz.");

        var actions = new HBoxContainer();
        actions.AddThemeConstantOverride("separation", 16);
        column.AddChild(actions);
        var next = new Button {
            Text = game.HasPendingDisasters ? "Afetleri göster" :
                game.SaveData.IsFinished ? "Oyun sonucunu göster" : "Haritaya dön",
            CustomMinimumSize = new Vector2(0, 48),
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ShrinkCenter
        };
        actions.AddChild(next);
        var advance = new TextureButton {
            TextureNormal = GD.Load<Texture2D>("res://assets/next.png"),
            IgnoreTextureSize = true,
            StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered,
            TextureFilter = TextureFilterEnum.Nearest,
            CustomMinimumSize = new Vector2(96, 96),
            Disabled = game.HasPendingDisasters || game.SaveData.IsFinished,
            TooltipText = game.HasPendingDisasters ? "Önce bekleyen afetleri görüntüleyin." :
                game.SaveData.IsFinished ? "Oyun tamamlandı." : "Sonraki güne geç"
        };
        if (advance.Disabled) advance.Modulate = new Color(1, 1, 1, .4f);
        actions.AddChild(advance);
        next.Pressed += () => {
            next.Disabled = advance.Disabled = true;
            game.AcknowledgeDaySummary();
        };
        advance.Pressed += () => {
            next.Disabled = advance.Disabled = true;
            game.AcknowledgeDaySummary(advanceDay: true);
        };
    }
}
