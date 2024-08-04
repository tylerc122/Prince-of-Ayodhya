using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class NPC : CharacterBody2D
{
	public string[] Dialogues;
	private int dialogueIndex = 0;

	private Label dialogueLabel;
	private bool playerInRange = false;

	public override void _Ready()
	{
		var area = GetNode<Area2D>("Area2D");

		area.Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));

		area.Connect("area_exited", new Callable(this, nameof(OnAreaExited)));


		dialogueLabel = GetNode<Label>("Label");

		dialogueLabel.Visible = false;

		GD.Print("NPC Ready. Dialogue label initialized.");
	}
	public override void _Input(InputEvent @event)
	{
		GD.Print($"Player in range: {playerInRange}");
		if (@event.IsActionPressed("interact") && playerInRange)
		{
			GD.Print("Interaction triggered. Showing next dialogue.");
			ShowNextDialogue();
		}
	}

	private void OnAreaEntered(Area2D area)
	{
		GD.Print($"Something entered: {area.Name} of type {area.GetType()}");
		var parent = area.GetParent();
		GD.Print($"Parent node: {parent.Name}, Type: {parent.GetType()}");

		if (parent is Ram)
		{
			GD.Print("Ram entered the area.");
			playerInRange = true;
			dialogueLabel.Visible = true;
			dialogueLabel.Text = "[Press 'E' to interact]";
		}
		else
		{
			GD.Print($"Entered area's parent is not of type Ram: {parent.GetType()}");
		}
	}
	private void OnAreaExited(Area2D area)
	{
		var parent = area.GetParent();
		if (parent is Ram)
		{
			playerInRange = false;
			dialogueLabel.Visible = false;
			GD.Print("Ram exited NPC range.");
		}
	}
	private void ShowNextDialogue()
	{
		if (dialogueIndex < Dialogues.Length)
		{
			dialogueLabel.Text = Dialogues[dialogueIndex];
			dialogueIndex++;
		}
		else
		{
			dialogueLabel.Text = "";
			dialogueIndex = 0;
		}
	}
}
