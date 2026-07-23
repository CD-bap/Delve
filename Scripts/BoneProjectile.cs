using Godot;
using System;

namespace Delve.Scripts
{
    public partial class BoneProjectile : RigidBody2D
    {
        [Export] public int Speed { get; set; } = 175; // Adjust speed as needed.
        [Export] public Vector2 Direction { get; set; } = Vector2.Zero;
        private AnimationPlayer _animationPlayer;
        
        private const int WALL_COLLISION_LAYER = 5;

        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("shoot");
            GravityScale = 0;
            
            base._Ready();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Direction != Vector2.Zero)
            {
                Vector2 velocity = Direction * (float)(Speed * delta);
                Position += velocity;
            }
        }
        


    }
}

