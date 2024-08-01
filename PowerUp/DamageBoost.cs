using Godot;

public partial class DamageBoost : PowerUp
{
    private int damageIncrease = 5;

    public DamageBoost()
    {
        PowerUpName = "Damage Boost";
        // Either we need to specify that it's only for the run or have some distinction visually.
        Description = "Increases damage by 10 (for the current run?)";
    }

    public override void Apply(Ram ram)
    {
        //ram.Damage += damageIncrease;
    }

    public override bool isTemporary()
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveEffect(Ram ram)
    {
        throw new System.NotImplementedException();
    }
}