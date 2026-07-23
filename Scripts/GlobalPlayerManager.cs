using Godot;
using System;

namespace Delve.Scripts
{
    public partial class GlobalPlayerManager : Node
    {
        public static Player Player;
        public static PackedScene PlayerScene;

        public static bool PlayerSpawned  = false;


        public override async void _Ready()
        {
            PlayerScene = GD.Load<PackedScene>("res://Scenes/player.tscn");
            AddPlayerInstance();

            await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
            base._Ready();
        }
        

        public void AddPlayerInstance()
        {
            Player = PlayerScene.Instantiate<Player>();
            AddChild(Player);
        }

        public void SetPlayerPosition(Vector2 newPosition)
        {
            if (IsInstanceValid(Player))
            {
                Player.GlobalPosition = newPosition;
            }
            else
            {
                GD.PrintErr("Player node is not valid.");
            }

        }

        public static void SetAsParent(Node2D p)
        {
            if (Player.GetParent() != null)
            {
                Player.GetParent().RemoveChild(Player);
            }
            p.AddChild(Player);
        }

        public static void UnparentPlayer(Node2D p)
        {
            p.RemoveChild(Player);
        }
    }
}

