using Godot;
using System;

public partial class Ram : CharacterBody2D
{
	// Controls speed of Ram.
	public int speed = 350;
	// Controls sprint speed of Ram.
	public int sprintSpeed = 500;
	// Controls roll speed of Ram.
	public int rollSpeed = 750;
	// Controls max health of Ram.
	public int maxHealth = 100;
	// Controls current health of Ram.
	public int currentHealth;
	// Controls max stamina of Ram.
	public int maxStamina = 100;
	// Controls current stamina of Ram.
	public int currentStamina;
	// Controls how fast Ram's stamina drains.
	public int staminaDrain = 10;
	// Controls how fast Ram's stamina regenerates.
	public int staminaRegen = 10;
	// Tracks the last known direction traversed
	// 1 for right, 2 for left, 3 for up, 4 for down, 5 for up_right, 6 for up_left, 7 for down_right, 8 for down_left.
	public int tracker = 0;

	// Initializing varibles that we will assign later on ready.
	private AnimatedSprite2D animatedSprite2D;
	private CollisionShape2D collisionShape2D;
	private Timer rollTimer;
	private Vector2 rollDirection;
	private Timer staminaRegenTimer;

	// Controls how long the roll lasts.
	private const float roll_duration = 0.34f;

	// An integral piece of our animations is making sure they don't clash, intially, without this bool var, as soon as the roll button was pressed, the animation would stop after a frame due to either the walking,
	// or the idle animation playing immmediately after, to fix this, we make sure the user isn't in a roll before we play any other animations.
	private bool isRolling = false;

	/// Called when the node enters the scene tree for the first time.
	/// Basically _Ready is called when a node is added to the scene tree and all its children nodes are ready.
	public override void _Ready()
	{
		// We use _Ready() to assign our previously initialized AnimatedSprite2D var. We get our animated 2d node in our scene tree and set it equal to this variable.
		// This will be used when it comes to movement and animations later.
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		// We'll now assign our collisionshape var, this'll be used later when we add i-frames on our roll.
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

		// Assign our Timer var for our roll.
		rollTimer = GetNode<Timer>("RollTimer");

		// Controls after how long our timer will emit the timeout signal.
		rollTimer.WaitTime = roll_duration;

		// Our timer is now a 'OneShot' timer, meaning that it will only run one time when started.
		rollTimer.OneShot = true;

		// When the our timer, rollTimer, times out, we call the func OnRollTimerTimeout().
		rollTimer.Connect("timeout", new Callable(this, nameof(OnRollTimerTimeout)));

		currentHealth = maxHealth;

		currentStamina = maxStamina;

		staminaRegenTimer = GetNode<Timer>("StaminaRegenTimer");

		staminaRegenTimer.WaitTime = 1.0f;

		staminaRegenTimer.Connect("timeout", new Callable(this, nameof(RegenerateStamina)));

		staminaRegenTimer.Start();


		// We also want to initalize a starting animation, we'll have ram's idle_right animation playing as we start a new level.
		animatedSprite2D.Play("idle_right");
	}

	public void TakeDamage(int amount)
	{
		currentHealth -= amount;

		if (currentHealth <= 0)
		{
			currentHealth = 0;

			GD.Print("Game Over!");
		}
	}

	public void Heal(int amount)
	{
		currentHealth += amount;

		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}

	public void RegenerateStamina()
	{
		if (currentStamina < maxStamina)
		{
			currentStamina += staminaRegen;

			if (currentStamina > maxStamina)
			{
				currentStamina = maxStamina;
			}
		}
	}

