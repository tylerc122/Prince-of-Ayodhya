using Godot;
using System;

public partial class Shrine : Node2D
{
	private PowerUp[] powerUpOptions = new PowerUp[3];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GeneratePowerUpOptions();
		DisplayPowerUpOptions();
	}

	private void GeneratePowerUpOptions()
	{
		PowerUpManager powerUpManager = GetNode<PowerUpManager>("../PowerUpManager");
		Random random = new Random();

		for (int i = 0; i < powerUpOptions.Length; i++)
		{
			int index = random.Next(powerUpManager.allTemporaryPowerUps.Count);
			powerUpOptions[i] = powerUpManager.allTemporaryPowerUps[index];
		}
	}

	private void DisplayPowerUpOptions()
	{
		// Need some UI to choose power up
	}

	public void OnPowerUpSelected(int optionIndex)
	{
		if (optionIndex >= 0 && optionIndex < powerUpOptions.Length)
		{
			PowerUpManager powerUpManager = GetNode<PowerUpManager>("PowerUpManager");
			powerUpManager.ApplyPowerUp(powerUpOptions[optionIndex]);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
