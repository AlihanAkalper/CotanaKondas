using Godot;

public partial class TenantDialoguePanel : CanvasLayer
{
    public Ev_numaraları Region { get; set; }
    public int FloorIndex { get; set; }
    public override void _Ready()
    {
        Layer = 30;
        var game = GameManager.Instance;
        var house = game.SaveData.Houses.Find(h => h.Type == Region);
        if (house == null || FloorIndex < 0 || FloorIndex >= house.Floors.Count) { QueueFree(); return; }
        var tenant = house.Floors[FloorIndex];
        int id = tenant.PendingQuestionId;
        if (id < 0 || id >= TenantDialogues.All.Length) { QueueFree(); return; }
        var dialogue = TenantDialogues.All[id];
        var overlay = new ColorRect { Color = new Color(0, 0, 0, .65f), MouseFilter = Control.MouseFilterEnum.Stop };
        AddChild(overlay);
        overlay.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        var panel = new PanelContainer();
        overlay.AddChild(panel);
        panel.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        panel.AnchorLeft = .12f; panel.AnchorRight = .88f;
        panel.AnchorTop = .08f; panel.AnchorBottom = .92f;
        var style = new StyleBoxFlat { BgColor = new Color(.08f, .10f, .14f, 1) };
        style.ContentMarginLeft = style.ContentMarginRight = 18;
        style.ContentMarginTop = style.ContentMarginBottom = 16;
        panel.AddThemeStyleboxOverride("panel", style);
        var scroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        panel.AddChild(scroll);
        var column = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        column.AddThemeConstantOverride("separation", 12);
        scroll.AddChild(column);
        column.AddChild(new Label { Text = $"{RegionDatabase.Regions[Region].Name} • {FloorIndex + 1}. kattaki kiracı — {dialogue.Topic}\nKiracı farkındalığı: {tenant.Awareness}/100",
            AutowrapMode = TextServer.AutowrapMode.WordSmart });
        column.AddChild(new Label { Text = dialogue.Question, AutowrapMode = TextServer.AutowrapMode.WordSmart });
        column.AddChild(new Label { Text = "Yanıtını seç:" });
        // Her kategorideki iki alternatiften yalnızca biri gösterilir.
        int[] order = tenant.QuestionAnswers;
        foreach (int index in order)
        {
            var button = new Button { Text = dialogue.Answers[index],
                AutowrapMode = TextServer.AutowrapMode.WordSmart,
                Alignment = HorizontalAlignment.Left, CustomMinimumSize = new Vector2(0, 54) };
            column.AddChild(button);
            button.Pressed += () => {
                if (!TenantRules.Answer(game.SaveData, Region, FloorIndex, id, index)) return;
                game.SaveGame();
                (GetTree().CurrentScene as BuildingMap)?.RefreshScene();
                QueueFree();
            };
        }
        var close = new Button { Text = "Şimdi değil" };
        column.AddChild(close);
        close.Pressed += QueueFree;
        var menu = new Button { Text = "Kaydet ve ana menüye dön" };
        column.AddChild(menu);
        menu.Pressed += () => {
            game.SaveGame();
            GetTree().ChangeSceneToFile("res://scenes/GirişEkranı.tscn");
        };
    }
}
