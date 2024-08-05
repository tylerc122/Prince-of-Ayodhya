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

    public override void _PhysicsProcess(double delta)
    {
        if (target != null)
        {
            Vector2 direction = (target.GlobalPosition - GlobalPosition).Normalized();
            Velocity = direction * speed;
            MoveAndSlide();
        }
    }
    public void TakeDamage(int damage)
    {

    }
}