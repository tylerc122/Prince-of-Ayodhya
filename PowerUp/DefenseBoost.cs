using Godot;

public partial class DefenseBoost : PowerUp
{
    private int defenseBoost = 5;

    public DefenseBoost()
    {
        PowerUpName = "Defense Boost";

        // Either we need to specify that it's only for the run or have some distinction visually.
        Description = "Decreases damage taken by 5";
    }

    public override void Apply(Ram ram)
    {

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