using Godot;
using System;

namespace Delve.Scripts
{
    public partial class PlayerSpawn : Node2D
    {
        public override void _Ready()
        {
            Visible = false;
            GlobalPlayerManager playerManager = GetNode<GlobalPlayerManager>("/root/GlobalPlayerManager");

            if (GlobalPlayerManager.PlayerSpawned == false)
            {
                playerManager.SetPlayerPosition(GlobalPosition);
                GlobalPlayerManager.PlayerSpawned = true;
            }
            base._Ready();
        }
    }
}

