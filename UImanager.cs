using Godot;
using System;

public partial class UImanager : CanvasLayer
{
	// Initialize vars.
	private ProgressBar health;
	private Ram ram;
	public override void _Ready()
	{
		// Get health bar node.
		health = GetNode<ProgressBar>("HealthBar");

		// Get Ram node.
		ram = GetNode<Ram>("../CharacterBody2D");

		// Check to see if healthbar is accurate.
		ram.TakeDamage(10);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update health bar to match health every frame
		health.Value = ram.currentHealth;
	}
}
