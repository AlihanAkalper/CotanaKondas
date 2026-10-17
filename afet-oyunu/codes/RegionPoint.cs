using Godot;

public partial class RegionPoint : TextureButton
{
    [Export] public Ev_numaraları RegionType;
    [Export] private StatPanel _statPanel;

    [ExportGroup("Durum Dokuları")]
    [Export] private Texture2D _forSaleTexture;
    [Export] private Texture2D _purchasedTexture;

    public override void _Ready()
    {
        if (_statPanel == null)
            _statPanel = GetTree().Root.FindChild("StatPanel", true, false) as StatPanel;

        MouseEntered += OnHoverEnter;
        MouseExited += OnHoverExit;
        Pressed += OnClicked;

        UpdatePointTexture();
    }

    public void UpdatePointTexture()
    {
        if (GameManager.Instance?.SaveData == null) return;

        var houseData = GameManager.Instance.SaveData.Houses.Find(h => h.Type == RegionType);

        if (houseData != null && houseData.IsBuilt)
        {
            if (_purchasedTexture != null) TextureNormal = _purchasedTexture;
        }
        else
        {
            if (_forSaleTexture != null) TextureNormal = _forSaleTexture;
        }
    }

    private void OnHoverEnter() => _statPanel?.ShowStats(RegionType);
    private void OnHoverExit() => _statPanel?.HideStats();

    private void OnClicked()
    {
        GameManager.Instance.SelectedRegion = RegionType;
        var houseData = GameManager.Instance.SaveData.Houses.Find(h => h.Type == RegionType);

        if (houseData != null && houseData.IsBuilt)
        {
            // ARSA ZATEN ALINMIŞ: Şehir/Ev haritasına git
            GetTree().ChangeSceneToFile("res://scenes/şehir.tscn");
        }
        else
        {
            // ARSA HENÜZ ALINMAMIŞ (SALE): Türkiye haritasında satın alma onay kutusunu aç
            var confirmDialog = GetTree().Root.FindChild("ConfirmDialog", true, false) as PlotConfirmDialog;
            confirmDialog?.Open(RegionType);
        }
    }
}