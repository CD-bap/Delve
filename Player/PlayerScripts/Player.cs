using Godot;

namespace Delve.Scripts
{
    public partial class Player : CharacterBody2D
    {
        [Signal]
        public delegate void DirectionChangedEventHandler(Vector2 newDirection);
        [Signal] 
        public delegate void PlayerDamagedEventHandler(HurtBox hurtBox);
        [Signal]
        public delegate void PlayerDestroyedEventHandler(HurtBox hurtBox);
        
        
        
        public static readonly Vector2[] Dir4 = { Vector2.Right, Vector2.Down, Vector2.Left, Vector2.Up };
        
        private Vector2 _cardinalDirection = Vector2.Down;
        public Vector2 Direction = Vector2.Zero;
        
        public bool Invulnerable = false;
        [Export] private int _hp = 6;
        private int _maxHp = 6;

        
        public AnimationPlayer AnimationPlayer;
        private Sprite2D _sprite;
        private PlayerStateMachine _stateMachine;
        private HitBox _hitBox;
        public AnimationPlayer EffectAnimationPlayer;
        private Delve.Scripts.PlayerHud _playerHud;

        

        public override void _Ready()
        {
            AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _sprite = GetNode<Sprite2D>("Sprite2D");
            _stateMachine = GetNode<PlayerStateMachine>("StateMachine");
            _hitBox = GetNode<HitBox>("HitBox");
            EffectAnimationPlayer = GetNode<AnimationPlayer>("EffectAnimationPlayer");
            _playerHud = GetNode<Delve.Scripts.PlayerHud>("/root/PlayerHud");

            
            GlobalPlayerManager.Player = this;
            _stateMachine.Initialize(this);
            _hitBox.Damaged += TakeDamage;
            UpdateHp(99);
            base._Ready();
        }

        public override void _Process(double delta)
        {
            // _direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
            // _direction.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
            Direction = new Vector2(
                Input.GetAxis("left", "right"),
                Input.GetAxis("up", "down")
            ).Normalized();
            
            base._Process(delta);
            
        }

        public override void _PhysicsProcess(double delta)
        {
            MoveAndSlide();
            base._PhysicsProcess(delta);
        }

        public bool SetDirection()
        {
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
            if(Invulnerable == true) return;
            UpdateHp(-hurtBox.Damage);
            if (_hp > 0)
            {
                EmitSignal(SignalName.PlayerDamaged, hurtBox);
            }
            else
            {
                EmitSignal(SignalName.PlayerDamaged, hurtBox);
                UpdateHp(99);
            }
        }

        public void UpdateHp(int delta)
        {
            _hp = Mathf.Clamp(_hp + delta, 0, _maxHp);
            _playerHud.UpdateHealth(_hp, _maxHp);
        }

        public async void MakeInvulnerable(float duration)
        {
            Invulnerable = true;
            _hitBox.Monitoring = false;
            
            await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);
            
            Invulnerable = false;
            _hitBox.Monitoring = true;
        }
    }
}

