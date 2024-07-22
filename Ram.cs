using Godot;
using System;

public partial class Ram : CharacterBody2D
{
	// Controls speed of sprite
	public int Speed = 350;

	// Tracks the last known direction traversed
	// 1 for right, 2 for left, 3 for up, 4 for down.
	public int tracker = 0;

	private AnimatedSprite2D animatedSprite2D;

	/// Called when the node enters the scene tree for the first time.
	/// Basically _Ready is called when a node is added to the scene tree and all its children nodes are ready.
	public override void _Ready()
	{
		// We use _Ready to assign our previously initialized AnimatedSprite2D var.
		// We get our animated 2d node in our scene tree and set it equal to this variable.
		// This will be used when it comes to movement and animations later.
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// We also want to initalize a starting animation, we'll have ram's idle_right animation playing as we start a new level.
		animatedSprite2D.Play("idle_right");
	}

	/// _PhysicsProcess will handle all of our animations as well as basic cardinal movement, we decided not to have animations for
	/// ordinal directions for simplicity sake.
	public override void _PhysicsProcess(double delta)
	{
		// Initialize our velocity vector.
		Vector2 velocity = new Vector2();

		// Set up bool variables, this makes it a lot easier when we make our conditional statements
		// for animations and movement.
		// ui_right, left, up, and down are mapped to wasd keys or the arrow keys in godot.
		bool movingRight = Input.IsActionPressed("ui_right");
		bool movingLeft = Input.IsActionPressed("ui_left");
		bool movingUp = Input.IsActionPressed("ui_up");
		bool movingDown = Input.IsActionPressed("ui_down");

		// Cardinal directions.
		if (movingRight)
		{
			// If the right key is pressed, the x component of the velocity vector is increased by 1, in turn moving us right.
			velocity.X += 1;
			// We also make sure to update our tracker.
			tracker = 1;

		}
		if (movingLeft)
		{
			// If the left key is pressed, the x component of the velocity vector is decreased by 1, in turn moving us left.
			velocity.X -= 1;
			// Update tracker.
			tracker = 2;
		}
		if (movingUp)
		{
			// If the up key is pressed, the y component of the velocity vector is DECREASED by 1, in turn moving us up.
			// Contrary to what intuition says, we decrease our y component instead of increasing because in godot, (0,0) is in
			// the top left corner instead of the middle of the screen, therefore we need to decrease y in order for us to move up.
			velocity.Y -= 1;
			// Update tracker.
			tracker = 3;

		}
		if (movingDown)
		{
			// If the down key is pressed, the y component of the velocity vector is INCREASED by 1, explanation above.
			velocity.Y += 1;
			// Update tracker.
			tracker = 4;
		}
		// Animation handling.
		// First we check whether or not the velocity vector is zero or not.
		if (velocity != Vector2.Zero)
		{
			// If our vector is anything but (0,0), we normalize the vector velocity, ensuring the direction of movement is maintained but the length of the
			// vector is set to 1.
			// We then multiply that normalized velocity by both speed and delta time, this adjusts the velocity to reflect the desired speed and also makes it frame independent.
			velocity = velocity.Normalized() * Speed * (float)delta;
			// We then update the position of ram.
			Position += velocity;
			// After that, we can start handling animations, based on what our x and y components of velocity are doing,
			// we can update the animations accordingly.
			// This is also where, if we decide to in the future, we could update our animations to also handle ordinal direction animation handling.
			if (velocity.X > 0)
			{
				animatedSprite2D.Play("right");
			}
			else if (velocity.X < 0)
			{
				animatedSprite2D.Play("left");
			}
			else if (velocity.Y > 0)
			{
				animatedSprite2D.Play("down");
			}
			else if (velocity.Y < 0)
			{
				animatedSprite2D.Play("up");
			}
		}
		// If our velocity vector happens to be zero.
		else
		{
			// We can use a switch case in order to handle our idle animations.
			// Since we've been consistently updating our tracker variable, we know what the last
			// direction moved was, so we play the corresponding animation based on that.
			switch (tracker)
			{
				case 1:
					animatedSprite2D.Play("idle_right");
					break;
				case 2:
					animatedSprite2D.Play("idle_left");
					break;
				case 3:
					animatedSprite2D.Play("idle_up");
					break;
				case 4:
					animatedSprite2D.Play("idle_down");
					break;
			}
		}
	}
}

