using Godot;
using System;

public abstract partial class PowerUp : Node2D
{
    public String PowerUpName { get; protected set; }
    public String Description { get; protected set; }
    public bool isTemporary { get; protected set; }

    public abstract void Apply(Ram ram);
    public abstract void RemoveEffect(Ram ram);


}