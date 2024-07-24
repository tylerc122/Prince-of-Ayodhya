using Godot;
using System;

public partial class UImanager : CanvasLayer
{

	private ProgressBar health;
	public int value;
	private Ram ram;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = GetNode<ProgressBar>("HealthBar");
		
		// BULL SHIT I HAVE NO IDEA WHY IT WAS SO HARD I HATE GODOT
		ram= GetNode<Ram>("../CharacterBody2D");

		ram.TakeDamage(10);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//update value to rams health at every frame
		value = ram.Health();
		//update health bar to match health every frame
		health.Value = value;
	}
}
