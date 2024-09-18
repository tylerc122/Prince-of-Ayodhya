using Godot;

public partial class SpeedBoost : PowerUp
{
    private int speedBoost = 50;

    public SpeedBoost()
    {
        PowerUpName = "Speed Boost";
        Description = "Increases movement speed by 50";
        isTemporary = true;
    }

    public override void Apply(Ram ram)
    {
        ram.speed += speedBoost;
    }

    public override void RemoveEffect(Ram ram)
    {
        ram.speed -= speedBoost;
    }
}