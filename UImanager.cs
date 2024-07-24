using Godot;
using System;

public partial class UImanager : CanvasLayer
{

	private ProgressBar health;
	private ProgressBar stamina;
	public int healthValue;
	public int staminaValue;
	private Ram ram;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = GetNode<ProgressBar>("HealthBar");
		stamina = GetNode<ProgressBar>("StaminaBar");
		
		// BULL SHIT I HAVE NO IDEA WHY IT WAS SO HARD I HATE GODOT
		ram= GetNode<Ram>("../CharacterBody2D");

		ram.TakeDamage(10);
	}
	//updates health bar to match player health
	public void updateHealth(){

		healthValue = ram.currentHealth;
		health.Value = healthValue;
	}
	//updates stamina bar to match player stamina
	public void updateStamina(){
		staminaValue = ram.currentStamina;
		stamina.Value = staminaValue;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		updateHealth();
		updateStamina();

	}
}
