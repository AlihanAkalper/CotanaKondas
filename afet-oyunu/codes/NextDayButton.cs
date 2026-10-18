using Godot;

public partial class NextDayButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnNextDayPressed;
	}
    private void OnNextDayPressed()
    {
        // Gün döngüsünü başlatır, kiraları toplar ve afet kontrolünü çalıştırır
        GameManager.Instance.AdvanceToNextDay();
    }
}
