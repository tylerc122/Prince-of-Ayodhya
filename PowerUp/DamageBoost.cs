using Godot;

public partial class DamageBoost : PowerUp
{
    private int damageIncrease = 5;

    public DamageBoost()
    {
        PowerUpName = "Damage Boost";
        // Either we need to specify that it's only for the run or have some distinction visually. Maybe we can just make it clear thru gameplay, don't think we need allat
        Description = "Increases damage by 10";
        isTemporary = true;
    }

    public override void Apply(Ram ram)
    {
        ram.attackDamage += damageIncrease;
    }

    public override void RemoveEffect(Ram ram)
    {
        ram.attackDamage -= damageIncrease;
    }
}