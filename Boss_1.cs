using Godot;
using System;
using System.Collections;
/// <summary>
/// SO I DONT FORGET HOW THIS WORKS:
/// When an attack is chosen, the state transitions from 'Idle' to 'Attack'
/// Each specific attack method will set the 'currentAttack' and then play the associated animation
/// When the animation plays, it will call the 'PerformAttack' method, which will (eventually) execute the logic of the attack
/// Then when the animation finishes, 'OnAnimationFinished' is called and the boss state will go back to idle.
/// Moreover, when the bosses health reaches 25% or under, (s)he will enter an enraged mode, we can just speed up attacks and/or make them do more damage.
/// </summary>

public partial class Boss_1 : CharacterBody2D
{
	// Stores boss states.
	enum BossState { Idle, Attack, Enraged }
	// Initially in Idle state.
	private BossState state = BossState.Idle;

	// Basic attributes.
	public int MaxHealth = 100;
	private int health;
	private float attackTimer = 0;



	// Nodes
	private AnimatedSprite2D animatedSprite;
	private Area2D attackArea;

	// Random variable for choosing attacks.
	private Random random = new Random();
	// In charge of storing the boss's currentAttack.
	private String currentAttack;

	public override void _Ready()
	{
		health = MaxHealth;
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		attackArea = GetNode<Area2D>("Area2D");

		animatedSprite.Connect("animation_finished", new Callable(this, nameof(OnAnimationFinished)));
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
				}
				break;

			// If in Attack state
			case BossState.Attack:
				// Perform the attack.
				PerformAttack();
				break;

			// If in an Enraged State
			case BossState.Enraged:
				// Perform an enraged attack.
				PerformEnragedAttack();
				break;
		}
	}
	/// Don't think I need to say what this will do.
	private void ChooseAttack()
	{
		// Make use of our random var that we initialized earlier.
		int rand = random.Next(0, 4);
		// Switch expression. Will chooose attack based on number.
		switch (rand)
		{
			case 0:
				SlamAttack();
				break;
			case 1:
				BoulderThrow();
				break;
			case 2:
				StompAttack();
				break;
			case 3:
				ChargeAttack();
				break;
		}
		// Reset attack timer once attack is performed.
		attackTimer = 2;
	}

	/// The next four functions are very similar so I will blanket with one summary
	/// Once called, will update currentAttack, play the animation, and then "perform" the attack.
	// Note: I feel that perform is the wrong word, when I say perform I really mean the part that actually does damage,
	// The animation will be carried out but "performing" the attack in this case is the logic and the actual damage being dealt.
	private void SlamAttack()
	{
		GD.Print("Slam Attack");
		currentAttack = "SlamAttack";
		animatedSprite.Play("SlamAttack");
		PerformAttack();
	}
	private void BoulderThrow()
	{
		GD.Print("Boulder Throw");
		currentAttack = "BoulderThrow";
		animatedSprite.Play("BoulderThrow");
		PerformAttack();
	}

	private void StompAttack()
	{
		GD.Print("Stomp Attack");
		currentAttack = "StompAttack";
		animatedSprite.Play("StompAttack");
		PerformAttack();
	}

	private void ChargeAttack()
	{
		GD.Print("Charge Attack");
		currentAttack = "ChargeAttack";
		animatedSprite.Play("ChargeAttack");
		PerformAttack();
	}
	private void PerformEnragedAttack()
	{
		// NOT doin allat
	}
	// *TODO* All logic of attacks
	private void PerformAttack()
	{
		switch (currentAttack)
		{
			case "SlamAttack":
				break;
			case "BoulderThrow":
				break;
			case "StompAttack":
				break;
			case "ChargeAttack":
				break;
		}
	}

	// Called when Ram performs a successful attack, currently not in use, but self explanatory.
	public void OnHealthChanged(int newHealth)
	{
		health = newHealth;

		if (health <= 25)
		{
			state = BossState.Enraged;
		}
		else if (health <= 0)
		{
			Die();
		}
	}
	private void Die()
	{
		GD.Print("Boss Died");
	}

	// Called when an attacking(?) animation is carried out in full.
	private void OnAnimationFinished()
	{
		// Sets state back to idle if in attack
		// Note: A little confused by my own code, in what case would an animation be carried out while not in Attack mode?
		// Furthermore, do we not set the bosses state back to idle when we're in an enraged state? 
		// I'll revisit this tmrw.
		if (state == BossState.Attack)
		{
			state = BossState.Idle;
		}
	}

}
