using Godot;

public partial class FloodEffect : Node
{
    [Export] private ColorRect _waterRect; // Mavi su katmanı[cite: 14]

    public void TriggerFlood(float riseHeight = 350f, float duration = 4.0f)
    {
        if (_waterRect == null) return;

        Vector2 startPos = _waterRect.Position;
        Vector2 floodedPos = new Vector2(startPos.X, startPos.Y - riseHeight);

        Tween tween = CreateTween();

        // 1. Aşama: Su aşağıdan yukarı doğru yükselsin[cite: 14]
        tween.TweenProperty(_waterRect, "position", floodedPos, duration * 0.4f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);

        // 2. Aşama: Su tepe noktasında 1 saniye beklesin
        tween.TweenInterval(1.0f);

        // 3. Aşama: Su yavaşça geri çekilsin
        tween.TweenProperty(_waterRect, "position", startPos, duration * 0.4f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.In);
    }
}