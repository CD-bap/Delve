using Godot;
using System;

namespace Delve.Scripts
{
    public partial class HurtBox : Area2D
    {
        [Export] public int Damage = 1;
        public override void _Ready()
        {
            AreaEntered += Area_Entered;
            base._Ready();
        }
        
        public void Area_Entered(Area2D area)
        {
            if (area is HitBox hitBox)
            {
                hitBox.TakeDamage(this);
            }
        }
    }
}

