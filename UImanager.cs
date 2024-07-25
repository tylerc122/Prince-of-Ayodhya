using Godot;
using System;

public partial class UImanager : CanvasLayer
{
	// Initialize vars.
	private ProgressBar health;
	private ProgressBar stamina;
	public int healthValue;
	public int staminaValue;
	private Ram ram;
	public override void _Ready()
	{
		// Get health bar node.
		health = GetNode<ProgressBar>("HealthBar");
		stamina = GetNode<ProgressBar>("StaminaBar");

		// Get Ram node.
		ram = GetNode<Ram>("../Ram");

		// Adds GameOver method to invocation list of OnDeath event.
		ram.OnDeath += GameOver;

		// Check to see if healthbar is accurate.
		ram.TakeDamage(10);
	}
	//updates health bar to match player health
	public void updateHealth()
	{
		healthValue = ram.currentHealth;
		health.Value = healthValue;
	}
	//updates stamina bar to match player stamina
	public void updateStamina()
	{
		staminaValue = ram.currentStamina;
		stamina.Value = staminaValue;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		updateHealth();
		updateStamina();

	}

	private void GameOver()
	{
		GD.Print("Ram restarts cycle");
	}
}
