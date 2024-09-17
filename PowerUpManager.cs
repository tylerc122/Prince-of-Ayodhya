using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PowerUpManager : Node
{
	private Ram ram;
	public List<PowerUp> activePowerUps = new List<PowerUp>();
	public List<PowerUp> allPermanentPowerUps = new List<PowerUp>();
	public List<PowerUp> allTemporaryPowerUps = new List<PowerUp>();
	public override void _Ready()
	{
		ram = GetNode<Ram>("../Ram");
		InitializePowerUps();

	}

	public void InitializePowerUps()
	{
		allTemporaryPowerUps.Add(new DamageBoost());
		allTemporaryPowerUps.Add(new SpeedBoost());
		allTemporaryPowerUps.Add(new DefenseBoost());
	}
	public void ApplyPowerUp(PowerUp powerUp)
	{
		powerUp.Apply(ram);
		activePowerUps.Add(powerUp);
	}

	public void OnDeath()
	{
		foreach (var powerUp in activePowerUps.ToList())
		{
			if (powerUp.isTemporary)
			{
				powerUp.RemoveEffect(ram);
				activePowerUps.Remove(powerUp);
			}
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (var powerUp in activePowerUps)
		{
			powerUp.Apply(ram);
		}
	}
}
