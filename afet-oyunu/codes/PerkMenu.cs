using Godot;
using System;

public partial class PerkMenu : Control
{
    [Export] private VBoxContainer _perkListContainer;
    [Export] private Button _closeButton;
    private Texture2D _rowTexture;

    private static AtlasTexture TabelaPart(Texture2D atlas, Rect2 region) =>
        new AtlasTexture { Atlas = atlas, Region = region, FilterClip = true };

    private static StyleBoxTexture TabelaStyle(Texture2D texture, float padding)
    {
        var style = new StyleBoxTexture { Texture = texture };
        foreach (var side in new[] { Side.Left, Side.Top, Side.Right, Side.Bottom })
        {
            style.SetTextureMargin(side, 1);
            style.SetContentMargin(side, padding);
        }
        return style;
    }

    public override void _Ready()
    {
        Visible = false;
        TextureFilter = TextureFilterEnum.Nearest;
        var tabela = GD.Load<Texture2D>("res://assets/tabela.png");
        AddThemeStyleboxOverride("panel", TabelaStyle(
            // Üst sağdaki çarpı y=8'e kadar tabelayla örtüşüyor.
            // Arka plana dahil edilirse esnetilir ve ikinci, bozuk bir çarpı oluşur.
            TabelaPart(tabela, new Rect2(17, 9, 31, 39)), 20));
        _rowTexture = TabelaPart(tabela, new Rect2(84, 8, 25, 5));
        _perkListContainer.AddThemeConstantOverride("separation", 10);
        var title = GetNode<Label>("MainBox/HBoxContainer/titleLabel");
        title.AddThemeColorOverride("font_color", new Color("392719"));
        title.AddThemeFontSizeOverride("font_size", 22);

        if (_closeButton != null)
        {
            _closeButton.Text = "";
            _closeButton.TooltipText = "Kapat";
            _closeButton.Icon = TabelaPart(tabela, new Rect2(44, 0, 9, 9));
            _closeButton.ExpandIcon = true;
            _closeButton.CustomMinimumSize = new Vector2(36, 36);
            _closeButton.AddThemeConstantOverride("icon_max_width", 36);
            _closeButton.SizeFlagsVertical = SizeFlags.ShrinkCenter;
            foreach (string state in new[] { "normal", "hover", "pressed" })
                _closeButton.AddThemeStyleboxOverride(state, new StyleBoxEmpty());
            _closeButton.Pressed += () => Visible = false;
        }
    }

    public void Open()
    {
        RefreshPerkList();
        Visible = true;
    }

    public void RefreshPerkList()
    {
        if (_perkListContainer == null) return;

        // Eski satırları temizle
        foreach (Node child in _perkListContainer.GetChildren())
        {
            _perkListContainer.RemoveChild(child);
            child.QueueFree();
        }

        var save = GameManager.Instance.SaveData;

        foreach (var (perkType, info) in PerkDatabase.Perks)
        {
            bool isPurchased = save.PurchasedPerks != null && save.PurchasedPerks.Contains(perkType);

            // 1. Dış Çerçeve Kartı
            var cardPanel = new PanelContainer();
            cardPanel.CustomMinimumSize = new Vector2(0, 88);
            cardPanel.AddThemeStyleboxOverride("panel", TabelaStyle(_rowTexture, 10));

            // 2. Yatay Satır Düzeni
            var rowHBox = new HBoxContainer();
            rowHBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            rowHBox.AddThemeConstantOverride("separation", 12);

            // 3. Perk İkonu (TextureRect)
            var iconRect = new TextureRect();
            iconRect.CustomMinimumSize = new Vector2(44, 44);
            iconRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            iconRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            iconRect.SizeFlagsVertical = SizeFlags.ShrinkCenter;

            if (ResourceLoader.Exists(info.IconPath))
            {
                iconRect.Texture = GD.Load<Texture2D>(info.IconPath);
            }

            // 4. Metin Bölümü (İsim ve Açıklama alt alta)
            var textVBox = new VBoxContainer();
            textVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            textVBox.Alignment = BoxContainer.AlignmentMode.Center;

            var nameLabel = new Label();
            nameLabel.Text = info.Name;
            nameLabel.AddThemeFontSizeOverride("font_size", 16);
            nameLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            nameLabel.AddThemeColorOverride("font_color", new Color("fff1d5"));

            var descLabel = new Label();
            descLabel.Text = info.Description + $" Kiracı farkındalığı +{TenantRules.PerkAwarenessBonus}.";
            descLabel.AddThemeFontSizeOverride("font_size", 12);
            descLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            descLabel.AddThemeColorOverride("font_color", new Color("e4d1b5"));

            textVBox.AddChild(nameLabel);
            textVBox.AddChild(descLabel);

            // 5. Fiyat Etiketi (Sağda)
            var priceLabel = new Label();
            priceLabel.Text = $"{info.Price:N0} TL";
            priceLabel.HorizontalAlignment = HorizontalAlignment.Right;
            priceLabel.VerticalAlignment = VerticalAlignment.Center;
            priceLabel.CustomMinimumSize = new Vector2(85, 0);
            priceLabel.AddThemeFontSizeOverride("font_size", 12);

            // 6. Satın Alma Butonu
            var buyBtn = new Button();
            buyBtn.CustomMinimumSize = new Vector2(80, 32);
            buyBtn.SizeFlagsVertical = SizeFlags.ShrinkCenter;

            if (isPurchased)
            {
                buyBtn.Text = "Alındı";
                buyBtn.Disabled = true;
            }
            else
            {
                buyBtn.Text = "Satın Al";
                if (save.Money < info.Price)
                {
                    buyBtn.Disabled = true;
                }

                buyBtn.Pressed += () => OnBuyPerkClicked(info);
            }

            // Elemanları yatay sırayla ekle
            rowHBox.AddChild(iconRect);
            rowHBox.AddChild(textVBox);
            rowHBox.AddChild(priceLabel);
            rowHBox.AddChild(buyBtn);

            cardPanel.AddChild(rowHBox);
            _perkListContainer.AddChild(cardPanel);
        }
    }

    private void OnBuyPerkClicked(PerkInfo info)
    {
        var save = GameManager.Instance.SaveData;
        if (save.IsFinished) return;

        save.PurchasedPerks ??= new();
        if (save.PurchasedPerks.Contains(info.Type)) return;

        if (save.Money < info.Price)
        {
            GD.Print("Yetersiz bakiye!");
            return;
        }

        save.Money -= info.Price;

        save.PurchasedPerks.Add(info.Type);
        TenantRules.ApplyPerk(save);

        GameManager.Instance.SaveGame();
        HUD.Instance?.UpdateHUD();
        (GetTree().CurrentScene as BuildingMap)?.RefreshScene();

        GD.Print($"{info.Name} satın alındı! Kalan Para: {save.Money:N0} TL");
        RefreshPerkList();
    }
}
