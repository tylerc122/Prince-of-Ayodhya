using Godot;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

public partial class Ram : CharacterBody2D
{
	// Basic attributes
	public int speed = 350;
	public int sprintSpeed = 500;
	public int rollSpeed = 750;
	public int maxHealth = 100;
	public int maxStamina = 100;
	public int staminaDrain = 1;
	public int staminaRegen = 1;
	public int attackDamage = 10;
	private const float roll_duration = 0.34f;


	// State variables
	public int currentHealth;
	public int currentStamina;
	public int tracker = 0;
	private bool isRolling = false;
	private Godot.Vector2 rollDirection;

	// Knockback variables
	private Godot.Vector2 knockbackVelocity = Godot.Vector2.Zero;
	private float knockbackDuration = 0.2f;
	private float knockbackTimer = 0;

	// Invincibility variables
	private bool isInvincible = false;
	private float invincibilityDuration = 0.5f;
	private Timer invincibiltyTimer;

	// Nodes
	private AnimatedSprite2D animatedSprite2D;
	private CollisionShape2D collisionShape2D;
	private Timer rollTimer;
	private Timer staminaRegenTimer;
	private Area2D interactionArea;
	private Area2D attackArea;
	private Boss_1 boss;

	// Event(s)
	public event Action OnDeath;

	/// _Ready is called when a node is added to the scene tree and all its children nodes are ready
	public override void _Ready()
	{
		// Assign vars, get nodes from scene.
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		staminaRegenTimer = GetNode<Timer>("StaminaRegenTimer");
		rollTimer = GetNode<Timer>("RollTimer");
		invincibiltyTimer = GetNode<Timer>("InvincibilityTimer");
		interactionArea = GetNode<Area2D>("Area2D");
		boss = GetNode<Boss_1>("/root/Node2D/Boss_1");
		attackArea = GetNode<Area2D>("AttackArea");

		if (attackArea == null)
		{
			GD.Print("Attack area bad");
		}

		if (boss == null)
		{
			GD.Print("boss isn't right");
		}

		// rollTimer properties.
		rollTimer.WaitTime = roll_duration;
		rollTimer.OneShot = true;
		rollTimer.Connect("timeout", new Callable(this, nameof(OnRollTimerTimeout)));

		// Max health & stam initially.
		currentHealth = maxHealth;
		currentStamina = maxStamina;

		// staminaRegenTimer properties.
		staminaRegenTimer.WaitTime = 0.05f;
		staminaRegenTimer.Connect("timeout", new Callable(this, nameof(RegenerateStamina)));
		staminaRegenTimer.Start();

		// Initially idle
		animatedSprite2D.Play("idle_right");

		invincibiltyTimer.OneShot = true;
		invincibiltyTimer.WaitTime = invincibilityDuration;
		staminaRegenTimer.Connect("timeout", new Callable(this, nameof(ResetInvIncibility)));

	}

