using Godot;

public partial class EnemyBow : EnemyBase
{
    // Scene variable(s).
    public PackedScene ArrowScene;

    // Attack cooldown variables.
    public float attackCooldown = 2.0f;
    private Timer attackCooldownTimer;

    // State variable(s)
    private bool canAttack = true;

    public override void _Ready()
    {
        // Initialize attackCooldownTimer
        // This includes making it oneshot, setting up wait time, & connecting timeout to specialized method.
        attackCooldownTimer = GetNode<Timer>("AttackCooldownTimer");
        attackCooldownTimer.WaitTime = attackCooldown;
        attackCooldownTimer.OneShot = true;
        attackCooldownTimer.Connect("timeout", new Callable(this, nameof(OnAttackCooldownTimeout)));

        // As long as the arrow scene isn't initialzed falsely.
        if (ArrowScene == null)
        {
            // Initialize it.
            ArrowScene = (PackedScene)ResourceLoader.Load("res://arrow.tscn");
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        // If target is initialized and able to attack.
        if (target != null && canAttack)
        {
            // Call Attack().
            Attack();
        }
    }

    protected void Attack()
    {
        // If ArrowScene is initialized properly and attack cooldown timer isn't running.
        if (ArrowScene != null && canAttack)
        {
            // Initialize arrowInstance to an instance of our arrow scene.
            var arrowInstance = (Arrow)ArrowScene.Instantiate();
            GetParent().AddChild(arrowInstance);
            // Initializes the arrow instance at the position of the bow enemy.
            arrowInstance.Initialize(GlobalPosition, target.GlobalPosition);

            // Start the attack cooldown timer and set canAttack to false.
            canAttack = false;
            attackCooldownTimer.Start();
        }
    }

    private void OnAttackCooldownTimeout()
    {
        // Set canAttack to true on time out of attack cooldown timer.
        canAttack = true;
    }
}