using Godot;
using System;

public partial class DoğalAfetler : Node
{
	public enum Afetler
	{
		Deprem = 0,
		Sel = 1,
		Yangın = 2,
		Heyelan = 3,
		Çığ = 4
	}
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
