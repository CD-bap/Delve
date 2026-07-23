using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class LevelTileMap : TileMapLayer
    {
        public override void _Ready()
        {
            GlobalLevelManager globalLevelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            globalLevelManager.ChangeTileMapBounds(GetTilemapBounds());
            base._Ready();
        }

        public Array<Vector2> GetTilemapBounds()
        {
            var bounds = new Array<Vector2>();
            bounds.Add(GetUsedRect().Position * RenderingQuadrantSize);
            bounds.Add(GetUsedRect().End * RenderingQuadrantSize);
            
            return bounds;
        }
    }
}

