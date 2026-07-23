using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class GlobalLevelManager : Node
    {
        [Signal] public delegate void LevelLoadStartedEventHandler();
        [Signal] public delegate void LevelLoadedEventHandler();
        [Signal] public delegate void TileMapBoundsChangedEventHandler(Array<Vector2> bounds);
        
        public Array<Vector2> CurrentTileMapBounds;
        public string targetTransition;
        public Vector2 positionOffset;
        
        

        public async override void _Ready()
        {
            await ToSignal(GetTree(), "process_frame");
            EmitSignal(SignalName.LevelLoaded);
            base._Ready();
        }

        public void ChangeTileMapBounds(Array<Vector2> bounds)
        {
            CurrentTileMapBounds = bounds;
            EmitSignal(SignalName.TileMapBoundsChanged, bounds);
        }

        public async void LoadNewLevel(string levelPath, string _targetTransition, Vector2 _positionOffset)
        {
            GetTree().Paused = true;
            targetTransition = _targetTransition;
            positionOffset = _positionOffset;
            
            await ToSignal(GetTree(), "process_frame");
            
            EmitSignal(SignalName.LevelLoadStarted);

            await ToSignal(GetTree(), "process_frame");

            GetTree().ChangeSceneToFile(levelPath);
            
            await ToSignal(GetTree(), "process_frame");
            
            GetTree().Paused = false;
            
            await ToSignal(GetTree(), "process_frame");
            
            EmitSignal(SignalName.LevelLoaded);
        }
    }
}

