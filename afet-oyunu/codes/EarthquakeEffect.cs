using Godot;
using System;

public partial class EarthquakeEffect : Node
{

	[Export] Camera2D _camera;

	public void TriggerEarthquake(float duration = 2.0f, float intensity = 15.0f) 
	{
		if (_camera == null) return;

        Vector2 initialOffset = _camera.Offset;
        Tween tween = CreateTween();
        int shakes = (int)(duration * 20); // Saniyede 20 titreşim

        var rand = new Random();

        for (int i = 0; i < shakes; i++)
        {
            // Rastgele yönlerde ofset belirle
            float xOffset = ((float)rand.NextDouble() * 2f - 1f) * intensity;
            float yOffset = ((float)rand.NextDouble() * 2f - 1f) * intensity;
            Vector2 randomTarget = initialOffset + new Vector2(xOffset, yOffset);

            // Her adımda titreşimin şiddetini biraz azaltarak sönümle
            intensity = Mathf.Max(0f, intensity - (intensity / shakes));

            tween.TweenProperty(_camera, "offset", randomTarget, duration / shakes);
        }

        // Titreşim bitince kamerayı orijinal yerine döndür
        tween.TweenProperty(_camera, "offset", initialOffset, 0.05f);
    }
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
