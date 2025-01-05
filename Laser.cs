using Godot;
using System;

public partial class Laser : CharacterBody3D
{
	[Export]
	public int Speed { get; set; } = 25;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = this.Basis * Vector3.Forward * Speed;
		MoveAndSlide();
	}

	public void Initialize(Basis basis, Vector3 playerPosition)
	{
		this.Position = playerPosition;
		this.Basis = basis;
	}

	private void OnLaserTimerTimeout()
	{
		QueueFree();
	}
}
