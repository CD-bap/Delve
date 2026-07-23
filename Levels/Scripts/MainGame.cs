using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delve.Scripts
{
    public partial class MainGame : Node2D
    {
       
        [Export] private PackedScene StarterRoomScene;
        [Export] private PackedScene[] BaseRooms;
        [Export] private PackedScene[] LongRooms;
        [Export] private PackedScene[] BigRooms;
        
        private Dictionary<Vector2, Node2D> _generatedRooms = new Dictionary<Vector2, Node2D>();
        private Vector2 _gridSize = new Vector2(960, 540); // Room size in pixels
        
        public override void _Ready()
        {
            this.YSortEnabled = true;
            GlobalPlayerManager.SetAsParent(this);
            GenerateDungeon();
        }
        
        public void GenerateDungeon()
        {
            // Clear previous generation if any
            foreach (Node child in GetChildren())
            {
                child.QueueFree();
            }
            _generatedRooms.Clear();
            
            // Create starting room at (0, 0)
            var startRoom = StarterRoomScene.Instantiate<Node2D>();
            AddChild(startRoom);
            _generatedRooms[Vector2.Zero] = startRoom;
            
            // Generate rooms recursively
            GenerateRoomsFrom(startRoom, Vector2.Zero,0,8);
        }
        
        private void GenerateRoomsFrom(Node2D room, Vector2 gridPosition, int currentDepth, int maxDepth)
        {
            try
            {
                if (currentDepth >= maxDepth)
                    return;
                // Get all door markers in this room
                var doorsNode = room.GetNodeOrNull<Node>("Doors");
                if (doorsNode == null)
                {
                    GD.PrintErr($"Room '{room.Name}' is missing a 'Doors' node.");
                    return;
                }
                var doors = doorsNode.GetChildren();
            
                foreach (Marker2D door in doors)
                {
                    // Determine direction and new grid position
                    Vector2 direction = GetDirectionFromDoorName(door.Name);
                    Vector2 newGridPos = gridPosition + direction;
                
                    // Skip if position already occupied
                    if (_generatedRooms.ContainsKey(newGridPos)) continue;
                
                    // Randomly decide whether to spawn a room (70% chance)
                    if (GD.Randf() > 0.7f) continue;
                
                    // Select random room type
                    PackedScene[] roomPool = GetRandomRoomPool();
                    if (roomPool.Length == 0) continue;
                
                    PackedScene roomScene = roomPool[GD.Randi() % roomPool.Length];
                
                    // Create and position the new room
                    var newRoom = roomScene.Instantiate<Node2D>();
                    newRoom.Position = newGridPos * _gridSize;
                    AddChild(newRoom);
                    _generatedRooms[newGridPos] = newRoom;
                
                    // Align doors between rooms
                    AlignDoors(room, door, newRoom, direction);
                
                    // Recursively generate from this new room
                    GenerateRoomsFrom(newRoom, newGridPos,currentDepth + 1,maxDepth);
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error during room generation: {ex}");
            }
            
        }
        
        private Vector2 GetDirectionFromDoorName(string doorName)
        {
            if (doorName.Contains("North")) return new Vector2(0, -1);
            if (doorName.Contains("South")) return new Vector2(0, 1);
            if (doorName.Contains("East")) return new Vector2(1, 0);
            if (doorName.Contains("West")) return new Vector2(-1, 0);
            return Vector2.Zero;
        }
        
        private PackedScene[] GetRandomRoomPool()
        {
            float roll = GD.Randf();
            
            if (roll < 0.6f) return BaseRooms;       // 60% chance for base rooms
            if (roll < 0.9f) return LongRooms;       // 30% chance for long rooms
            return BigRooms;                         // 10% chance for big rooms
        }
        
        private void AlignDoors(Node2D roomA, Marker2D doorA, Node2D roomB, Vector2 direction)
        {
            string oppositeDir = GetOppositeDirection(doorA.Name);
            if (string.IsNullOrEmpty(oppositeDir))
            {
                GD.PrintErr($"Invalid door name: '{doorA.Name}'");
                return;
            }

            // Try to find any door in the opposite direction
            var doorsNode = roomB.GetNodeOrNull<Node>("Doors");
            if (doorsNode == null)
            {
                GD.PrintErr($"Room '{roomB.Name}' is missing door '{oppositeDir}'");
                return;
            }

            foreach (Node child in doorsNode.GetChildren())
            {
                if (child is Marker2D doorB && doorB.Name.ToString().Contains(oppositeDir.Split('_')[1]))
                {
                    Vector2 offset = doorA.GlobalPosition - doorB.GlobalPosition;
                    roomB.Position += offset;
                    break; // Align with the first matching door found
                }
            }
        }
        
        private string GetOppositeDirection(string directionName)
        {
            var parts = directionName.Split('_');
            if (parts.Length < 2)
            {
                GD.PrintErr($"Invalid door name: '{directionName}'");
                return "";
            }

            string baseDir = new string(parts[1].Where(char.IsLetter).ToArray());

            return $"Door_{GetOppositeDirName(baseDir)}";
        }

        private string GetOppositeDirName(string dir)
        {
            switch (dir)
            {
                case "North": return "South";
                case "South": return "North";
                case "East": return "West";
                case "West": return "East";
                default: return "";
            }
        }
            
           
           
    }
}

