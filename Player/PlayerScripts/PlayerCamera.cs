using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class PlayerCamera : Camera2D
    {
        public override void _Ready()
        {
            GlobalLevelManager globalLevelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            globalLevelManager.TileMapBoundsChanged += UpdateLimit;
            UpdateLimit(globalLevelManager.CurrentTileMapBounds);
            base._Ready();
        }

        public void UpdateLimit(Array<Vector2> bounds)
        {
            if (bounds == null || bounds.Count == 0)
            {
                return;
            }
            LimitLeft = (int)bounds[0].X;
            LimitRight = (int)bounds[1].X;
            LimitTop = (int)bounds[0].Y;
            LimitBottom = (int)bounds[1].Y;
        }
    }
}

