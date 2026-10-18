using Godot;

// Ortak oynatıcı sayesinde sahne değişse de tıklama sesi tamamlanır.
public partial class ButtonSounds : AudioStreamPlayer
{
    private Callable _playClick;

    public override void _Ready()
    {
        var click = GD.Load<AudioStreamMP3>("res://assets/audio/buton_tiklama.mp3");
        if (click == null) return;
        click.Loop = false;
        Stream = click;
        Bus = "Master";
        VolumeDb = -6f;
        MaxPolyphony = 4;
        ProcessMode = ProcessModeEnum.Always;
        _playClick = Callable.From(() => Play());
        GetTree().NodeAdded += ConnectButton;
        ConnectExistingButtons(GetTree().Root);
    }

    private void ConnectButton(Node node)
    {
        if (node is BaseButton button && !button.IsConnected(BaseButton.SignalName.Pressed, _playClick))
            button.Connect(BaseButton.SignalName.Pressed, _playClick);
    }

    private void ConnectExistingButtons(Node node)
    {
        ConnectButton(node);
        foreach (Node child in node.GetChildren()) ConnectExistingButtons(child);
    }

    public override void _ExitTree()
    {
        GetTree().NodeAdded -= ConnectButton;
    }
}
