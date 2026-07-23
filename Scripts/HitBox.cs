using Godot;
using System;

namespace Delve.Scripts
{
    public partial class HitBox : Area2D
    {
        [Signal]
        public delegate void DamagedEventHandler(HurtBox hurtBox);
        
        public void TakeDamage(HurtBox hurtBox)
        {
            GD.Print("Took Damage: ",hurtBox.Damage);
            EmitSignal(SignalName.Damaged, hurtBox);
        }
    }
}

