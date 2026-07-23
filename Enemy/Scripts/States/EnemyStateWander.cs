using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyStateWander : EnemyState
    {
        [Export] private string _animName = "walk";
        [Export] private double _wanderSpeed = 20.0;
        [ExportCategory("AI")] 
        [Export] private double _stateAnimationDuration = 0.5;
        [Export] private double _minStateCycles = 1;
        [Export] private double _maxStateCycles = 3;
        [Export] private EnemyState _nextState;

        private double _timer = 0;
        private Vector2 _direction;
        
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Init()
        {
            
        }

        public override void Enter()
        {
            _timer = GD.RandRange(_minStateCycles, _maxStateCycles) * _stateAnimationDuration;
            int rand = GD.RandRange(0,3);
            _direction = Enemy.Dir4[rand];
            Enemy.Velocity = _direction * (float)_wanderSpeed;
            Enemy.SetDirection(_direction);
            Enemy.UpdateAnimation(_animName);
        }
        public override void Exit()
        {
            
        }

        public override EnemyState Process(double delta)
        {
            EnemyStateAggro aggroState = null;
            foreach (var state in StateMachine.GetChildren())
            {
                if (state is EnemyStateAggro aggro)
                {
                    aggroState = aggro;
                    break;
                }
            }

            if (aggroState != null)
            {
                // Check if the player is within the radius defined in EnemyStateAggro
                if (Enemy.Position.DistanceTo(GlobalPlayerManager.Player.Position) <= aggroState.AggroRadius)
                {
                    return aggroState; // Transition to Aggro state
                }
            }
            
            _timer -= delta;
            if (_timer <= 0)
            {
                return _nextState;
            }
            return null;
        }
        public override EnemyState Physics(double delta)
        {
            return null;
        }
    }
}

