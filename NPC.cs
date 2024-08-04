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
		GetNode<CollisionShape2D>("CollisionShape2D").Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		GetNode<CollisionShape2D>("CollisionShape2D").Connect("body_exited", new Callable(this, nameof(OnBodyExited)));

		dialogueLabel = GetNode<Label>("Label");
		dialogueLabel.Visible = false;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Ram)
		{
			playerInRange = true;
			ShowNextDialogue();
		}
	}
	private void OnBodyExited()
	{

	}
	private void ShowNextDialogue()
	{

	}
}
