using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyStateStun : EnemyState
    {
        [Export] private string _animName = "stun";
        [Export] private double _knockBackSpeed = 200.0;
        [Export] private double _decelerateSpeed = 10.0;
        [ExportCategory("AI")] 
        [Export] private EnemyState _nextState;
        
        private Vector2 _damagePosition;
        private Vector2 _direction;
        private bool _animationFinished = false;
        
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Init()
        {
            Enemy.EnemyDamaged += OnEnemyDamaged;
        }

        public override void Enter()
        {
            Enemy.Invulnerable = true;
            _animationFinished = false;

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
            Enemy.Invulnerable = false;
            Callable callback = new Callable(this, nameof(OnAnimationFinished));
            
            Enemy.AnimationPlayer.Disconnect("animation_finished", callback);
        }

        public override EnemyState Process(double delta)
        {
            if (_animationFinished == true)
            {
                return _nextState;
            }
            Enemy.Velocity -= Enemy.Velocity * (float)_decelerateSpeed * (float)delta;
            return null;
        }
        public override EnemyState Physics(double delta)
        {
            return null;
        }

        public void OnEnemyDamaged(HurtBox hurtBox)
        {
            _damagePosition = hurtBox.GlobalPosition;
            StateMachine.ChangeState(this);
        }
        public void OnAnimationFinished(string a)
        {
            _animationFinished = true;
        }
    }
}

