using Godot;
using System;

namespace Delve.Scripts
{
    public partial class StateIdle : State
    {
        public State Walk;
        public State Attack;
        public override void _Ready()
        {
            
            base._Ready();
        }

        public override void Enter()
        {
            Player.UpdateAnimation("idle");
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
        }

        public override State Process(double delta)
        {
            if (Player.Direction != Vector2.Zero)
            {
                return Walk;
            }
            
            Player.Velocity = Vector2.Zero;
            return null;
        }
        public override State Physics(double delta)
        {
            return null;
        }
        public override State HandleInput(InputEvent @event)
        {
            if (@event.IsActionPressed("attack"))
            {
                return Attack;
            }
            return null;
        }
    }
}

