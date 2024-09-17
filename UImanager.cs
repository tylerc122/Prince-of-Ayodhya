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
	private Sprite2D barOutlines;

	public override void _Ready()
	{
		health = GetNode<ProgressBar>("Control/HealthBar");
		stamina = GetNode<ProgressBar>("Control/StaminaBar");
		ram = GetNode<Ram>("../Ram");
		ram.OnDeath += GameOver;
		ram.TakeDamage(10);

		// New code for bar outlines
		barOutlines = GetNode<Sprite2D>("Control/Sprite2D");
		GetTree().Root.SizeChanged += UpdateBarOutlinesPosition;

		// Call it once to set initial position
		UpdateBarOutlinesPosition();
	}

	public override void _Process(double delta)
	{
		updateHealth();
		updateStamina();

	}

	/// Updates health bar to match player health
	public void updateHealth()
	{
		healthValue = ram.currentHealth;
		health.Value = healthValue;
	}

	/// Updates stamina bar to match player stamina
	public void updateStamina()
	{
		staminaValue = ram.currentStamina;
		stamina.Value = staminaValue;
	}

	/// ACQUIRED FROM CHATPGT
	/// This was really annoying and you won't see me dealing with the UI so what's done is done SORRY.
	private void UpdateBarOutlinesPosition()
	{
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

		float xPosition = viewportSize.X - (barOutlines.Texture.GetWidth() * barOutlines.Scale.X / 2);
		float yPosition = barOutlines.Texture.GetHeight() * barOutlines.Scale.Y / 2;

		barOutlines.Position = new Vector2(xPosition, yPosition);
	}

	public override void _ExitTree()
	{
		// Disconnect the signal when the node is removed from the scene
		GetTree().Root.SizeChanged -= UpdateBarOutlinesPosition;
	}
	private void GameOver()
	{
		GD.Print("Ram restarts cycle");
	}
}


