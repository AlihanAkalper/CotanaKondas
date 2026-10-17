using Godot;
using System;

public partial class BuildMenu : Control
{
    [Export] private Label _titleLabel;
    [Export] private OptionButton _materialDropdown;
    [Export] private SpinBox _amountInput;
    [Export] private Label _materialCostLabel;
    [Export] private Label _statusMessageLabel;
    [Export] private Button _confirmButton;
    [Export] private Button _cancelButton;

    private Ev_numaraları _currentRegion;

    public override void _Ready()
    {
        // Inspector bağlantıları henüz yapılmadıysa hata vermesini engelle
        if (_materialDropdown == null || _confirmButton == null || _cancelButton == null)
            return;

        _materialDropdown.Clear();
        _materialDropdown.AddItem("Odun (100 TL)", (int)Materyal.Odun);
        _materialDropdown.AddItem("Tuğla (250 TL)", (int)Materyal.Tuğla);
        _materialDropdown.AddItem("Çelik (600 TL)", (int)Materyal.Çelik);

        _materialDropdown.ItemSelected += _ => UpdateCostDisplay();
        _amountInput.ValueChanged += _ => UpdateCostDisplay();

        _confirmButton.Pressed += OnConfirmPressed;
        _cancelButton.Pressed += () => Visible = false;
    }

    public void Open(Ev_numaraları region)
    {
        _currentRegion = region;
        var info = RegionDatabase.Regions[region];
        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == region);

        if (_titleLabel != null)
            _titleLabel.Text = $"{info.Name} - İnşaat Paneli";

        if (_statusMessageLabel != null)
            _statusMessageLabel.Text = "";

        if (_amountInput != null)
        {
            _amountInput.MinValue = 1;
            _amountInput.MaxValue = 500;
            _amountInput.Value = 10;
        }

        UpdateCostDisplay();
        Visible = true;
    }

    private void UpdateCostDisplay()
    {
        if (_materialDropdown == null || _amountInput == null || _materialCostLabel == null)
            return;

        Materyal selectedMat = (Materyal)_materialDropdown.GetSelectedId();
        int amount = (int)_amountInput.Value;

        if (!MaterialPrices.UnitPrices.ContainsKey(selectedMat)) return;

        int unitPrice = MaterialPrices.UnitPrices[selectedMat];
        int totalCost = unitPrice * amount;

        _materialCostLabel.Text = $"Ödenecek Tutar: {totalCost:N0} TL ({amount} adet)";
    }

    private void OnConfirmPressed()
    {
        Materyal selectedMat = (Materyal)_materialDropdown.GetSelectedId();
        int amount = (int)_amountInput.Value;

        int unitPrice = MaterialPrices.UnitPrices[selectedMat];
        int totalCost = unitPrice * amount;

        // Bakiye kontrolü
        if (GameManager.Instance.SaveData.Money < totalCost)
        {
            if (_statusMessageLabel != null)
                _statusMessageLabel.Text = "Yetersiz bakiye!";
            return;
        }

        var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == _currentRegion);
        if (house == null) return;

        // Parayı düş ve eve malzemeyi ata
        GameManager.Instance.SaveData.Money -= totalCost;
        house.material = selectedMat;
        house.MaterialAmount += amount;
        house.Health = 100f; // Ev tamamlandı/yenilendi

        GameManager.Instance.SaveGame();
        HUD.Instance?.UpdateHUD();

        if (_statusMessageLabel != null)
            _statusMessageLabel.Text = "İnşaat tamamlandı!";

        // Şehir sahnesini yenile (boş arsa yerine evin görseli gelsin)
        GetTree().CreateTimer(0.8f).Timeout += () =>
        {
            Visible = false;
            var cityScene = GetTree().CurrentScene as BuildingMap;
            cityScene?.RefreshScene();
        };
    }
}