using Godot;
using System;

public partial class DisasterAnimationManager : Node
{
    [Export] private Camera2D _camera;
    public event Action Completed;
    private Tween _animation;
    private Vector2 _cameraOffset;
    private Node2D _spriteEffects;
    private AudioStreamPlayer _earthquakeAudio;
    private AudioStreamPlayer _fireAudio;
    private AudioStreamPlayer _floodAudio;

    public override void _Ready() => _cameraOffset = _camera?.Offset ?? Vector2.Zero;

    private static SpriteFrames LoadFrames(string path, int count, double speed, bool loop)
    {
        var texture = GD.Load<Texture2D>(path);
        var frames = new SpriteFrames();
        frames.SetAnimationSpeed("default", speed);
        frames.SetAnimationLoopMode("default", loop ? SpriteFrames.LoopMode.Linear : SpriteFrames.LoopMode.None);
        int width = texture.GetWidth() / count;
        for (int i = 0; i < count; i++)
            frames.AddFrame("default", new AtlasTexture
            {
                Atlas = texture,
                Region = new Rect2(i * width, 0, width, texture.GetHeight()),
                FilterClip = true
            });
        return frames;
    }

    private AnimatedSprite2D AddEffect(SpriteFrames frames, Vector2 position, Vector2 scale)
    {
        var sprite = new AnimatedSprite2D
        {
            SpriteFrames = frames, Position = position, Scale = scale,
            Centered = false, TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        _spriteEffects.AddChild(sprite);
        sprite.Play();
        return sprite;
    }

    private void ClearEffects()
    {
        if (GodotObject.IsInstanceValid(_floodAudio)) _floodAudio.Stop();
        if (GodotObject.IsInstanceValid(_earthquakeAudio)) _earthquakeAudio.Stop();
        if (GodotObject.IsInstanceValid(_fireAudio)) _fireAudio.Stop();
        if (GodotObject.IsInstanceValid(_spriteEffects))
        {
            _spriteEffects.Hide();
            _spriteEffects.QueueFree();
            _spriteEffects = null;
        }
        if (GodotObject.IsInstanceValid(_camera)) _camera.Offset = _cameraOffset;
    }

    public void PlayDisasterAnimation(AfetTürü disaster)
    {
        _animation?.Kill();
        ClearEffects();
        _spriteEffects = new Node2D { ZIndex = 6 };
        AddChild(_spriteEffects);
        _animation = CreateTween();
        switch (disaster)
        {
            case AfetTürü.Deprem:
                if (!GodotObject.IsInstanceValid(_earthquakeAudio))
                {
                    _earthquakeAudio = new AudioStreamPlayer
                    {
                        Name = "EarthquakeAudio",
                        Stream = GD.Load<AudioStream>("res://assets/audio/deprem.mp3"),
                        Bus = "Master"
                    };
                    AddChild(_earthquakeAudio);
                }
                _earthquakeAudio.Play();
                for (int i = 0; i < 60; i++)
                {
                    float strength = 18f * (1f - i / 60f);
                    var offset = _cameraOffset + new Vector2((float)GD.RandRange(-strength, strength), (float)GD.RandRange(-strength, strength));
                    if (_camera != null) _animation.TweenProperty(_camera, "offset", offset, .04);
                }
                if (_camera != null) _animation.TweenProperty(_camera, "offset", _cameraOffset, .1);
                break;
            case AfetTürü.Yangın:
                if (!GodotObject.IsInstanceValid(_fireAudio))
                {
                    _fireAudio = new AudioStreamPlayer
                    {
                        Name = "FireAudio",
                        Stream = GD.Load<AudioStream>("res://assets/audio/yangin.mp3"),
                        Bus = "Master"
                    };
                    AddChild(_fireAudio);
                }
                _fireAudio.Play();
                var house = GameManager.Instance.SaveData.Houses.Find(h => h.Type == GameManager.Instance.SelectedRegion);
                var fire = LoadFrames("res://assets/fire.png", 10, 12, true);
                var building = GetParent().GetNodeOrNull<Bina>("Bina");
                Vector2 origin = building?.Position ?? new Vector2(583, 556);
                float floorHeight = building?.KatHeight ?? 120;
                for (int floor = 0; floor < Math.Max(1, house?.Floors.Count ?? 1); floor++)
                    for (int column = 0; column < 5; column++)
                    {
                        var flame = AddEffect(fire,
                            origin + new Vector2(-210 + column * 80, -floorHeight * floor - 110),
                            new Vector2(1.5f, 1.5f));
                        flame.Frame = (floor * 3 + column * 2) % 10;
                    }
                _animation.TweenInterval(3);
                _animation.TweenProperty(_spriteEffects, "modulate:a", 0f, .5);
                break;
            case AfetTürü.Sel:
                if (!GodotObject.IsInstanceValid(_floodAudio))
                {
                    _floodAudio = new AudioStreamPlayer
                    {
                        Name = "FloodAudio",
                        Stream = GD.Load<AudioStream>("res://assets/audio/sel.mp3"),
                        Bus = "Master"
                    };
                    AddChild(_floodAudio);
                }
                _floodAudio.Play();
                var flood = LoadFrames("res://assets/sel.png", 12, 4, false);
                var background = GetParent().GetNodeOrNull<Sprite2D>("Sprite2D");
                Vector2 size = background != null ? background.Texture.GetSize() * background.Scale : new Vector2(1158, 665);
                Vector2 topLeft = background != null ? background.Position - size / 2 : Vector2.Zero;
                AddEffect(flood, topLeft, size / new Vector2(640, 360));
                _animation.TweenInterval(3);
                _animation.TweenProperty(_spriteEffects, "modulate:a", 0f, .4);
                break;
        }
        _animation.TweenCallback(Callable.From(() =>
        {
            ClearEffects();
            Completed?.Invoke();
        }));
    }

    public override void _ExitTree()
    {
        _animation?.Kill();
        if (GodotObject.IsInstanceValid(_floodAudio)) _floodAudio.Stop();
        if (GodotObject.IsInstanceValid(_earthquakeAudio)) _earthquakeAudio.Stop();
        if (GodotObject.IsInstanceValid(_fireAudio)) _fireAudio.Stop();
        if (GodotObject.IsInstanceValid(_camera)) _camera.Offset = _cameraOffset;
    }
}
