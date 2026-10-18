using Godot;

public partial class BackButton : TextureButton
{
    public override void _Pressed()
    {
        // Butona basıldığı an doğrudan sahneyi değiştirir
        if (GameManager.Instance.HasPendingDisasters) return;
        GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }
}
