using Godot;
using System;

public partial class DisasterAnimationManager : Node
{
    [Export] private Camera2D _camera;
    [Export] private CpuParticles2D _fireParticles;
    [Export] private ColorRect _waterRect;
    public event Action Completed;
    private Tween _animation;
    private Vector2 _cameraOffset;

    public void PlayDisasterAnimation(AfetTürü disaster)
    {
        _animation?.Kill();
        _cameraOffset = _camera?.Offset ?? Vector2.Zero;
        _animation = CreateTween();
        switch (disaster)
        {
            case AfetTürü.Deprem:
                for (int i = 0; i < 60; i++)
                {
                    float strength = 18f * (1f - i / 60f);
                    var offset = _cameraOffset + new Vector2((float)GD.RandRange(-strength, strength), (float)GD.RandRange(-strength, strength));
                    if (_camera != null) _animation.TweenProperty(_camera, "offset", offset, .04);
                }
                if (_camera != null) _animation.TweenProperty(_camera, "offset", _cameraOffset, .1);
                break;
            case AfetTürü.Yangın:
                if (_fireParticles != null)
                {
                    var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == GameManager.Instance.SelectedRegion);
                    float height = Math.Max(1, house?.Floors.Count ?? 1) * 120f;
                    _fireParticles.Position = new Vector2(583, 540 - height / 2);
                    _fireParticles.EmissionRectExtents = new Vector2(185, height / 2);
                    _fireParticles.ColorRamp = new Gradient
                    {
                        Offsets = new float[] { 0, .25f, .65f, 1 },
                        Colors = new Color[] { new(1, .85f, .15f, .9f), new(1, .35f, .04f, .85f), new(.35f, .12f, .06f, .6f), new(.15f, .15f, .15f, 0) }
                    };
                    _fireParticles.Show();
                    _fireParticles.Emitting = true;
                    _fireParticles.Restart();
                }
                _animation.TweenInterval(3);
                _animation.TweenCallback(Callable.From(() => { if (_fireParticles != null) _fireParticles.Emitting = false; }));
                _animation.TweenInterval(1.3);
                break;
            case AfetTürü.Sel:
                if (_waterRect != null)
                {
                    _waterRect.Position = new Vector2(-100, 700);
                    _waterRect.Show();
                    _animation.TweenProperty(_waterRect, "position:y", 390f, 1.5).SetTrans(Tween.TransitionType.Sine);
                    _animation.TweenInterval(1);
                    _animation.TweenProperty(_waterRect, "position:y", 700f, 1.5).SetTrans(Tween.TransitionType.Sine);
                }
                break;
        }
        _animation.TweenCallback(Callable.From(() =>
        {
            if (_fireParticles != null) { _fireParticles.Emitting = false; _fireParticles.Hide(); }
            _waterRect?.Hide();
            Completed?.Invoke();
        }));
    }

    public override void _ExitTree()
    {
        _animation?.Kill();
        if (GodotObject.IsInstanceValid(_camera)) _camera.Offset = _cameraOffset;
    }
}
