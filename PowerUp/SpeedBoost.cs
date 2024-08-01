using Godot;

public partial class SpeedBoost : PowerUp
{
    private int speedBoost = 5;

    public SpeedBoost()
    {
        PowerUpName = "Speed Boost";
        // Either we need to specify that it's only for the run or have some distinction visually.
        Description = "Increases movement speed by x";
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