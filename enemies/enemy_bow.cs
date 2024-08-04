using Godot;

public partial class EnemyBow : EnemyBase
{
    public PackedScene ArrowScene;
    public float attackCooldown = 2.0f;

    private Timer attackCooldownTimer;
    private bool canAttack = true;

    public override void _Ready()
    {
        attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer");
        attackCooldownTimer.WaitTime = attackCooldown;
        attackCooldownTimer.OneShot = true;
        attackCooldownTimer.Connect("timeout", new Callable(this, nameof(OnAttackCooldownTimeout)));

        if (ArrowScene == null)
        {
            ArrowScene = (PackedScene)ResourceLoader.Load("res://arrow.tscn");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target != null && canAttack)
        {
            Attack();
        }
    }

    protected void Attack()
    {
        if (ArrowScene != null && canAttack)
        {
            var arrowInstance = (Arrow)ArrowScene.Instantiate();
            GetParent().AddChild(arrowInstance);
            arrowInstance.Initialize(GlobalPosition, target.GlobalPosition);

            canAttack = false;
            attackCooldownTimer.Start();
        }
    }

    private void OnAttackCooldownTimeout()
    {
        canAttack = true;
    }
}