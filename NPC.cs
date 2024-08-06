using Godot;
using System;
using System.Runtime.CompilerServices;

/// Defines our basic NPC Class.
public partial class NPC : CharacterBody2D
{
	// Holds the dialogue that can be accessed by ram.
	String[] Dialogues = new String[10];

	// Tracks the index of the dialogue.
	private int dialogueIndex = 0;
	// Initialize our label, which deals with showing our dialogue in game.
	private RichTextLabel dialogueLabel;
	// Truth value if player is in range of NPC or not.
	private bool playerInRange = false;

	public override void _Ready()
	{
		// Assign vars to nodes.
		var area = GetNode<Area2D>("Area2D");
		dialogueLabel = GetNode<RichTextLabel>("RichTextLabel");

		// Connect area aspects for when Ram enters & exists the area with our specified methods.
		area.Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
		area.Connect("area_exited", new Callable(this, nameof(OnAreaExited)));

		// Set up dialogue.
		Dialogues[0] = "Hello! Welcome to " + "[b]Dandaka Forest.[/b]";
		// Ideally, the boldened controls will not be stagnant, rather we want to make it whatever they assign to that specific action.
		Dialogues[1] = "Use [b]'Space'[/b] to roll";
		Dialogues[2] = "Use [b]'Shift'[/b] to sprint";
		Dialogues[3] = "Press [b]'left-click'[/b] to attack";
		// Maybe in the future, you can hover over invincibility frames and it'll give you a deeper explanation.
		Dialogues[4] = "Rolling gives you [b]invincibility-frames[/b], use it to dodge any incoming attacks";
		// This is not implemented, just a thought to prevent spam rolling.
		Dialogues[5] = "But don't roll too much, you will become fatigued if your stamina reaches zero, meaning you will regenerate stamina much slower than normal";
		Dialogues[6] = "v";
		Dialogues[7] = "c";
		Dialogues[8] = "d";
		Dialogues[9] = "Goodbye and good luck!";


		// Initially, dialogue is not visible.
		dialogueLabel.Visible = false;
	}

	/// Deals with inputs from Ram.
	/// @param @event inputted key from user.
	public override void _Input(InputEvent @event)
	{
		// If inputted key is interact (e) and Ram is within the specified range of the NPC.
		if (@event.IsActionPressed("interact") && playerInRange)
		{
			// Allow the next dialogue to be shown.
			ShowNextDialogue();
		}
	}

	/// Called when Ram enters the area of the NPC
	/// @param the area of the object that entered NPC range.
	private void OnAreaEntered(Area2D area)
	{
		// Gets the parent of the area.
		var parent = area.GetParent();
		// If that parent is Ram.
		if (parent is Ram)
		{
			// Set playerInRange to true, let Label be visible, give the default text.
			playerInRange = true;
			dialogueLabel.Visible = true;
			dialogueLabel.Text = "[Press 'E' to interact]";
		}
	}
	/// Called when ram exists the area of the NPC
	/// @param the area of the object that entered NPC range.
	private void OnAreaExited(Area2D area)
	{
		// Same logic as OnAreaEntered:

		var parent = area.GetParent();
		if (parent is Ram)
		{
			playerInRange = false;
			dialogueLabel.Visible = false;
		}
	}
	/// Handles showing subsequent dialogue.
	private void ShowNextDialogue()
	{
		GD.Print(dialogueIndex);
		// As long as there is more dialogue left.
		if (dialogueIndex < Dialogues.Length)
		{
			// Show new dialogue.
			dialogueLabel.Text = Dialogues[dialogueIndex];
			// & Increase dialogue index.
			dialogueIndex++;
		}
		else
		{
			// If not, then we show default message and return to the beginning of dialogue.
			dialogueLabel.Text = "[Press 'E' to interact]";
			dialogueIndex = 0;
		}
	}
}
