using Godot;
using System;

namespace Delve.Scripts
{
    public partial class StateStun : State
    {
        [Export] private float _knocbackSpeed = 100;
        [Export] private float _decelerateSpeed = 100;
        [Export] private float _invulnerableDuration = 1;
        
        private HurtBox _hurtBox;
        private Vector2 _direction;
        
        private State _nextState = null;
        
        public State Idle;
        public override void _Ready()
        {
            
            base._Ready();
        }
        
        
        public override void Init()
        {

            Player.PlayerDamaged += PlayerDamaged;
        }

        public override void Enter()
        {
            Callable callback = new Callable(this, nameof(AnimationFinished));

            if (Player.AnimationPlayer.IsConnected("animation_finished", callback))
            {
                Player.AnimationPlayer.Disconnect("animation_finished", callback); // Ensure no duplicate connections
            }

            Player.AnimationPlayer.Connect("animation_finished", callback);
            
            _direction = Player.GlobalPosition.DirectionTo(_hurtBox.GlobalPosition);
            Player.Velocity = _direction * -_knocbackSpeed;
            Player.SetDirection();
            
            Player.UpdateAnimation("stun");
            
            Player.MakeInvulnerable(_invulnerableDuration);
            Player.EffectAnimationPlayer.Play("damaged");
            
            base.Enter();
        }
        public override void Exit()
        {
            _nextState = null;
            
            Callable callback = new Callable(this, nameof(AnimationFinished));
            
            Player.AnimationPlayer.Disconnect("animation_finished", callback);
            base.Exit();
        }

        public override State Process(double delta)
        {
            
            return _nextState;
        }
        public override State Physics(double delta)
        {
            return null;
        }
        public override State HandleInput(InputEvent @event)
        {
            return null;
        }

        public void PlayerDamaged(HurtBox hurtBox)
        {
            if (StateMachine == null)
            {
                GD.PrintErr("State machine is not initialized!");
                return;
            }

            _hurtBox = hurtBox;

            if (_hurtBox == null)
            {
                GD.PrintErr("HurtBox is null! Cannot process PlayerDamaged.");
                return;
            }

            StateMachine.ChangeState(this);

        }
        public void AnimationFinished(string a)
        {
            _nextState = Idle;
        }
    }
}