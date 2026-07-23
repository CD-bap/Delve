using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyStateDestroy : EnemyState
    {
        [Export] private string _animName = "death";
        [Export] private double _knockBackSpeed = 200.0;
        [Export] private double _decelerateSpeed = 10.0;
        [ExportCategory("AI")]
        
        private Vector2 _damagePosition;
        private Vector2 _direction;
        private HitBox _hitBox;
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Init()
        {
            Enemy.EnemyDestroyed += OnEnemyDestroyed;
        }

        public override void Enter()
        {
            Enemy.Invulnerable = true;
            _direction = Enemy.GlobalPosition.DirectionTo(_damagePosition);
            Enemy.SetDirection(_direction);
            Enemy.Velocity = _direction * -(float)_knockBackSpeed;
            
            Enemy.UpdateAnimation(_animName);
            
            Callable callback = new Callable(this, nameof(OnAnimationFinished));

            if (Enemy.AnimationPlayer.IsConnected("animation_finished", callback))
            {
                Enemy.AnimationPlayer.Disconnect("animation_finished", callback); // Ensure no duplicate connections
            }

            Enemy.AnimationPlayer.Connect("animation_finished", callback);


        }
        public override void Exit()
        {
            
        }

        public override EnemyState Process(double delta)
        {
            Enemy.Velocity -= Enemy.Velocity * (float)_decelerateSpeed * (float)delta;
            return null;
        }
        public override EnemyState Physics(double delta)
        {
            return null;
        }

        public void OnEnemyDestroyed(HurtBox hurtBox)
        {
            _damagePosition = hurtBox.GlobalPosition;
            StateMachine.ChangeState(this);
        }
        public void OnAnimationFinished(string a)
        {
            Enemy.QueueFree();
        }
    }
}

