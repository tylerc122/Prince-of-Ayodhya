using Godot;
using System;

public partial class Icon : Node2D
{
	public int Speed = 200;
	private Sprite2D spriteRight;
	private Sprite2D spriteLeft;
	private Sprite2D spriteUp;
	private Sprite2D spriteDown;
	private Sprite2D spriteUpRight;
	private Sprite2D spriteUpLeft;
	private Sprite2D spriteDownRight;
	private Sprite2D spriteDownLeft;

    // Called when the node enters the scene tree for the first time.
	// Basically _Ready is called when a node is added to the scene tree and all its children nodes are ready.
    public override void _Ready()
    {
		// We use _Ready here to initalize all of our Sprite2D variables by getting references to the actual Sprite2D nodes in the scene
		// We assign each direction to it's corresponding drawn sprite, we do this by using the GetNode<Sprite2D>() method,
		// which gets the named node in the method from the scene tree.
		spriteRight = GetNode<Sprite2D>("spriteRight");
		spriteLeft = GetNode<Sprite2D>("spriteLight");
		spriteUp = GetNode<Sprite2D>("spriteUp");
		spriteDown = GetNode<Sprite2D>("spriteDown");
		spriteUpRight = GetNode<Sprite2D>("spriteUpRight");
		spriteUpLeft = GetNode<Sprite2D>("spriteUpLeft");
		spriteDownRight = GetNode<Sprite2D>("spriteDownRight");
		spriteDownLeft = GetNode<Sprite2D>("spriteDownLeft");

		// Shows right facing first.
		ShowOnlySprite(spriteRight)
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		Vector2 velocity = new Vector2();

		if(Input.IsActionPressed("ui_right")){
			velocity.X += 1;
			// How do we get sprite to face right direction?
		}

		if(Input.IsActionPressed("ui_left")){
			velocity.X -= 1;
			// How do we get sprite to face right direction?
		}

		if(Input.IsActionPressed("ui_down")){
			velocity.Y += 1;
			// How do we get sprite to face right direction?
		}

		if(Input.IsActionPressed("ui_up")){
			velocity.Y -= 1;
			// How do we get sprite to face right direction?
		}

		velocity = velocity.Normalized() * Speed * (float)delta;
		Position += velocity;
	}
}
