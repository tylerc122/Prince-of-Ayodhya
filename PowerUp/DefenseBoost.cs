using Godot;

public partial class DefenseBoost : PowerUp
{
    private int defenseBoost = 5;

    public DefenseBoost()
    {
        PowerUpName = "Defense Boost";
        Description = "Decreases damage taken by 5";
        isTemporary = true;
    }

    public override void Apply(Ram ram)
    {
        ram.defense += defenseBoost;
    }
    public override void RemoveEffect(Ram ram)
    {
        ram.defense -= defenseBoost;
    }
}