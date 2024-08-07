using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class shockwave : Area2D
{
	// Speed of shockwave
	private float speed = 200.0f;

	// Can just use positional updating rather than moveandslide, don't need collision really.
	private Vector2 velocity = new Vector2();

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		// When the shockwave enters a body, call OnBodyentered
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	/// This should be called at the end of every call of CreateShockwave()
	/// NOTE: now that I'm looking at it, do we even need this file? feel like we could just do it in Boss_1.
	/// @param A normalized vector that indicates the direction the shockwave should go.
	public void StartStompEffect(Vector2 direction)
	{
		// Calculate velocity.
		velocity = direction.Normalized() * speed;

		// Play animation
		animatedSprite.Play("shockwave");
	}
	public override void _PhysicsProcess(double delta)
	{
		// Update position, collision isn't too important, ram will go thru shockwave anyway.
		Position += velocity * (float)delta;
	}

	/// Self explanatory, just checking to see if it's Ram.
	private void OnBodyEntered(Node body)
	{
		if (body is Ram ram)
		{
			ram.TakeDamage(10);
		}
	}
}
