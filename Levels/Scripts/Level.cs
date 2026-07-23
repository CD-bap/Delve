using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class Level : Node2D
    {
        private GlobalLevelManager _globalLevelManager;
        
        public override void _Ready()
        {
            this.YSortEnabled = true;
            _globalLevelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            _globalLevelManager.LevelLoadStarted += FreeLevel;
        
            // Move this after connecting the signal to ensure proper order
            GlobalPlayerManager.SetAsParent(this);
            base._Ready();
        }

        public void FreeLevel()
        {
            if (IsInstanceValid(this))
            {
                // Disconnect the signal first to prevent multiple calls
                if (_globalLevelManager != null)
                {
                    _globalLevelManager.LevelLoadStarted -= FreeLevel;
                }
            
                // Ensure the player is unparented before freeing
                GlobalPlayerManager.UnparentPlayer(this);
                QueueFree();
            }
        }
    
        public override void _ExitTree()
        {
            // Cleanup when node is being removed
            if (_globalLevelManager != null)
            {
                _globalLevelManager.LevelLoadStarted -= FreeLevel;
            }
            base._ExitTree();
        }
    }
}