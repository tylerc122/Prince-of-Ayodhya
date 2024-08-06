using Godot;
using System;
using System.Reflection;
using System.Threading;

public partial class Weapon_Axis : Marker2D
{

	private Godot.Timer Attack_Timer;
	private Marker2D marker2D;

	private Sprite2D Sword;

	private bool cooldown = true;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		marker2D = GetNode<Marker2D>("Handle_Axis");
		Attack_Timer = GetNode<Godot.Timer>("Attack_Timer");
		Sword = GetNode<Sprite2D>("Handle_Axis/Sword");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LookAt(GetGlobalMousePosition());

		if (Input.IsActionPressed("click") && cooldown){
			marker2D.RotationDegrees = 180;
			Attack_Timer.Start();
			cooldown = false;
			
		}
		if (this.RotationDegrees >= 90 && this.RotationDegrees <= 270 || this.RotationDegrees <= -90 && this.RotationDegrees >= -270){
			this.Scale = new Vector2(1, -1);
			
		}
		else{
			this.Scale = new Vector2(1, 1);
		}
		if (this.RotationDegrees >= 360 ||this.RotationDegrees <= -360){
			this.RotationDegrees = 0;
		}

	

		
	}
	public void _on_attack_timer_timout(){
		marker2D.RotationDegrees = 0;
		cooldown = true;
	}
}
	
