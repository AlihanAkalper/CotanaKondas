using Godot;
using System.IO;

public partial class MainMenu : Control
{
    [ExportGroup("Ana Menü Butonları")]
    [Export] private Button _continueButton;
    [Export] private Button _newGameButton;
    [Export] private Button _settingsButton;
    [Export] private Button _exitButton;

    [ExportGroup("Ayarlar Paneli")]
    [Export] private PanelContainer _settingsPanel;
    [Export] private HSlider _volumeSlider;
    [Export] private Button _closeSettingsButton;

    private string _savePath;
    private int _masterBusIndex;

    public override void _Ready()
    {
        // Kayıt dosyasının yolu
        _savePath = ProjectSettings.GlobalizePath("user://").PathJoin("savegame.json");

        // Godot ses kanalı indeksi (0 genelde Master kanaldır)
        _masterBusIndex = AudioServer.GetBusIndex("Master");

        // Kayıt dosyası yoksa Continue butonunu pasif yap
        if (_continueButton != null)
        {
            bool hasSaveFile = File.Exists(_savePath);
            _continueButton.Disabled = !hasSaveFile;
            _continueButton.Pressed += OnContinuePressed;
        }

        if (_newGameButton != null)
            _newGameButton.Pressed += OnNewGamePressed;

        if (_settingsButton != null)
            _settingsButton.Pressed += () => _settingsPanel.Visible = true;

        if (_exitButton != null)
            _exitButton.Pressed += OnExitPressed;

        // Ayarlar paneli bağlantıları
        if (_settingsPanel != null)
            _settingsPanel.Visible = false;

        if (_closeSettingsButton != null)
            _closeSettingsButton.Pressed += () => _settingsPanel.Visible = false;

        if (_volumeSlider != null)
        {
            _volumeSlider.MinValue = 0.0f;
            _volumeSlider.MaxValue = 1.0f;
            _volumeSlider.Step = 0.05f;

            // Mevcut ses seviyesini çubuğa aktar
            float currentDb = AudioServer.GetBusVolumeDb(_masterBusIndex);
            _volumeSlider.Value = Mathf.DbToLinear(currentDb);

            _volumeSlider.ValueChanged += OnVolumeChanged;
        }
    }

    private void OnContinuePressed()
    {
        // Kayıtlı veriyi yükle ve Türkiye haritasına geç
        if (GameManager.Instance.LoadGame())
        {
            GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
        }
        else
        {
            GD.PrintErr("Kayıt yüklenemedi!");
        }
    }

    private void OnNewGamePressed()
    {
        // Varsa eski kayıt dosyasını sil
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }

        // GameManager'da tertemiz bir veri başlat
        GameManager.Instance.SaveData = new GameSaveData();
        GameManager.Instance.SaveGame();

        // Türkiye haritasına başla
        GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }

    private void OnVolumeChanged(double value)
    {
        // 0.0 - 1.0 arasındaki lineer değeri desibele (dB) çevir
        float volumeLinear = (float)value;
        float volumeDb = Mathf.LinearToDb(volumeLinear);

        // Çubuk tamamen sıfıra çekilirse sesi kapat (Mute)
        if (volumeLinear <= 0.01f)
        {
            AudioServer.SetBusMute(_masterBusIndex, true);
        }
        else
        {
            AudioServer.SetBusMute(_masterBusIndex, false);
            AudioServer.SetBusVolumeDb(_masterBusIndex, volumeDb);
        }
    }

    private void OnExitPressed()
    {
        // Oyundan çık
        GetTree().Quit();
    }
}