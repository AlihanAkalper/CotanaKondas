using Godot;
using System;

public partial class PerkMenu : Control
{
    [Export] private VBoxContainer _perkListContainer;
    [Export] private Button _closeButton;

    public override void _Ready()
    {
        Visible = false;

        if (_closeButton != null)
        {
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
            cardPanel.CustomMinimumSize = new Vector2(0, 60);

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
            nameLabel.AddThemeFontSizeOverride("font_size", 13);

            var descLabel = new Label();
            descLabel.Text = info.Description;
            descLabel.AddThemeFontSizeOverride("font_size", 10);
            descLabel.Modulate = new Color(0.85f, 0.85f, 0.85f);

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

        save.PurchasedPerks ??= new();
        if (save.PurchasedPerks.Contains(info.Type)) return;

        if (save.Money < info.Price)
        {
            GD.Print("Yetersiz bakiye!");
            return;
        }

        save.Money -= info.Price;

        if (!save.PurchasedPerks.Contains(info.Type))
        {
            save.PurchasedPerks.Add(info.Type);
        }

        GameManager.Instance.SaveGame();
        HUD.Instance?.UpdateHUD();

        GD.Print($"{info.Name} satın alındı! Kalan Para: {save.Money:N0} TL");
        RefreshPerkList();
    }
}
