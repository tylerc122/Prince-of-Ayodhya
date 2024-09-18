/// Shrine.cs will ideally be one of two ways ram will achieve power ups. The 'shrine', ambiguous name since I don't know what it should actually be
/// will bestow the temporary power ups, most likely after every level you beat up to the boss. Veer suggested that we should do different gods,
/// similar to how hades has boons that corresponds to the greeks gods, we'll have gods like Saraswati, Agni, Durga, and more, ideally they all 
/// will give unique power ups based on their given descriptions in Hinduism. The counterpart to shrine will be a meditation area we have in the home base area
/// this will be useful after Ram completes hard encounters such as bosses. Bosses will reward Ram with 'soul points' which will be used in meditation to
/// upgrade his soul, these are the permanent upgrades that will transfer over each time he restarts the cycle. 
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
}
