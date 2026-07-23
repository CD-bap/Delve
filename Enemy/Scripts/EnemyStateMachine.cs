using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class EnemyStateMachine : Node
    {
        private Array<EnemyState> _states;
        public EnemyState PervState;
        private EnemyState _currentState;
        
        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            base._Ready();
        }

        public override void _Process(double delta)
        {
            ChangeState(_currentState.Process(delta));
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            ChangeState(_currentState.Physics(delta));
            base._PhysicsProcess(delta);
        }

        public void Initialize(Enemy enemy)
        {
            _states = [];
            foreach (Node c in GetChildren())
            {
                if (c is EnemyState enemyState)
                {
                    enemyState.Enemy = enemy;
                    _states.Add(enemyState);
                }
            }

            foreach (EnemyState s in _states)
            {
                s.Enemy = enemy;
                s.StateMachine = this;
                s.Init();
            }

            if (_states.Count > 0)
            {
                ChangeState(_states[0]);
                ProcessMode = ProcessModeEnum.Inherit;
            }
        }

        public void ChangeState(EnemyState newState)
        {
            if (newState == null || newState == _currentState)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.Exit();
            }
            PervState = _currentState;
            _currentState = newState;
            _currentState.Enter();
        }
    }
}

