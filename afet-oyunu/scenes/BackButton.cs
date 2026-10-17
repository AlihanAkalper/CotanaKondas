using Godot;

public partial class BackButton : TextureButton
{
    public override void _Pressed()
    {
        // Butona basıldığı an doğrudan sahneyi değiştirir
        GetTree().ChangeSceneToFile("res://scenes/türkiye_haritası.tscn");
    }
}