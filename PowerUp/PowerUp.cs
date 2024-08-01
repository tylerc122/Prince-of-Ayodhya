using Godot;
using System;

public abstract partial class PowerUp : Node2D
{
    public String PowerUpName { get; set; }
    public String Description { get; set; }

    public abstract void Apply(Ram ram);

    public abstract bool isTemporary();

    public abstract void RemoveEffect(Ram ram);


}