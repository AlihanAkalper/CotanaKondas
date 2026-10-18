using Godot;
using System;
using System.Collections.Generic;

public partial class Kiraci : AnimatedSprite2D
{
    private HouseSaveData _house;
    private FloorSaveData _tenant;
    private int _floorIndex;
    private AnimatedSprite2D _bubble;
    private float? _floorSurfaceY;
    private readonly Dictionary<Texture2D, float> _frameFeet = new();

    public void SetFloorSurface(float y)
    {
        _floorSurfaceY = y;
        AlignFeetToFloor();
    }

    private void AlignFeetToFloor()
    {
        if (!_floorSurfaceY.HasValue || SpriteFrames == null || !SpriteFrames.HasAnimation(Animation) ||
            SpriteFrames.GetFrameCount(Animation) == 0) return;
        var texture = SpriteFrames.GetFrameTexture(Animation, Frame);
        if (texture == null) return;
        if (!_frameFeet.TryGetValue(texture, out float feetY))
        {
            using var image = texture.GetImage();
            feetY = image.GetUsedRect().End.Y;
            _frameFeet.Add(texture, feetY);
        }
        float localFeetY = feetY - (Centered ? texture.GetHeight() / 2f : 0f) + Offset.Y;
        Position = new Vector2(Position.X, _floorSurfaceY.Value - localFeetY * Scale.Y);
    }
    public int Farkindalik => _tenant?.Awareness ?? 0;
    public bool HasTenant => _tenant?.HasTenant ?? false;

    public void BindTenant(HouseSaveData house, FloorSaveData tenant, int floorIndex)
    {
        _house = house;
        _tenant = tenant;
        _floorIndex = floorIndex;
        UpdateBubble();
    }

    private void UpdateBubble()
    {
        if (_bubble == null) return;
        _bubble.Visible = _house != null && _tenant != null && TenantRules.CanTalk(_house, _tenant) &&
            !(GameManager.Instance?.HasPendingDisasters ?? false);
        // Sprite'ın gerçekten baktığı yönü kullan; dururken yön değişkeni önden değişebilir.
        _bubble.Animation = FlipH ? "SagaYurume" : "SolaYurume";
        // Kuyruk sağdaysa balon başın solunda, soldaysa başın sağında kalır.
        _bubble.Position = new Vector2(FlipH ? -7.426f : 7.426f, _bubble.Position.Y);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) return;
        if (!IsVisibleInTree() || _bubble == null || !_bubble.Visible || HUD.Instance == null || HUD.Instance.DialogueOpen) return;
        var texture = _bubble.SpriteFrames.GetFrameTexture(_bubble.Animation, 0);
        var rect = new Rect2(-texture.GetSize() / 2, texture.GetSize());
        if (!rect.HasPoint(_bubble.GetLocalMousePosition())) return;
        GetViewport().SetInputAsHandled();
        HUD.Instance.ShowTenantDialogue(_house.Type, _floorIndex);
    }
	private int direction = 1;
	[Export] public float WalkSpeed = 10;
	[Export] public int MinX = -27;
	[Export] public int MaxX = 40;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        FrameChanged += AlignFeetToFloor;
        AnimationChanged += AlignFeetToFloor;
        _bubble = GetNodeOrNull<AnimatedSprite2D>("Baloncuk");
        UpdateBubble();
		Position = new Vector2((float)GD.RandRange(MinX, MaxX), Position.Y);
        AlignFeetToFloor();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private double WalkTime = 0.0;
	private double IdleTime = 0.0;
	public override void _Process(double delta)
	{
        UpdateBubble();
        if ((HUD.Instance?.DialogueOpen ?? false) || (GameManager.Instance?.HasPendingDisasters ?? false)) return;
		if(Position.X < MinX)
		{
			direction = 1;
		}else if(Position.X > MaxX)
		{
			direction = -1;
		}
		if(WalkTime > 0)
		{
			Animation = "default";
			if (direction == 1)
			{
				FlipH = true;
			}
			else
			{
				FlipH = false;
			}
			Position += new Vector2(WalkSpeed*direction*(float)delta,0);
			WalkTime -= delta;
			IdleTime += 3*delta;
		}
		else if(IdleTime <= 0)
		{
			if(GD.Randf()> 0.5){direction *= -1;}
			WalkTime += (double)GD.Randf()*3;
		}
		else
		{
			Animation = "idle";
			IdleTime -= delta;
		}
        UpdateBubble();
	}
}
