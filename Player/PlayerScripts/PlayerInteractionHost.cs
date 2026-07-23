using Godot;
using System;
using System.Diagnostics;

namespace Delve.Scripts
{
    public partial class PlayerInteractionHost : Node2D
    {
        public Player Player;
        public override void _Ready()
        {
            Player = GetParent<Player>();
            Player.DirectionChanged += UpdateDirection;
            base._Ready();
        }

        public void UpdateDirection(Vector2 newDirection)
        {
            if (newDirection == Vector2.Down)
            {
                RotationDegrees = 0;
            }
            else if (newDirection == Vector2.Up)
            {
                RotationDegrees = 180;
            }
            else if (newDirection == Vector2.Left)
            {
                RotationDegrees = 90;
            }
            else if (newDirection == Vector2.Right)
            {
                RotationDegrees = -90; 
            }
            else
            {
                RotationDegrees = 0;
            }

        }
    }
}

