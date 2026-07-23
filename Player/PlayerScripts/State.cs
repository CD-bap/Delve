using Godot;
using System;

namespace Delve.Scripts
{
    public partial class State : Node
    {
        public Player Player;
        public PlayerStateMachine StateMachine;

        public override void _Ready()
        {
            base._Ready();
        }

        public virtual void Init()
        {
            
        }
        public virtual void Enter()
        {
            
        }
        public virtual void Exit()
        {
            
        }

        public virtual State Process(double delta)
        {
            return null;
        }
        public virtual State Physics(double delta)
        {
            return null;
        }
        public virtual State HandleInput(InputEvent @event)
        {
            return null;
        }
    }
}

