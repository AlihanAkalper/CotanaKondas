using Godot;
using System;

public partial class BuildMenu : Control
{
    private VBoxContainer _rows;
    private Ev_numaraları _region;
    private string _message = "";
    public override void _Ready()
    {
        foreach (Node child in GetChildren()) { RemoveChild(child); child.QueueFree(); }
        SetAnchorsAndOffsetsPreset(LayoutPreset.TopLeft);
        Size = GetViewportRect().Size;
        GetViewport().SizeChanged += ResizeToViewport;
        MouseFilter = MouseFilterEnum.Ignore;
        var panel = new PanelContainer();
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
        var scroll = new ScrollContainer { HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        margin.AddChild(scroll);
        _rows = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rows.AddThemeConstantOverride("separation", 8);
        scroll.AddChild(_rows);
        Hide();
    }
    private void ResizeToViewport() => Size = GetViewportRect().Size;
    public override void _ExitTree() => GetViewport().SizeChanged -= ResizeToViewport;
    public void Open(Ev_numaraları region) { _region = region; _message = ""; Refresh(); Show(); }
    private void Label(string text) => _rows.AddChild(new Label { Text = text, AutowrapMode = TextServer.AutowrapMode.WordSmart });
    private void Button(string text, Action action, bool disabled = false)
    {
        var button = new Button { Text = text, Disabled = disabled };
        button.Pressed += action;
        _rows.AddChild(button);
    }
    private void Buy(string action, Materyal material = Materyal.Alınmadı, int floor = -1)
    {
        var game = GameManager.Instance;
        var h = game.SaveData.Houses.Find(x => x.Type == _region);
        bool ok = ConstructionRules.Purchase(game.SaveData, h, action, material, floor);
        _message = ok ? "İşlem tamamlandı." : "İşlem yapılamadı. Bakiye ve bina durumunu kontrol edin.";
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
        Label(RegionDatabase.Regions[_region].Name + " — Bina ve Malzeme");
        Button("Kapat", Hide);
        if (h == null || !h.IsBuilt) { Label("Önce bu arsayı satın alın."); return; }
        Label($"Bakiye: {game.SaveData.Money:N0} TL");
        if (h.material == Materyal.Alınmadı)
        {
            Label("Bu arsa için tek malzeme seçin. Seçim sonradan değiştirilemez.");
            foreach (var material in new[] { Materyal.Odun, Materyal.Tuğla, Materyal.Çelik })
            {
                int price = ConstructionRules.MaterialPrice(_region, material);
                Button($"{material} seç — {price:N0} TL", () => Buy("select", material), game.SaveData.Money < price);
            }
        }
        else
        {
            Label($"{h.material} • Seviye {h.MaterialLevel}/5 • Satın alma sayısı: {h.MaterialLevel}");
            Label($"Dayanıklılık: ×{ConstructionRules.Durability(h):0.00} • Gelir: ×{ConstructionRules.IncomeBonus(h):0.00}\nGünlük gelir: {ConstructionRules.DailyIncome(h):N0} TL");
            Label($"Bina: {h.Health:0.#}/300 can" + (h.Health <= 0 ? " — Kullanılamaz, onarım gerekiyor" : ""));
            if (h.MaterialLevel < ConstructionRules.MaxFloors)
            {
                int price = ConstructionRules.UpgradePrice(h);
                Button($"Seviye {h.MaterialLevel + 1} — {price:N0} TL", () => Buy("upgrade"), h.Health <= 0 || game.SaveData.Money < price);
            }
            else Label("En yüksek malzeme seviyesine ulaşıldı.");
            Label($"Katlar: {h.Floors.Count}/5 • Açılan kat hakkı: {h.MaterialLevel}");
            if (h.Floors.Count < ConstructionRules.MaxFloors)
            {
                int price = ConstructionRules.FloorPrice(h);
                Button($"{h.Floors.Count + 1}. katı inşa et — {price:N0} TL", () => Buy("floor"),
                    h.Health <= 0 || h.Floors.Count >= h.MaterialLevel || game.SaveData.Money < price);
            }
            int repair = ConstructionRules.RepairPrice(h);
            Button($"Binayı onar — {repair:N0} TL", () => Buy("repair"), repair <= 0 || game.SaveData.Money < repair);
            for (int i = 0; i < h.Floors.Count; i++)
            {
                int floor = i;
                int price = ConstructionRules.RepairPrice(h, i);
                string status = h.Health <= 0 || h.Floors[i].Health <= 0 ? "Kullanılamaz" : "Kullanılabilir";
                Button($"{i + 1}. kat: {h.Floors[i].Health:0.#}/100 • {status} • Onar: {price:N0} TL",
                    () => Buy("repair", floor: floor), price <= 0 || game.SaveData.Money < price);
            }
        }
        if (_message.Length > 0) Label(_message);
    }
}


