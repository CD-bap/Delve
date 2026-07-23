using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyStateIdle : EnemyState
    {
        [Export] private string _animName = "idle";
        [ExportCategory("AI")] 
        [Export] private double _minStateDuration = 0.5;
        [Export] private double _maxStateDuration = 1.5;
        [Export] private EnemyState _afterIdleState;

        private double _timer = 0.0;
        
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Init()
        {
            
        }

        public override void Enter()
        {
            Enemy.Velocity = Vector2.Zero;
            _timer = GD.RandRange(_minStateDuration, _maxStateDuration);
            Enemy.UpdateAnimation(_animName);
        }
        public override void Exit()
        {
            
        }

        public override EnemyState Process(double delta)
        {
            if (Enemy.Name == "Boss")
            {
                return null;
            }
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
                return _afterIdleState;
            }
            return null;
        }
        public override EnemyState Physics(double delta)
        {
            return null;
        }
    }
}