	/// _PhysicsProcess will handle all of our animations as well as basic cardinal movement, we decided not to have animations for
	/// ordinal directions for simplicity sake.
	public override void _PhysicsProcess(double delta)
	{
		// Before we do any kind of idle/walking animation, we must check if we are currently in a roll state.
		if (isRolling)
		{

			Velocity = rollDirection * rollSpeed;
			MoveAndSlide();
			return;
		}

		// Initialize our velocity vector.
		Vector2 velocity = new Vector2();

		// Set up bool variables, this makes it a lot easier when we make our conditional statements for animations and movement.
		// ui_right, left, up, and down are mapped to wasd keys or the arrow keys in godot.
		bool movingRight = Input.IsActionPressed("ui_right");
		bool movingLeft = Input.IsActionPressed("ui_left");
		bool movingUp = Input.IsActionPressed("ui_up");
		bool movingDown = Input.IsActionPressed("ui_down");
		bool roll = Input.IsActionPressed("ui_roll");
		bool sprint = Input.IsActionPressed("ui_sprint");

		if (sprint && currentStamina > 0)
		{
			currentStamina -= staminaDrain;

			if (currentStamina < 0)
			{
				currentStamina = 0;
			}
		}
		else if (currentStamina <= 0)
		{
			sprint = false;
		}

		// Cardinal & Ordinal Directions.
		// Check if right key is being pressed.
		if (movingRight)
		{
			// If it is, x component of the velocity vector is increased by 1, in turn moving us right.
			velocity.X += 1;

			// Check if the up key is being pressed.
			if (movingUp)
			{
				// If it is, we must also decrease our y component, in turn moving us up_right.
				velocity.Y -= 1;

				// Update tracker.
				tracker = 5;
			}

			// If we aren't pressing up, check if down being pressed.
			else if (movingDown)
			{
				// If it is, we must also increase our y component, in turn moving us down_right.
				velocity.Y += 1;

				// Update tracker.
				tracker = 7;
			}

			// If only the right key is being pressed.
			else
			{
				// We've already updated our position accordingly, so we just need to update the tracker.
				tracker = 1;
			}

		}

		// Check if the left key is being pressed.
		if (movingLeft)
		{
			// If it is, the x component of the velocity vector is decreased by 1, in turn moving us left.
			velocity.X -= 1;

			// Check if up key being pressed.
			if (movingUp)
			{
				// If it is, we must also decrease our y component, in turn moving us up_left. 
				velocity.Y -= 1;

				// Update tracker.
				tracker = 6;
			}

			// If we aren't pressing up, check if down being pressed.
			else if (movingDown)
			{
				// If it is, we must also increase our y component, in turn moving us down_left.
				velocity.Y += 1;

				// Update tracker.
				tracker = 8;
			}

			// If only the left key is being pressed.
			else
			{
				// We've already updated our position accordingly, so we just need to update the tracker.
				tracker = 2;
			}
		}

		// Check if only the up key is being pressed.
		if (movingUp && !movingRight && !movingLeft)
		{
			// If it is, only decrease y component.
			velocity.Y -= 1;
			// Update tracker.
			tracker = 3;

		}

		// Check if only the down key is being pressed.
		if (movingDown && !movingRight && !movingLeft)
		{
			// If it is, only increase y component.
			velocity.Y += 1;
			// Update tracker.
			tracker = 4;
		}

		// Animation handling.
		// First we check whether or not the velocity vector is zero or not.
		if (velocity != Vector2.Zero)
		{
			// If our vector is anything but (0,0) We  use a ternary operator. 
			// If sprint is being held down, we mult our normalized velocity by the sprint speed rather than the normal walking speed, if it isn't then we just mult by speed.
			velocity = velocity.Normalized() * (sprint ? sprintSpeed : speed);


			Velocity = velocity;
			MoveAndSlide();

			// After that, we can start handling animations, based on what our x and y components of velocity are doing, we can update the animations accordingly.
			// This is also where, if we decide to in the future, we could update our animations to also handle ordinal direction animation handling.
			//*TODO*
			if (velocity.X > 0)
			{
				if (velocity.Y < 0)
				{
					// up_right
					animatedSprite2D.Play("right");
				}
				else if (velocity.Y > 0)
				{
					// down_right
					animatedSprite2D.Play("right");
				}
				else
				{
					animatedSprite2D.Play("right");
				}
			}

			else if (velocity.X < 0)
			{
				if (velocity.Y < 0)
				{
					// up_left
					animatedSprite2D.Play("left");
				}
				else if (velocity.Y > 0)
				{
					// down_left
					animatedSprite2D.Play("left");
				}
				else
				{
					animatedSprite2D.Play("left");
				}
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
			// Since we've been consistently updating our tracker variable, we know what the last direction moved was, so we play the corresponding animation based on that.
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
				case 5:
					// up_right
					animatedSprite2D.Play("idle_right");
					break;
				case 6:
					// up_left
					animatedSprite2D.Play("idle_left");
					break;
				case 7:
					// down_right
					animatedSprite2D.Play("idle_right");
					break;
				case 8:
					// down_left
					animatedSprite2D.Play("idle_left");
					break;
			}
		}

		// If the roll button is pressed and our rollTimer isn't running.
		if (roll && rollTimer.IsStopped() && currentStamina >= 40)
		{
			// We allow the user to roll.
			StartRoll();
			currentStamina -= 40;
		}
	}

	/// Handles the animations of Ram's roll as well as disabling our collisionShape.
	private void StartRoll()
	{
		isRolling = true;
		// Similar switch case to our idle animation, but rather than our idle animation, we want our roll animation.
		// We also update our rollDirection vector, so that we roll in the correct direction.
		switch (tracker)
		{
			case 1:
				animatedSprite2D.Play("roll_right");
				rollDirection = new Vector2(1, 0);
				break;
			case 2:
				animatedSprite2D.Play("roll_left");
				rollDirection = new Vector2(-1, 0);
				break;
			case 3:
				animatedSprite2D.Play("roll_up");
				rollDirection = new Vector2(0, -1);
				break;
			case 4:
				animatedSprite2D.Play("roll_down");
				rollDirection = new Vector2(0, 1);
				break;

			case 5:
				// up_right
				animatedSprite2D.Play("roll_right");
				rollDirection = new Vector2(1, -1).Normalized();
				break;
			case 6:
				// up_left
				animatedSprite2D.Play("roll_left");
				rollDirection = new Vector2(-1, -1).Normalized();
				break;
			case 7:
				// down_right
				animatedSprite2D.Play("roll_right");
				rollDirection = new Vector2(1, 1).Normalized();
				break;
			case 8:
				// down_left
				animatedSprite2D.Play("roll_left");
				rollDirection = new Vector2(-1, 1).Normalized();
				break;
		}

		// Once an animation has been chosen, we immediately disable our collision shape and start the timer.
		collisionShape2D.Disabled = true;
		rollTimer.Start();
	}

	/// Handles enabling our collisionShape.
	private void OnRollTimerTimeout()
	{
		// Once the timer is up, since we connected our timeout signal to this function, this function will be called
		// and we will return to our original state, turning our colisionShape back on.
		isRolling = false;
		collisionShape2D.Disabled = false;
	}
}

