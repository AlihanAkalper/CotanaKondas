using Godot;
using System;
using System.Linq;

public partial class BuildMenu : Control
{
    private VBoxContainer _rows;
    private Ev_numaraları _region;
    private string _message = "";
    private Label _title;
    private Texture2D _rowTexture;

    private static AtlasTexture TabelaPart(Texture2D atlas, Rect2 region) =>
        new AtlasTexture { Atlas = atlas, Region = region, FilterClip = true };

    private static StyleBoxTexture TabelaStyle(Texture2D texture, float padding, float brightness = 1f)
    {
        var style = new StyleBoxTexture
        {
            Texture = texture,
            ModulateColor = new Color(brightness, brightness, brightness)
        };
        foreach (var side in new[] { Side.Left, Side.Top, Side.Right, Side.Bottom })
        {
            style.SetTextureMargin(side, 1);
            style.SetContentMargin(side, padding);
        }
        return style;
    }
    public override void _Ready()
    {
        foreach (Node child in GetChildren()) { RemoveChild(child); child.QueueFree(); }
        SetAnchorsAndOffsetsPreset(LayoutPreset.TopLeft);
        Size = GetViewportRect().Size;
        GetViewport().SizeChanged += ResizeToViewport;
        MouseFilter = MouseFilterEnum.Ignore;
        TextureFilter = TextureFilterEnum.Nearest;
        var tabela = GD.Load<Texture2D>("res://assets/tabela.png");
        _rowTexture = TabelaPart(tabela, new Rect2(84, 8, 25, 5));
        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", TabelaStyle(
            TabelaPart(tabela, new Rect2(17, 9, 31, 39)), 8));
        AddChild(panel);
        panel.SetAnchorsAndOffsetsPreset(LayoutPreset.Center);
        panel.OffsetLeft = -350;
        panel.OffsetTop = -285;
        panel.OffsetRight = 350;
        panel.OffsetBottom = 285;
        var margin = new MarginContainer();
        foreach (var side in new[] { "left", "right", "top", "bottom" })
            margin.AddThemeConstantOverride("margin_" + side, 12);
        panel.AddChild(margin);
        var content = new VBoxContainer();
        content.AddThemeConstantOverride("separation", 12);
        margin.AddChild(content);
        var header = new HBoxContainer();
        content.AddChild(header);
        _title = new Label { SizeFlagsHorizontal = SizeFlags.ExpandFill,
            AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _title.AddThemeColorOverride("font_color", new Color("392719"));
        _title.AddThemeFontSizeOverride("font_size", 22);
        header.AddChild(_title);
        var close = new Button
        {
            Icon = TabelaPart(tabela, new Rect2(44, 0, 9, 9)),
            ExpandIcon = true,
            CustomMinimumSize = new Vector2(36, 36),
            SizeFlagsVertical = SizeFlags.ShrinkCenter,
            TooltipText = "Kapat"
        };
        close.AddThemeConstantOverride("icon_max_width", 36);
        foreach (string state in new[] { "normal", "hover", "pressed" })
            close.AddThemeStyleboxOverride(state, new StyleBoxEmpty());
        close.Pressed += Hide;
        header.AddChild(close);
        var scroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        scroll.SizeFlagsVertical = SizeFlags.ExpandFill;
        content.AddChild(scroll);
        _rows = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rows.AddThemeConstantOverride("separation", 8);
        scroll.AddChild(_rows);
        Hide();
    }
    private void ResizeToViewport() => Size = GetViewportRect().Size;
    public override void _ExitTree() => GetViewport().SizeChanged -= ResizeToViewport;
    public void Open(Ev_numaraları region) { _region = region; _message = ""; Refresh(); Show(); }
    private void Label(string text)
    {
        var card = new PanelContainer();
        card.AddThemeStyleboxOverride("panel", TabelaStyle(_rowTexture, 10));
        var label = new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        label.AddThemeColorOverride("font_color", new Color("fff1d5"));
        card.AddChild(label);
        _rows.AddChild(card);
    }
    private void Button(string text, Action action, bool disabled = false, string iconPath = null)
    {
        var button = new Button { Text = text, Disabled = disabled };
        button.AddThemeStyleboxOverride("normal", TabelaStyle(_rowTexture, 12));
        button.AddThemeStyleboxOverride("hover", TabelaStyle(_rowTexture, 12, 1.25f));
        button.AddThemeStyleboxOverride("pressed", TabelaStyle(_rowTexture, 12, .8f));
        button.AddThemeStyleboxOverride("disabled", TabelaStyle(_rowTexture, 12, .65f));
        foreach (string state in new[] { "font_color", "font_hover_color", "font_pressed_color" })
            button.AddThemeColorOverride(state, new Color("fff1d5"));
        button.AddThemeColorOverride("font_disabled_color", new Color("b8a68d"));
        if (iconPath != null)
        {
            button.Icon = GD.Load<Texture2D>(iconPath);
            button.ExpandIcon = true;
            button.IconAlignment = HorizontalAlignment.Left;
            button.CustomMinimumSize = new Vector2(0, 80);
            button.TextureFilter = TextureFilterEnum.Nearest;
            button.AddThemeConstantOverride("icon_max_width", 64);
            button.AddThemeConstantOverride("h_separation", 16);
        }
        button.Pressed += action;
        _rows.AddChild(button);
    }
    private void Buy(string action, Materyal material = Materyal.Alınmadı, int floor = -1)
    {
        var game = GameManager.Instance;
        var h = game.SaveData.Houses.Find(x => x.Type == _region);
        bool ok = ConstructionRules.Purchase(game.SaveData, h, action, material, floor);
        _message = ok ? "" : "İşlem yapılamadı. Bakiye ve bina durumunu kontrol edin.";
        if (ok)
        {
            game.SaveGame();
            HUD.Instance?.UpdateHUD();
            (GetTree().CurrentScene as BuildingMap)?.RefreshScene();
        }
        Refresh();
    }
    private void Refresh()
    {
        foreach (Node child in _rows.GetChildren()) { _rows.RemoveChild(child); child.QueueFree(); }
        var game = GameManager.Instance;
        var h = game.SaveData.Houses.Find(x => x.Type == _region);
        _title.Text = RegionDatabase.Regions[_region].Name + " — Bina ve Malzeme";
        if (h == null || !h.IsBuilt) { Label("Önce bu arsayı satın alın."); return; }
        Label($"Bakiye: {game.SaveData.Money:N0} TL");
        if (h.material == Materyal.Alınmadı)
        {
            Label("Bu arsa için tek malzeme seçin. Seçim sonradan değiştirilemez.");
            foreach (var material in new[] { Materyal.Odun, Materyal.Tuğla, Materyal.Çelik })
            {
                int price = ConstructionRules.MaterialPrice(_region, material);
                string iconPath = material switch
                {
                    Materyal.Odun => "res://assets/wood.png",
                    Materyal.Tuğla => "res://assets/brick.png",
                    _ => "res://assets/steel.png"
                };
                Button($"{material} seç — {price:N0} TL", () => Buy("select", material), game.SaveData.Money < price, iconPath);
            }
        }
        else
        {
            Label($"{h.material} • Seviye {h.MaterialLevel}/{ConstructionRules.MaxMaterialLevel} • Satın alma sayısı: {h.MaterialLevel}");
            Label($"Dayanıklılık: ×{ConstructionRules.Durability(h):0.00} • Gelir: ×{ConstructionRules.IncomeBonus(h):0.00}\nGünlük gelir: {ConstructionRules.DailyIncome(h):N0} TL");
            if (h.MaterialLevel < ConstructionRules.MaxMaterialLevel)
            {
                int price = ConstructionRules.UpgradePrice(h);
                Button($"Seviye {h.MaterialLevel + 1} — {price:N0} TL", () => Buy("upgrade"), game.SaveData.Money < price);
            }
            else Label("En yüksek malzeme seviyesine ulaşıldı.");
            Label($"Katlar: {h.Floors.Count}/{ConstructionRules.MaxFloors} • Açılan kat hakkı: {Math.Min(h.MaterialLevel, ConstructionRules.MaxFloors)}");
            if (h.Floors.Any(f => f.Health <= 0))
                Label("Yıkılan katları aşağıdan yukarıya yeniden inşa edin. Her kat için yeni kat ücreti ödenir. Alt katlar yapılmadan üst kat yapılamaz veya yeni kat eklenemez.");
            if (h.Floors.Count < ConstructionRules.MaxFloors)
            {
                int price = ConstructionRules.FloorPrice(h);
                Button($"{h.Floors.Count + 1}. katı inşa et — {price:N0} TL", () => Buy("floor"),
                    h.Floors.Any(f => f.Health <= 0) || h.Floors.Count >= h.MaterialLevel || game.SaveData.Money < price);
            }
            for (int i = 0; i < h.Floors.Count; i++)
            {
                int floor = i;
                bool destroyed = h.Floors[i].Health <= 0;
                int price = ConstructionRules.RepairPrice(h, i);
                string status = h.Floors[i].Health <= 0 ? "Kullanılamaz" :
                    !h.Floors[i].HasTenant ? "Boş — kiracı bekleniyor" : $"Kiracı farkındalığı: {h.Floors[i].Awareness}/100";
                string actionLabel = destroyed ? "Yeniden inşa et" : "Onar";
                Button($"{i + 1}. kat: {h.Floors[i].Health:0.#}/100 • {status} • {actionLabel}: {price:N0} TL",
                    () => Buy("repair", floor: floor), h.Floors.Take(i).Any(f => f.Health <= 0) || price <= 0 || game.SaveData.Money < price);
            }
        }
        if (_message.Length > 0) Label(_message);
    }
}


