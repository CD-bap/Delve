using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class PlayerHud : CanvasLayer
    {
        private Array<HeartGui> _hearts = [];
        
        private HFlowContainer _hFlowContainer;
        public override void _Ready()
        {
            _hFlowContainer = GetNode<HFlowContainer>("Control/HFlowContainer");
            if (_hFlowContainer == null)
            {
                GD.PrintErr("HFlowContainer not found in PlayerHUD!");
                return;
            }

            foreach (var c in _hFlowContainer.GetChildren())
            {
                if (c is HeartGui h)
                {
                    _hearts.Add(h);
                    h.Visible = false;
                }
            }
            base._Ready();
        }

        public void UpdateHealth(int hp, int maxHp)
        {
            UpdateMaxHp(maxHp);
            for (int i = 0; i < maxHp; i++)
            {
                UpdateHeart(i, hp);
            }
        }

        public void UpdateHeart(int index, int hp)
        {
            int value = Mathf.Clamp(hp - index * 2, 0, 2);
            _hearts[index].Value = value;
        }

        public void UpdateMaxHp(int maxHp)
        {
            int heartCount = Mathf.RoundToInt(maxHp * 0.5);
            for (int i = 0; i < _hearts.Count; i++)
            {
                if (i < heartCount)
                {
                    _hearts[i].Visible = true;
                }else
                {
                    _hearts[i].Visible = false;
                }
            }
        }
    }
}

