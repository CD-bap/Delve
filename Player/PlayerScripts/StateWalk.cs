using Godot;
using System;

namespace Delve.Scripts
{
    public partial class StateWalk : State
    {
        [Export] private float _speed = 100;
        public State Idle;
        public State Attack;
        public override void _Ready()
        {
            
            base._Ready();
        }

        public override void Enter()
        {
            Player.UpdateAnimation("walk");
            base.Enter();
        }
        public override void Exit()
        {
            base.Exit();
        }

        public override State Process(double delta)
        {
            if (Player.Direction == Vector2.Zero)
            {
                return Idle;
            }
            Player.Velocity = Player.Direction * _speed;

            if (Player.SetDirection())
            {
                Player.UpdateAnimation("walk");
            }
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

