using Godot;
using System;
using System.Threading.Tasks;

namespace Delve.Scripts
{
    public partial class StateAttack : State
    {
        private bool _attacking = false;

        [Export] private AudioStream _attackPlayer;
        private double _decelerateSpeed = 10.0;
        
        
        public AnimationPlayer AnimationPlayer;
        public AnimationPlayer AttackAnimPlayer;
        public AudioStreamPlayer2D Audio;
        public HurtBox HurtBox;
        public State Walk;
        public State Idle;
        public override void _Ready()
        {
            
            base._Ready();
        }

        public override async void Enter()
        {
            Player.UpdateAnimation("attack");
            AttackAnimPlayer.Play("attack_" + Player.AnimDirection());
            AnimationPlayer.AnimationFinished += EndAttack;

            Audio.Stream = _attackPlayer;
            Audio.Play();
            Audio.PitchScale = (float)GD.RandRange( 0.9, 1.1);
            _attacking = true;
            
            base.Enter();

            await Delay(0.15);
            HurtBox.Monitoring = true;
        }
        public override void Exit()
        {
            AnimationPlayer.AnimationFinished -= EndAttack;
            _attacking = false;
            HurtBox.Monitoring = false;
            base.Exit();
        }

        public override State Process(double delta)
        {
            Player.Velocity -= Player.Velocity * (float)_decelerateSpeed * (float)delta;

            if (_attacking == false)
            {
                if (Player.Direction == Vector2.Zero)
                {
                    return Idle;
                }
                return Walk;
            }
            return null;
        }
        public override State Physics(double delta)
        {
            return null;
        }
        public override State HandleInput(InputEvent @event)
        {
            return null;
        }

        public void EndAttack(StringName newAnimName)
        {
            _attacking = false;
        }
        private async Task Delay(double seconds)
        {
            await ToSignal(GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
        }

    }
}

