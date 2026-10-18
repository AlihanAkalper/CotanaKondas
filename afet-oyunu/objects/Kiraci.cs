using Godot;
using System;

public partial class Kiraci : AnimatedSprite2D
{
	public int Farkindalik = (int)GD.Randf()*50 + 50;
	private int direction = 1;
	[Export] public float WalkSpeed = 10;
	[Export] public int MinX = -27;
	[Export] public int MaxX = 50;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = new Vector2(GD.Randf()*77 - 27,Position.Y);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private double WalkTime = 0.0;
	private double IdleTime = 0.0;
	public override void _Process(double delta)
	{
		if(Position.X < -27)
		{
			direction = 1;
		}else if(Position.X > 50)
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
	}
}
