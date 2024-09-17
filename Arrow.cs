using Godot;
using System;
// Note: Commented out variables are used if we want arc on the arrow, will decide when troubleshooting
// Note: I don't want to finish it but I know what to do, probably will end up doing it.
public partial class Arrow : Area2D
{
	public int speed = 300;
	public int damage = 10;

	// public float gravity = 500.0f;
	// private Vector2 velocity;
	// private Vector2 initialPosition;
	private Vector2 direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void Initialize(Vector2 startPosition, Vector2 targetPosition)
	{
		Position = startPosition;
		direction = (targetPosition - startPosition).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += direction * speed * (float)delta;

		// Delete if arrow goes off sceen? Would go here if we want/need to do it.
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Ram ram)
		{
			ram.TakeDamage(damage);
			// Destory arrow after hitting ram.
			QueueFree();
		}
		else
		{
			// Collision w/ floor? Other objects? Not a 1am problem.
			//QueueFree(); (?)
		}
	}
}
