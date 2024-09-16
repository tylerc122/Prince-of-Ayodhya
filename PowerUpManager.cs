using Godot;
using System;
using System.Collections.Generic;

public partial class PowerUpManager : Node
{
	private Ram ram;
	private List<PowerUp> temporaryPowerUps = new List<PowerUp>();
	public List<PowerUp> allTemporaryPowerUps = new List<PowerUp>();

	public override void _Ready()
	{
		ram = GetNode<Ram>("../Ram");
		allTemporaryPowerUps.Add(new DamageBoost());
		allTemporaryPowerUps.Add(new SpeedBoost());
		allTemporaryPowerUps.Add(new DefenseBoost());
	}

	public void ApplyPowerUp(PowerUp powerUp)
	{
		powerUp.Apply(ram);

		if (!powerUp.isTemporary())
		{
			// Not sure how to implement perm power ups yet
		}
		else
		{
			temporaryPowerUps.Add(powerUp);
		}
	}

	public void OnDeath()
	{
		foreach (var powerUp in temporaryPowerUps)
		{
			powerUp.RemoveEffect(ram);
		}
		temporaryPowerUps.Clear();
	}
}
