using Godot;
using System;

namespace Delve.Scripts
{
    public partial class EnemyStateAggro : EnemyState
    {
        [Export] private string _animName = "walk";
        [Export] private float _aggroSpeed = 50.0f;
        [Export] public float AggroRadius = 100.0f;
        [Export] private EnemyState _nextState;

        private Player _targetPlayer;
        
        private float _orbitAngle = 0.0f;
        private float _orbitSpeed = 2.0f;
        
        [Export] private float _shotCooldown = 1.5f;
        private float _currentCooldown = 0.0f; //



        public override void Init()
        {
            base.Init();
            _targetPlayer = GlobalPlayerManager.Player; 
        }

        public override void Enter()
        {
            GD.Print($"{Enemy.Name} entering Aggro State");
            Enemy.UpdateAnimation(_animName);
        }

        public override void Exit()
        {
            GD.Print($"{Enemy.Name} exiting Aggro State");
            Enemy.Velocity = Vector2.Zero;
        }

        public override EnemyState Process(double delta)
        {
            if (_targetPlayer != null && Enemy.Position.DistanceTo(_targetPlayer.Position) <= AggroRadius)
            {
                if (Enemy.Name.ToString().Contains("Skeleton"))
                {
                    AggroRadius = 300.0f;
                    RangedOrbitBehavior(delta);
                }
                else if (Enemy.Name.ToString().Contains("Slime"))
                {
                    AggroRadius = 500.0f;
                    SlimeBehavior(delta);
                }
                else if (Enemy.Name == "Boss")
                {
                    AggroRadius = 1000.0f;
                }
                else
                {
                    ChasePlayer(delta);
                }

            }
            else
            {
                return _nextState;
            }
            if (Enemy.Position.DistanceTo(GlobalPlayerManager.Player.Position) > AggroRadius)
            {
                return StateMachine.PervState;
            }

            return null;
        }

        public override EnemyState Physics(double delta)
        {
            return null;
        }

        private void ChasePlayer(double delta)
        {
            Vector2 direction = (_targetPlayer.Position - Enemy.Position).Normalized();
            Enemy.Velocity = direction * _aggroSpeed;
            Enemy.SetDirection(direction);
            
            Enemy.UpdateAnimation(_animName);
        }
        
        private void RangedOrbitBehavior(double delta)
        {
            // Calculate a position on the orbiting circle
            _orbitAngle += _orbitSpeed * (float)delta; // Increment angle for circling
            if (_orbitAngle > MathF.Tau) _orbitAngle -= MathF.Tau; // Limit angle to within 0 - 2π

            // Calculate offset for orbiting position
            Vector2 orbitOffset = new Vector2(
                AggroRadius * MathF.Cos(_orbitAngle),
                AggroRadius * MathF.Sin(_orbitAngle)
            );

            // Target position for the skeleton is the orbit position around the player
            Vector2 targetPosition = _targetPlayer.Position + orbitOffset;

            // Move toward the orbiting position smoothly
            Vector2 direction = (targetPosition - Enemy.Position).Normalized();
            Enemy.Velocity = direction * _aggroSpeed;

            // Update direction and animation
            Enemy.SetDirection(direction);
            Enemy.UpdateAnimation(_animName);

            // Handle shooting logic
            HandleShooting(delta);
        }

        private void HandleShooting(double delta)
        {
            // Reduce cooldown timer
            _currentCooldown -= (float)delta;

            // If cooldown is finished, shoot a bone
            if (_currentCooldown <= 0)
            {
                ShootBone();
                _currentCooldown = _shotCooldown; // Reset cooldown timer
            }
        }

        private void ShootBone()
        {
            // Load the bone projectile PackedScene
            PackedScene boneScene = ResourceLoader.Load<PackedScene>("res://Scenes/bone.tscn");
            if (boneScene == null)
            {
                GD.PrintErr("Failed to load bone projectile scene. Check the file path.");
                return;
            }

            // Instantiate the bone projectile
            BoneProjectile bone = boneScene.Instantiate<BoneProjectile>();

            // Add the bone projectile to the scene tree
            GetParent().AddChild(bone);

            // Set the bone's position to this enemy’s position
            bone.GlobalPosition = Enemy.GlobalPosition;

            // Get the Player instance via the GlobalPlayerManager
            Player player = GlobalPlayerManager.Player;

            if (player == null)
            {
                GD.PrintErr("Player instance is null. Ensure it has been spawned by the PlayerSpawn system.");
                return;
            }

            // Calculate the direction toward the player
            Vector2 directionToPlayer = (player.GlobalPosition - Enemy.GlobalPosition).Normalized();
            bone.Direction = directionToPlayer;

            // Optionally adjust other properties of the bone projectile, such as speed
            GD.Print($"Shooting bone projectile toward player at {player.GlobalPosition}!");
        }


        
        private double _actionTimer = 0;
        private bool _isCharging = false;
        private bool _isCooldown = false;
        private void SlimeBehavior(double delta)
        {
            double chargeTime = GD.RandRange(1.0f, 2.5f);
            double launchCooldown = GD.RandRange(0.5f, 1.5f);
            
            if (!_isCharging && !_isCooldown)
            {
                _isCharging = true;
                _actionTimer = chargeTime; 
                Enemy.Velocity = Vector2.Zero;
                Enemy.UpdateAnimation("idle");
            }
            else if (_isCharging)
            {
                _actionTimer -= (float)delta;
                if (_actionTimer <= 0)
                {
                    _isCharging = false;
                    LaunchAtPlayer();
                    _isCooldown = true;
                    _actionTimer = launchCooldown;
                }
            }
            else if (_isCooldown)
            {
                _actionTimer -= (float)delta;
                if (_actionTimer <= 0)
                {
                    _isCooldown = false;
                }
            }
        }
        
        private void LaunchAtPlayer()
        {
            Vector2 direction = (_targetPlayer.Position - Enemy.Position).Normalized();
            
            float launchSpeed = 200.0f;
            Enemy.Velocity = direction * launchSpeed;
            
            Enemy.UpdateAnimation("walk");
        }
    }
}
