using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class shockwave : Area2D
{
	private float speed = 200.0f;
	private Vector2 velocity = new Vector2();

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	public void StartShockwave(Vector2 direction)
	{
		velocity = direction.Normalized() * speed;
	}
	public override void _PhysicsProcess(double delta)
	{
		Position += velocity * (float)delta;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Ram ram)
		{
			ram.TakeDamage(10);
		}
	}
}
