using Godot;

public abstract partial class EnemyBase : CharacterBody2D
{
    public int health = 100;
    public int speed = 200;
    public int damage = 10;

    protected Ram target;
    protected bool isAttacking = false;

    public override void _Ready()
    {
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