using Godot;
using System;

namespace Delve.Scripts
{
    public partial class HeartGui : Control
    {
        private Sprite2D _sprite;
        
        private int _value = 2;
        
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateSprite();
            }
        }
        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
            base._Ready();
        }

        public void UpdateSprite()
        {
            _sprite.Frame = _value;
        }
    }
}

