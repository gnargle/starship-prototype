using Godot;
using Microsoft.VisualBasic;
using System;

public partial class PlayerShip : CharacterBody3D
{
	[Export]
	public bool InvertControls = true;

	[Export]
	public int Speed { get; set; } = 14;

	[Export]
	public float BoostMultiplier { get; set; } = 2;

	[Export]
	public float BrakeMultiplier { get; set; } = 0.5f;

	public float TweenedYVal { get; set; } = 0;
	private bool _tweeningY = false;
	private Node3D Pivot;
	private PlayerCamera Camera;

	private Vector3 _targetVelocity = Vector3.Zero;

	private const int MAX_Y_DEGREES = 25;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pivot = GetNode<Node3D>("Pivot");
		Camera = GetNode<PlayerCamera>("Pivot/CameraPivot/PlayerCamera");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("boost_fire") || Input.IsActionJustReleased("brake_fire"))
		{
			Camera.ResetFOV();
		}
		if (Input.IsActionJustPressed("boost_fire"))
		{
			Camera.SetFOVBoost();
		}
		if (Input.IsActionJustPressed("brake_fire"))
		{
			Camera.SetFOVBrake();
		}
	}

	private float GetSpeedMultiplier()
	{
		if (Input.IsActionPressed("boost_fire"))
		{
			return BoostMultiplier;
		}
		if (Input.IsActionPressed("brake_fire"))
		{
			return BrakeMultiplier;
		}
		return 1;
	}

	private float GetTurnMultiplier()
	{
		return 1 + Math.Abs(Input.GetAxis("tilt_right", "tilt_left"));
	}

	private void UpdateShipHeading(double delta)
	{
		//First, we get the normalised input vector, which tells us how much the stick is tilted in any direction
		//then, we apply that input to our rotation by subtracting it from the Y and X (X and Y are swapped on input
		//compared to the vector. IDK whatever it works.
		//then we apply a forward vector to the new Pivot basis (that has been changed by us fiddling the rotation)
		//in order to continue heading forwards relative to our facing.

		var inputDir = InvertControls ? Input.GetVector("aim_left", "aim_right", "aim_down", "aim_up").Normalized()
		: Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down").Normalized();

		var rotDeg = Pivot.RotationDegrees;
		rotDeg.Y -= inputDir.X * GetTurnMultiplier();
		if (inputDir.Y != 0)
		{
			rotDeg.X -= inputDir.Y;
		}
		else if (_tweeningY)
		{
			rotDeg.X = TweenedYVal;
			if (TweenedYVal == 0)
			{
				_tweeningY = false;
			}
		}
		if (rotDeg.X > MAX_Y_DEGREES)
		{
			rotDeg.X = MAX_Y_DEGREES;
		}
		else if (rotDeg.X < -MAX_Y_DEGREES)
		{
			rotDeg.X = -MAX_Y_DEGREES;
		}
		Pivot.RotationDegrees = rotDeg;

		if (Input.IsActionJustReleased("aim_up") || Input.IsActionJustReleased("aim_down"))
		{
			//tween rotation of X axis back to zero, like the stick is returning to neutral, to level out flight.
			CreateTween().TweenProperty(this, "TweenedYVal", 0, 0.2f).From(rotDeg.X);
			_tweeningY = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		UpdateShipHeading(delta);

		Velocity = Pivot.Basis * Vector3.Forward * (Speed * GetSpeedMultiplier());
		MoveAndSlide();
	}
}
