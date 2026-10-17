using Godot;

public partial class BuildMenu : Control
{
    [Export] private Label _titleLabel;
    [Export] private Label _plotCostLabel;
    [Export] private Label _materialCostLabel;
    [Export] private Label _totalCostLabel;
    [Export] private OptionButton _materialDropdown;
    [Export] private SpinBox _amountInput;
    [Export] private Label _statusMessageLabel;
    [Export] private Button _confirmButton;
    [Export] private Button _cancelButton;

    private Ev_numaraları _currentRegion;

    public override void _Ready()
    {
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

        _titleLabel.Text = $"{info.Name} Arsa & İnşaat";
        _plotCostLabel.Text = $"Arsa Bedeli: {info.BasePlotPrice:N0} TL";
        _statusMessageLabel.Text = "";

        _amountInput.MinValue = 10;
        _amountInput.MaxValue = 500;
        _amountInput.Value = 50;

        UpdateCostDisplay();
        Visible = true;
    }

    private void UpdateCostDisplay()
    {
        var info = RegionDatabase.Regions[_currentRegion];
        Materyal selectedMat = (Materyal)_materialDropdown.GetSelectedId();
        int amount = (int)_amountInput.Value;

        int unitPrice = MaterialPrices.UnitPrices[selectedMat];
        int materialTotal = unitPrice * amount;
        int grandTotal = info.BasePlotPrice + materialTotal;

        _materialCostLabel.Text = $"Malzeme Maliyeti: {materialTotal:N0} TL ({amount} adet)";
        _totalCostLabel.Text = $"Toplam Ödenecek: {grandTotal:N0} TL";
    }

    private void OnConfirmPressed()
    {
        Materyal selectedMat = (Materyal)_materialDropdown.GetSelectedId();
        int amount = (int)_amountInput.Value;

        bool success = GameManager.Instance.BuyAndBuildHouse(_currentRegion, selectedMat, amount);

        if (success)
        {
            _statusMessageLabel.Text = "Satın alma ve inşaat tamamlandı!";
            GetTree().CreateTimer(0.8f).Timeout += () =>
            {
                Visible = false;

                // Türkiye haritasındaki butonların ikonlarını yenile
                var points = GetTree().GetNodesInGroup("RegionButtons");
                foreach (var node in points)
                {
                    if (node is RegionPoint rp) rp.UpdatePointTexture();
                }
            };
        }
        else
        {
            _statusMessageLabel.Text = "Yetersiz bakiye!";
        }
    }
}