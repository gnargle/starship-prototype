using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
	[Export]
	public int BoostFOV = 130;
	[Export]
	public int BrakeFOV = 70;
	[Export]
	public int DefaultFOV = 90;
	[Export]
	public float FOVTweenTime = 0.2f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetFOVBoost()
	{
		CreateTween().TweenProperty(this, "fov", BoostFOV, FOVTweenTime);
	}

	public void SetFOVBrake()
	{
		CreateTween().TweenProperty(this, "fov", BrakeFOV, FOVTweenTime);
	}

	public void ResetFOV()
	{
		CreateTween().TweenProperty(this, "fov", DefaultFOV, FOVTweenTime);
	}
}
