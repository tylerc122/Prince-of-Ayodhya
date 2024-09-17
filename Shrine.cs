using Godot;
using System;

public partial class Shrine : Node2D
{
	// Ideally we should have three powerup options per shrine.
	public int numberOfOptions = 3;

	private PowerUp[] powerUpOptions;
	private PowerUpManager powerUpManager;


	public override void _Ready()
	{
		// I don't believe these should happen on ready unless we plan to instantiate the shrine when the level ends.
		// Not even sure how ready works on instantiated objects, we'll see.
		GeneratePowerUpOptions();
		DisplayPowerUpOptions();
	}

	// Handles generating the randomized power ups.
	private void GeneratePowerUpOptions()
	{
		// Get assign powerupmanager node.
		PowerUpManager powerUpManager = GetNode<PowerUpManager>("../PowerUpManager");
		// Assign random var.
		Random random = new Random();

		// For loop to assign what power ups will be displayed.
		for (int i = 0; i < powerUpOptions.Length; i++)
		{
			// Our index is a random number picked from any number from 1 to n where n is the total count of all temporary power ups.
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
