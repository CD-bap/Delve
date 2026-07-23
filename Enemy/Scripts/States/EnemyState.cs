using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyState : Node
    {
        public Enemy Enemy;
        public EnemyStateMachine StateMachine;

        public virtual void Init()
        {
            
        }

        public virtual void Enter()
        {
            
        }
        public virtual void Exit()
        {
            
        }

        public virtual EnemyState Process(double delta)
        {
            return null;
        }
        public virtual EnemyState Physics(double delta)
        {
            return null;
        }
    }
}

