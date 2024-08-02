using Godot;
using System;
using System.Numerics;

public partial class enemy_basic : CharacterBody2D
{
	// Basic attributes
	public int speed = 200;
	public int damage = 10;


	// State variables
	private bool isAttacking = false;
	private Ram target;
	private Godot.Vector2 velocity = Godot.Vector2.Zero;

	// Nodes 
	private Timer attackCooldownTimer;
	private CollisionShape2D attackRange;
	public override void _Ready()
	{
		// Target Ram.
		target = GetParent().GetNode<Ram>("Ram");

		// Assign attackCooldownTimer & its properties.
		attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer");
		attackCooldownTimer.WaitTime = 1.0f;
		attackCooldownTimer.OneShot = true;
		attackCooldownTimer.Connect("timeout", new Callable(this, nameof(ResetAttack)));

		// Assign attackRange & its properties.
		attackRange = GetNode<CollisionShape2D>("AttackRange");
		attackRange.Connect("body-entered", new Callable(this, nameof(OnAttackRangeEntered)));
	}
	public override void _PhysicsProcess(double delta)
	{
		// Direct our enemy towards Ram.
		Godot.Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
		velocity = direction * speed;

		// Move
		MoveAndSlide();
	}

	/// Called when someone (for now, only Ram) enters our enemies' attack range.
	/// @param body the node of which the enemy's attack range has collided with.
	private void OnAttackRangeEntered(Node body)
	{
		// As long as the body is Ram's and the enemy isn't already attacking
		if (body is Ram && !isAttacking)
		{
			// Call Attack().
			Attack();
		}
	}
	/// Called when an attack needs to be carried out
	private void Attack()
	{
		// Set our bool to true so we don't instantly kill Ram. (makes conditional above false)
		isAttacking = true;

		// Use our TakeDamage method to make our target (Ram) take damage.
		target.TakeDamage(damage);

		// Start the timer for the cooldown.
		attackCooldownTimer.Start();
	}
	/// Called when our attackCooldownTimer times out.
	private void ResetAttack()
	{
		// Set isAttacking to false so that when attack range collides w another body, will attack again.
		isAttacking = false;
	}
}
