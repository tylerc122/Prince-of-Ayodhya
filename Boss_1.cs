using Godot;
using System;
using System.Collections;
/// <summary>
/// SO I DONT FORGET HOW THIS WORKS:
/// First, the boss starts in an Idle state, health is set to 100 (max) and the attack timer starts at zero.
/// When the boss spawns in, attack timer is zero so immediately they will proceed to choose an attack.
/// The boss then moves towards ram using MoveTowardsRam().
/// Once ram is in attack range, IsRamInAttackRange() returns true, which then means a random attack is chosen.
/// This means that ExecuteAttack() is called, the attack name and animation name are set and the currentAttack is set to wtvr the attack is, the animation is also played & state is changed to attack.
/// During the next _PhysicsProcess() call, the boss is now an attack state so PerformAttack() will be called with normal multipliers.
/// Perform attack then checks currentAttack and calls the corresponding method.
/// When the animation is finished, OnAnimationFinished() is called and the boss goes back to idle, unless under 25 hp, then it will stay in enraged mode.
/// Note that many of the attack methods take a damage multiplier parameter, this is only used (in theory) when the boss is in an enraged state.
/// </summary>

public partial class Boss_1 : CharacterBody2D
{
	// Stores boss states
	enum BossState { Idle, Attack, Enraged }
	// Initially in Idle state
	private BossState state = BossState.Idle;

	// Basic attributes
	public int MaxHealth = 100;
	private int health;
	private float attackTimer = 0;
	private String currentAttack;

	private float moveSpeed = 100.0f;
	private float closeRange = 200.0f;

	// Enraged multipliers
	float enragedDamageMultiplier = 2.0f;
	float enragedSpeedMultiplier = 1.5f;

	// Nodes
	private AnimatedSprite2D animatedSprite;
	private Area2D attackArea;
	private Ram ram;
	private Area2D boulderArea;


	// Ranged attack attributes
	private float rangedAttackDistance = 300.0f;
	private float rangedAttackChance = 0.3f;
	private float windUpDuration = 1.0f;
	private float windUpTimer = 0f;
	private bool isWindingUp = false;
	private Sprite2D boulderSprite;

	private Random random = new Random();

