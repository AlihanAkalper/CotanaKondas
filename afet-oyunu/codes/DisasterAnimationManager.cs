// Scripts/DisasterAnimationManager.cs
using Godot;
using System;

public partial class DisasterAnimationManager : Node
{
    [Export] private Camera2D _camera;
    [Export] private CpuParticles2D _fireParticles; // Varsa alev efekti
    [Export] private ColorRect _waterRect;         // Varsa su efekti

    public void PlayDisasterAnimation(AfetTürü afet)
    {
        GD.Print($"[ANİMASYON] {afet} görsel efekti oynatılıyor!");

        switch (afet)
        {
            case AfetTürü.Deprem:
                StartCameraShake(2.5f, 20.0f);
                break;

            case AfetTürü.Yangın:
                if (_fireParticles != null)
                {
                    _fireParticles.Emitting = true;
                    GetTree().CreateTimer(3.0f).Timeout += () => _fireParticles.Emitting = false;
                }
                break;

            case AfetTürü.Sel:
                if (_waterRect != null)
                {
                    StartFloodAnimation();
                }
                break;
        }
    }

    private void StartCameraShake(float duration, float intensity)
    {
        if (_camera == null)
        {
            GD.PrintErr("HATA: DisasterAnimationManager içinde Camera2D atanmamış!");
            return;
        }

        Vector2 originalOffset = _camera.Offset;
        Tween tween = CreateTween();
        int steps = (int)(duration * 25);
        var rand = new Random();

        for (int i = 0; i < steps; i++)
        {
            float x = ((float)rand.NextDouble() * 2f - 1f) * intensity;
            float y = ((float)rand.NextDouble() * 2f - 1f) * intensity;
            intensity = Mathf.Max(0f, intensity - (intensity / steps));

            tween.TweenProperty(_camera, "offset", originalOffset + new Vector2(x, y), duration / steps);
        }

        tween.TweenProperty(_camera, "offset", originalOffset, 0.05f);
    }

    private void StartFloodAnimation()
    {
        Vector2 startPos = _waterRect.Position;
        Vector2 floodedPos = new Vector2(startPos.X, startPos.Y - 250f);

        Tween tween = CreateTween();
        tween.TweenProperty(_waterRect, "position", floodedPos, 1.5f).SetTrans(Tween.TransitionType.Sine);
        tween.TweenInterval(1.0f);
        tween.TweenProperty(_waterRect, "position", startPos, 1.5f).SetTrans(Tween.TransitionType.Sine);
    }
}