using Godot;

public abstract partial class EnemyBase : CharacterBody2D
{
    // Enemy stats
    public int health = 100;
    public int speed = 200;
    public int damage = 10;

    // Initialize target
    protected Ram target;

    // Start off with not being able to attack
    protected bool isAttacking = false;

    public override void _Ready()
    {
        // Get target, right now only getting ram as the target.
        target = GetParent().GetNode<Ram>("Ram");
    }

    /// Handles targeting & movement
    public override void _PhysicsProcess(double delta)
    {
        // If target is initialized correctly.
        if (target != null)
        {
            // We send our enemy towards Ram
            Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
            // Calculate velocity.
            Velocity = direction * speed;
            // Move and slide.
            MoveAndSlide();
        }
    }
    /// *TODO*
    public void TakeDamage(int damage)
    {

    }
}