using Godot;

// Autoload olarak sahneler arasında aynı oynatıcı ve çalma konumu korunur.
public partial class BackgroundMusic : AudioStreamPlayer
{
    public override void _Ready()
    {
        var music = GD.Load<AudioStreamMP3>("res://assets/audio/oyun_muzigi.mp3");
        if (music == null) return;
        music.Loop = true;
        Stream = music;
        Bus = "Master";
        VolumeDb = -12f;
        Play();
    }
}