	public override void _Ready()
	{
		// Assign attributes & nodes on ready
		health = MaxHealth;

		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		attackArea = GetNode<Area2D>("Area2D");

		ram = GetNode<Ram>("../Ram");

		var boulderArea = GetNode<Area2D>("../Boulder/Area2D");

		if (boulderArea == null)
		{
			GD.Print("boulderArea not initialized properly");
		}
		else
		{
			GD.Print("boulderArea good");
		}

		// Once animation finished, called OnAnimationFinished()
		animatedSprite.Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));

		boulderSprite = new Sprite2D();
		boulderSprite.Texture = ResourceLoader.Load<Texture2D>("res://sprites/d72ns19-705ce806-22a6-4112-a4d4-64fcd2fd76dd.png");
		// Make it smaller than the actual boulder
		boulderSprite.Scale = new Vector2(0.2f, 0.2f);
		boulderSprite.Visible = false;
		AddChild(boulderSprite);

	}
	public override void _PhysicsProcess(double delta)
	{
		// Switch expression based on the current state of the boss.
		switch (state)
		{
			// If in an Idle state
			case BossState.Idle:
				// Check if attackTimer is expired
				if (attackTimer <= 0)
				{
					// If so, choose an attack to do.
					// Note: I'm a little worried that this will be executed too fast/slow, not convinced a timer will be smooth enough.
					// Depending on how much damage ram is doing, I believe we'll have to adjust this.
					// If too fast, possible to maybe have another random case, maybe every 1/2 times he'll attack? idk
					// If too slow, we can always reduce the timer or some other robust solution, will think on it.
					ChooseAttack();
				}
				else
				{
					// If the attack timer isn't up, we can just reduce it.
					attackTimer -= (float)delta;
					MoveTowardsRam(moveSpeed, (float)delta);
				}

				break;

			// If in Attack state
			case BossState.Attack:

				// Perform the attack with no multiplier.
				PerformAttack(1.0f, 1.0f);
				state = BossState.Idle;
				break;

			// If in an Enraged State
			case BossState.Enraged:
				// Perform the attack with the enraged multipliers.
				PerformAttack(enragedDamageMultiplier, enragedSpeedMultiplier);
				state = BossState.Idle;
				break;
		}

		if (isWindingUp)
		{
			windUpTimer += (float)delta;

			if (windUpTimer >= windUpDuration)
			{
				isWindingUp = false;
				windUpTimer = 0f;
				boulderSprite.Visible = false;
				ExecuteRangedAttack();
			}
		}
	}

	/// After moving towards ram, boss_1 will choose an attack at random.
	private void ChooseAttack()
	{
		float distanceToRam = (ram.GlobalPosition - GlobalPosition).Length();

		if (distanceToRam > rangedAttackDistance && random.NextDouble() < rangedAttackChance)
		{
			StartWindUp();
		}

		else if (distanceToRam <= closeRange)
		{
			// Make use of our random var that we initialized earlier.
			int attackChoice = random.Next(4);
			// Switch expression. Will chooose attack based on number.
			switch (attackChoice)
			{
				case 0:
					ExecuteAttack("SlamAttack", "SlamAttack");
					break;
				case 1:
					ExecuteAttack("StompAttack", "StompAttack");
					break;
				case 2:
					ExecuteAttack("ChargeAttack", "ChargeAttack");
					break;
				case 3:
					StartWindUp();
					break;
			}
		}

		else
		{
			MoveTowardsRam(moveSpeed, 0.1f);
		}
		// Reset attack timer once attack is performed.
		attackTimer = 2;
	}

	/// Starts the windup for boulder attack.
	private void StartWindUp()
	{
		isWindingUp = true;
		// Increasing this value decreases the time spend winding up
		// Note: We may want to make this a variable value so that the time for it to wind up won't
		// always be the same, depends on how hard we want to make the game.
		windUpTimer = 0.5f;
		// Put the boulder just above the boss for now, god knows if we'll ever get this far in animating.
		boulderSprite.GlobalPosition = GlobalPosition + new Vector2(0, -100);
		boulderSprite.Visible = true;
		boulderSprite.Scale = new Vector2(0.04f, 0.04f);
		GD.Print("Started wind up");
	}
	/// Executes the attack by setting current attack, playing anim, & calling PerformAttack()
	/// @param attackName the name of the attack chosen from ChooseAttack()
	/// @param animationName the name of the animation corresponding with the attack.
	private void ExecuteAttack(string attackName, string animationName)
	{
		GD.Print(attackName);

		currentAttack = attackName;

		animatedSprite.Play(animationName);

		state = BossState.Attack;
	}

	/// Actually conducts the attack, animation, state change, everything that comes with a regular attack.
	private void ExecuteRangedAttack()
	{
		GD.Print("Executing ranged attack");
		currentAttack = "BoulderThrow";
		animatedSprite.Play("BoulderThrow");
		state = BossState.Attack;
		DealRangedDamage(10, boulderArea);
	}

	/// Performs the logic of the attack based on the currentAttack.
	private void PerformAttack(float damageMultiplier, float speedMultiplier)
	{
		// Since we now have the boss moving towards ram before attacking,
		// we need them to stop when they do the attack.
		Velocity = Vector2.Zero;
		switch (currentAttack)
		{
			case "SlamAttack":
				SlamAttackLogic(damageMultiplier);
				break;
			case "BoulderThrow":
				BoulderThrowLogic(damageMultiplier);
				break;
			case "StompAttack":
				StompAttackLogic(damageMultiplier);
				break;
			case "ChargeAttack":
				ChargeAttackLogic(damageMultiplier);
				break;
		}
	}

	/// Logic from the slam attack option.
	private void SlamAttackLogic(float damageMultiplier)
	{
		// If Ram is range for the intial slamdown
		if (IsRamInAttackRange(50.0f))
		{
			// He takes 20 damage.
			ram.TakeDamage((int)(20 * damageMultiplier));
		}
		// Either way, the shockwave will be created.
		CreateShockwave();
	}

	// The actual creation of the shockwave from the slam.
	private void CreateShockwave()
	{
		// Loads a resource form the shockwave scene, we must cast to a 'PackedScene' since we'll get a 'Resource' object if we don't.
		PackedScene shockwaveScene = (PackedScene)ResourceLoader.Load("res://shockwave.tscn)");

		// Succesfully loaded?
		if (shockwaveScene != null)
		{
			// If yes, create an instance of the PackedScene. Casting to Area2D
			Area2D shockwaveInstance = (Area2D)shockwaveScene.Instantiate();

			// This will make sure that the shockwave starts where the boss is.
			shockwaveInstance.Position = GlobalPosition;

			// Gets the parent of the Boss_1 node, should just be the root node (Node2D)
			// Then it'll add the instance of the shockwave as a child of the parent, making it active & visible in game.
			GetParent().AddChild(shockwaveInstance);

			Vector2 directionToRam = (ram.GlobalPosition - GlobalPosition).Normalized();

			// We then call StartShockwave() in shockwave.cs which starts the actual movement of the shockwave. 
			shockwaveInstance.Call("StartShockwave", directionToRam);
		}
	}

	/// Logic for the boulder throw attack option.
	private void BoulderThrowLogic(float damageMultiplier)
	{
		// Same thing as shockwave.
		PackedScene boulderScene = (PackedScene)ResourceLoader.Load("res://boulder.tscn");

		// Make sure loaded properly.
		if (boulderScene != null)
		{
			// Create an instance of the boulder, unlike the shockwave, we need this to be a RigidBody2D since we're dealing with physics & gravity.
			RigidBody2D boulderInstance = (RigidBody2D)boulderScene.Instantiate();

			// Make it visible and active in game.
			GetParent().AddChild(boulderInstance);

			// Starts at the boss.
			boulderInstance.GlobalPosition = GlobalPosition;

			GD.Print($"Boss Position: {GlobalPosition}");
			GD.Print($"Boulder Position: {boulderInstance.GlobalPosition}");

			// Initialize & assign directionToRam, need to make sure we're throwing boulder at ram.
			Vector2 directionToRam = (ram.GlobalPosition - GlobalPosition).Normalized();

			// Speed of boulder.
			// Note: Might need to slow down
			float boulderSpeed = 1900.0f;

			// Sets velocity of boulder.
			boulderInstance.LinearVelocity = directionToRam * boulderSpeed;

			float scaleFactor = 0.9f;
			boulderInstance.Scale = new Vector2(scaleFactor, scaleFactor);
		}

	}

	/// Logic fort he stomp attack option.
	private void StompAttackLogic(float damageMultiplier)
	{
		// Check if ram is in range.
		if (IsRamInAttackRange(100.0f))
		{
			// If he is, he'll take 15 damage.
			ram.TakeDamage((int)(15 * damageMultiplier));
			// He will also be knocked back
			Vector2 knockbackDirection = (ram.GlobalPosition - GlobalPosition).Normalized();
			ram.ApplyKnockback(knockbackDirection, 200.0f);

		}
		// Then we create the stomping effect.
		CreateStompEffect();
	}

	/// Creates the actual effect needed to visually notify the user that the shockwave has been sent out.
	private void CreateStompEffect()
	{
		PackedScene stompScene = (PackedScene)ResourceLoader.Load("res://stomp.tscn");

		if (stompScene != null)
		{
			Area2D stompInstance = (Area2D)stompScene.Instantiate();

			stompInstance.Position = GlobalPosition;

			GetParent().AddChild(stompInstance);

			stompInstance.Call("StartStompEffect");
		}
	}

	/// Logic for the charge attack option.
	private void ChargeAttackLogic(float damageMultiplier)
	{
		float chargeSpeed = 500.0f;

		MoveTowardsRam(chargeSpeed, (float)GetProcessDeltaTime());

		if (IsRamInAttackRange(30.0f))
		{
			ram.TakeDamage((int)(25 * damageMultiplier));
		}
	}

	/// Called when we want to check if ram is in the boss's attack range
	/// @param attackRange the range of the specific attack.
	/// @return bool the truth value of whether or not ram is within range or not.
	private bool IsRamInAttackRange(float attackRange)
	{
		return (ram.GlobalPosition - GlobalPosition).Length() <= attackRange;
	}

	/// Called when Ram performs a successful attack, currently not in use, but self explanatory.
	public void TakeDamage(int damage)
	{
		health -= damage;

		GD.Print("Boss took damage.");

		if (health <= 25)
		{
			state = BossState.Enraged;
		}
		else if (health <= 0)
		{
			Die(); ;
		}
	}

	/// Called when the boss die.
	private void Die()
	{
		GD.Print("Boss Died");
	}

	/// Called when an attacking(?) animation is carried out in full.
	private void OnAnimationFinished()
	{
		// If we're in attack state.
		if (state == BossState.Attack)
		{
			// Check if the health is above 25
			if (health > 25)
			{
				// If it is, we go back to idle.
				state = BossState.Idle;
			}
			else
			{
				// If it isn't go back to enraged.
				state = BossState.Enraged;
			}
		}
		attackTimer = 2;
	}

	/// Makes the boss move towards ram.
	private void MoveTowardsRam(float speed, float delta)
	{
		Vector2 directionToRam = (ram.GlobalPosition - GlobalPosition).Normalized();

		Velocity = directionToRam * speed;

		MoveAndSlide();
	}

	/// Specifically used for the boss to deal damage off the boulder throw.
	/// @param damage the damage that the boss will deal to ram.
	/// @param area the area of the boulder that we can use(?) 
	private void DealRangedDamage(int damage, Area2D area)
	{

	}
}