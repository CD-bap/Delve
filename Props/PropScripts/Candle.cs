using Godot;
using System;

namespace Delve.Scripts
{
    public partial class Candle : Node2D
    {
        public override void _Ready()
        {
            HitBox hitBox = GetNode<HitBox>("HitBox"); 
            hitBox.Damaged += TakeDamage;
            

            base._Ready();
        }

        public void TakeDamage(HurtBox hurtBox)
        {
            GD.Print("Took Damage: ", hurtBox.Damage);
            QueueFree();
        }
    }
}

