using Godot;
using System;



namespace Delve.Scripts
{
    [Tool]
    public partial class LevelTransition : Area2D
    {
        public enum SIDE { LEFT, RIGHT, TOP, BOTTOM }
        
        [Export(PropertyHint.File, "*.tscn")] 
        public string Level { get; set; } = "";

        [Export] public string targetTransitionArea = "LevelTransition";

        [ExportCategory("Collision Area Settings")] 
        [Export(PropertyHint.Range, "1,12,1,or_greater")] private int _size = 2;
        
        public int Size
        {
            get => _size; 
            set
            {
                _size = value; 
                UpdateArea(); 
            }
        }

        
        [Export] SIDE _side = SIDE.LEFT;
        public SIDE Side
        {
            get => _side;
            set
            {
                _side = value;
                UpdateArea();
            }
        }
        
        [Export] bool snapToGrid = false;
        
        public CollisionShape2D _collisionShape;

        public async override void _Ready()
        {
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
            GlobalLevelManager levelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            if (Engine.IsEditorHint())
            {
                return;
            }

            Monitoring = false;
            PlacePlayer();

            await ToSignal(levelManager, "LevelLoaded");
            
            Monitoring = true;
            
            BodyEntered += PlayerEntered;
            base._Ready();
        }

        public void PlayerEntered(Node2D p)
        {
            GlobalLevelManager levelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            levelManager.LoadNewLevel(Level, targetTransitionArea, GetOffset());
        }

        public void PlacePlayer()
        {
            GlobalLevelManager levelManager = GetNode<GlobalLevelManager>("/root/GlobalLevelManager");
            GlobalPlayerManager playerManager = GetNode<GlobalPlayerManager>("/root/GlobalPlayerManager");
            if (Name != levelManager.targetTransition)
            {
                return;
            }
            playerManager.SetPlayerPosition(GlobalPosition + levelManager.positionOffset);
        }

        public Vector2 GetOffset()
        {
            Vector2 offset = Vector2.Zero;
            var playerPosition = GlobalPlayerManager.Player.GlobalPosition;

            if (_side == SIDE.LEFT || _side == SIDE.RIGHT)
            {
                offset.Y = playerPosition.Y - GlobalPosition.Y;
                offset.X = 16;
                if (_side == SIDE.LEFT)
                {
                    offset.X *= -1;
                }
            }
            else
            {
                offset.X = playerPosition.X - GlobalPosition.X;
                offset.Y = 16;
                if (_side == SIDE.TOP)
                {
                    offset.Y *= -1;
                }
            }
            
            
            return offset;
        }
        public void UpdateArea()
        {
            Vector2 newRect = new Vector2(16, 16);
            Vector2 newPosition = Vector2.Zero;

            if (_side == SIDE.TOP)
            {
                newRect.X *= _size;
                newPosition.Y -= 8;
            }else if (_side == SIDE.BOTTOM)
            {
                newRect.X *= _size;
                newPosition.Y += 8;
            }else if (_side == SIDE.LEFT)
            {
                newRect.Y *= _size;
                newPosition.X -= 8;
            }
            else if(_side == SIDE.RIGHT)
            {
                newRect.Y *= _size;
                newPosition.X += 8;
            }

            if (_collisionShape == null)
            {
                _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
            }
            
            if (_collisionShape.Shape is RectangleShape2D rectangle)
            {
                var newShape = (RectangleShape2D)rectangle.Duplicate(); // Duplicate the shape
                newShape.Size = newRect; // Apply the new size
                _collisionShape.Shape = newShape; // Assign the new shape back to the collision node
            }
            else
            {
                GD.PrintErr("CollisionShape2D must be of type RectangleShape2D.");
            }

            // Update position
            _collisionShape.Position = newPosition;




        }
    }
}