	/// _PhysicsProcess will handle walking/idle animations as well as basic cardinal (and possibly ordinal) movement.
	/// @param delta time elapsed since the last frame was processed.
	public override void _PhysicsProcess(double delta)
	{
		if (knockbackTimer > 0)
		{
			// Reduce the knockback timer.
			knockbackTimer -= (float)delta;

			Velocity = knockbackVelocity;

			MoveAndSlide();

			return;
		}

		if (Input.IsActionJustPressed("attack"))
		{
			Attack();
		}

		// Before we do any kind of idle/walking animation, we must check if we are currently in a roll state.
		if (isRolling)
		{
			Velocity = rollDirection * rollSpeed;
			MoveAndSlide();
			return;
		}

		// Calculate current velocity & set it equal to our Vector2 var.
		Godot.Vector2 velocity = CalculateVelocity();

		bool sprintCheck = IsSprinting();


		// First we check whether or not the velocity vector is zero or not.
		if (velocity != Godot.Vector2.Zero)
		{
			// If our vector is anything but (0,0) We can use a ternary operator. 
			// If we are sprinting, we mult our normalized velocity by the sprint speed rather than the normal walking speed, if it isn't then we just mult by speed.
			velocity = velocity.Normalized() * (sprintCheck ? sprintSpeed : speed);
			// Set the velocity calculated equal to Velocity property of our CharacterBody2D class.
			Velocity = velocity;
			// Call MoveAndSlide to handle collision & movement.
			MoveAndSlide();
			// Update our animation based on the velocity.
			UpdateWalkAnimation(velocity);

		}
		// If our velocity vector happens to be zero.
		else
		{
			// Play the idle animation.
			PlayIdleAnimation();

		}

		// If the roll button is pressed and our rollTimer isn't running.
		if (Input.IsActionPressed("ui_roll") && rollTimer.IsStopped() && currentStamina >= 20)
		{
			GD.Print(currentStamina);
			// We allow the user to roll.
			StartRoll();
			currentStamina -= 20;
		}
	}
	private Godot.Vector2 CalculateVelocity()
	{
		// Initialize our velocity vector.
		Godot.Vector2 velocity = new Godot.Vector2();
		bool movingRight = Input.IsActionPressed("ui_right");
		bool movingLeft = Input.IsActionPressed("ui_left");
		bool movingUp = Input.IsActionPressed("ui_up");
		bool movingDown = Input.IsActionPressed("ui_down");

		// Check if right key is being pressed.
		if (movingRight)
		{
			// If it is, x component of the velocity vector is increased by 1, in turn moving us right.
			velocity.X += 1;
			tracker = 1;
		}

		// Check if the left key is being pressed.
		if (movingLeft)
		{
			// If it is, the x component of the velocity vector is decreased by 1, in turn moving us left.
			velocity.X -= 1;
			tracker = 2;
		}

		// Check if the up key is being pressed.
		if (movingUp)
		{
			// If it is, decrease the y component, moving us up.
			velocity.Y -= 1;
			tracker = 3;
		}

		// Check if the down key is being pressed.
		if (movingDown)
		{
			// If it is, increase the y component, moving us down.
			velocity.Y += 1;
			tracker = 4;
		}

		// Update tracker for diagonal movements.
		if (movingRight && movingUp)
		{
			// up_right
			tracker = 5;
		}
		else if (movingRight && movingDown)
		{
			// down_right
			tracker = 7;
		}
		else if (movingLeft && movingUp)
		{
			// up_left
			tracker = 6;
		}
		else if (movingLeft && movingDown)
		{
			// down_left
			tracker = 8;
		}

		return velocity;
	}
	/// Handles when Ram is sprinting.
	/// @return bool returns truth value of if Ram is sprinting or not.
	private bool IsSprinting()
	{
		// If the sprint key is being held, and we have stamina to spend.
		if (Input.IsActionPressed("ui_sprint") && currentStamina > 0)
		{
			// We continuously drain our stamina from our current stamina.
			currentStamina -= staminaDrain;

			// To make sure that we don't go below 0 stamina.
			if (currentStamina < 0)
			{
				currentStamina = 0;
			}
			return true;
		}
		return false;
	}
	/// Handles updating animations in regards to walking/running
	/// @param velocity the inputted vector used to determine which animation to play.
	private void UpdateWalkAnimation(Godot.Vector2 velocity)
	{
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

	/// Handles playing the idle animation based on tracker.
	private void PlayIdleAnimation()
	{
		// We can use a switch case in order to handle our idle animations.
		// Since we've been consistently updating our tracker variable, we know what the last direction moved was, so we play the corresponding animation based on that.
		switch (tracker)
		{
			case 1:
			// up_right
			case 5:
			// down_right
			case 7:
				animatedSprite2D.Play("idle_right");
				break;

			case 2:
			// up_left
			case 6:
			// down_left
			case 8:
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
	/// Handles the animations of Ram's roll as well as disabling our collisionShape.
	private void StartRoll()
	{

		isRolling = true;
		// Similar switch case to our idle animation, but rather than our idle animation, we want our roll animation.
		// We also update our rollDirection vector, so that we roll in the correct direction.
		switch (tracker)
		{
			case 1:
				GD.Print("r");
				animatedSprite2D.Play("roll_right");
				rollDirection = new Godot.Vector2(1, 0);
				break;
			case 2:
				GD.Print("l");
				animatedSprite2D.Play("roll_left");
				rollDirection = new Godot.Vector2(-1, 0);
				break;
			case 3:
				GD.Print("u");
				animatedSprite2D.Play("roll_up");
				rollDirection = new Godot.Vector2(0, -1);
				break;
			case 4:
				GD.Print("d");
				animatedSprite2D.Play("roll_down");
				rollDirection = new Godot.Vector2(0, 1);
				break;

			case 5:
				// up_right
				GD.Print("ur");
				animatedSprite2D.Play("roll_right");
				rollDirection = new Godot.Vector2(1, -1).Normalized();
				break;
			case 6:
				// up_left
				GD.Print("ul");
				animatedSprite2D.Play("roll_left");
				rollDirection = new Godot.Vector2(-1, -1).Normalized();
				break;
			case 7:
				// down_right
				GD.Print("dr");
				animatedSprite2D.Play("roll_right");
				rollDirection = new Godot.Vector2(1, 1).Normalized();
				break;
			case 8:
				// down_left
				GD.Print("dl");
				animatedSprite2D.Play("roll_left");
				rollDirection = new Godot.Vector2(-1, 1).Normalized();
				break;
		}

		// Once an animation has been chosen, we immediately disable our collision shape and start the timer.
		collisionShape2D.Disabled = true;
		rollTimer.Start();
	}
	/// Handles enabling our collisionShape after roll.
	private void OnRollTimerTimeout()
	{
		// Once the timer is up, since we connected our timeout signal to this function, this function will be called
		// and we will return to our original state, turning our colisionShape back on.
		isRolling = false;
		collisionShape2D.Disabled = false;
	}

	/// Called when Ram is hit by something.
	/// @param amount the amount of damage Ram takes.
	public void TakeDamage(int amount)
	{
		if (isInvincible)
		{
			return;
		}
		// Take away the damage taken from current health.
		currentHealth -= amount;

		// If current health goes below 0 or is 0.
		if (currentHealth <= 0)
		{
			// Make sure to set it back to 0, don't need neg health.
			currentHealth = 0;

			// Handle game over logic in UImanager.
			OnDeath?.Invoke();
		}
		isInvincible = true;

		invincibiltyTimer.Start();
	}

	// Resets invincibility after hit.
	private void ResetInvIncibility()
	{
		isInvincible = false;
	}

	/// Called when Ram picks up a healing item.
	/// @param amount the amount of health Ram will gain.
	public void Heal(int amount)
	{
		// Add the health to Ram's current health.
		currentHealth += amount;

		// If his current health exceeds maxHealth.
		if (currentHealth > maxHealth)
		{
			// Bring it back to the maxHealth, not supporting overheal for now.
			currentHealth = maxHealth;
		}
	}
	// Called when stam regen wait timer times out.
	public void RegenerateStamina()
	{
		// If Ram's current stamina is below max stam.
		if (!Input.IsActionPressed("ui_sprint") && currentStamina < maxStamina)
		{
			// Give him more stamina.
			currentStamina += staminaRegen;

			// When he gets to a point where it's exceeding the max.
			if (currentStamina > maxStamina)
			{
				// Just set it to the max.
				currentStamina = maxStamina;
			}
		}
	}

	public void Attack()
	{
		if (attackArea != null)
		{
		}
	}
	/// Called when Ram needs to be knocked back.
	public void ApplyKnockback(Godot.Vector2 direction, float force)
	{
		knockbackVelocity = direction * force;
		knockbackTimer = knockbackDuration;
	}
	public void OnAreaEntered(Area2D area)
	{
		GD.Print($"Ram entered area: {area.Name}");
	}

}


