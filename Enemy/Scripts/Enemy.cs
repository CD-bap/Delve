using Godot;
using System;

namespace Delve.Scripts
{
    public partial class Enemy : CharacterBody2D
    {
        [Signal] public delegate void DirectionChangedEventHandler(Vector2 newDirection);
        [Signal] public delegate void EnemyDamagedEventHandler(HurtBox hurtBox);

        [Signal]
        public delegate void EnemyDestroyedEventHandler(HurtBox hurtBox);
        
        public readonly Vector2[] Dir4 = { Vector2.Right, Vector2.Down, Vector2.Left, Vector2.Up };

        [Export] private int _hp = 5;
        
        private Vector2 _cardinalDirection = Vector2.Down;
        public Vector2 Direction = Vector2.Zero;
        public Player Player;
        public bool Invulnerable = false;
        
        public AnimationPlayer AnimationPlayer;
        private Sprite2D _sprite;
        private HitBox _hitBox;
        private EnemyStateMachine _stateMachine;
        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _sprite = GetNode<Sprite2D>("Sprite2D");
            _hitBox = GetNode<HitBox>("HitBox");
            _stateMachine = GetNode<EnemyStateMachine>("EnemyStateMachine");
            _stateMachine.Initialize(this);
            Player = GlobalPlayerManager.Player;
            _hitBox.Damaged += TakeDamage;
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            MoveAndSlide();
            base._PhysicsProcess(delta);
        }
        
        public bool SetDirection(Vector2 newDirection)
        {
            Direction = newDirection;
            if (Direction == Vector2.Zero)
            {
                return false;
            }
            int directionId = (int)Mathf.Round(((Direction + _cardinalDirection * (float)0.1).Angle() / Mathf.Tau) * Dir4.Length);
            directionId = Mathf.PosMod(directionId, Dir4.Length);

            Vector2 newDir = Dir4[directionId];

            if (newDir == _cardinalDirection)
            {
                return false;
            }
            _cardinalDirection = newDir;
            EmitSignal(SignalName.DirectionChanged, newDir);

            if (_cardinalDirection == Vector2.Left)
            {
                var scale = _sprite.Scale;
                scale.X = -1;
                _sprite.Scale = scale;
            }
            else
            {
                var scale = _sprite.Scale;
                scale.X = 1;
                _sprite.Scale = scale;
            }
            return true;
        }
        
        public void UpdateAnimation(string state)
        {  
            AnimationPlayer.Play(state + "_" + AnimDirection());
        }
        public string AnimDirection()
        {
            if (_cardinalDirection == Vector2.Down)
            {
                return "down";
            }

            if (_cardinalDirection == Vector2.Up)
            {
                return "up";
            }

            return "side";
        }

        public void TakeDamage(HurtBox hurtBox)
        {
            if(Invulnerable) return;
            _hp -= hurtBox.Damage;
            if (_hp > 0)
            {
                EmitSignal(SignalName.EnemyDamaged, hurtBox);
                
            }
            else
            {
                EmitSignal(SignalName.EnemyDestroyed, hurtBox);
            }
            
        }
    }
}

