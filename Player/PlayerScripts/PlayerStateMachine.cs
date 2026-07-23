using Godot;
using System;
using Godot.Collections;

namespace Delve.Scripts
{
    public partial class PlayerStateMachine : Node
    {
        private Array<State> _states;
        private State _pervState;
        private State _currentState;


        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Disabled;
            base._Ready();
        }

        public override void _Process(double delta)
        {
            ChangeState(_currentState.Process(delta));
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            ChangeState(_currentState.Physics(delta));
            base._PhysicsProcess(delta);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            ChangeState(_currentState.HandleInput(@event));
            base._UnhandledInput(@event);
        }

        public void Initialize(Player player)
        {
            _states = [];
            foreach(Node c in GetChildren())
            {
                if (c is State state)
                {
                    state.Player = player;
                    state.StateMachine = this;
                    _states.Add(state);
                    if (c.Name == "Idle")
                    {
                        var stateIdle = state as StateIdle;
                        stateIdle.Walk = GetNode<StateWalk>("Walk");

                        // Assign _attack for Attack state in StateIdle
                        stateIdle.Attack = GetNode<StateAttack>("Attack");

                    }
                    if (c.Name == "Walk")
                    {
                        var stateWalk = state as StateWalk;
                        stateWalk.Idle = GetNode<StateIdle>("Idle");
                        stateWalk.Attack = GetNode<StateAttack>("Attack");
                    }

                    if (c.Name == "Attack")
                    {
                        var stateAttack = state as StateAttack;
                        stateAttack.AnimationPlayer = player.GetNode<AnimationPlayer>("AnimationPlayer");
                        stateAttack.AttackAnimPlayer = player.GetNode<AnimationPlayer>("Sprite2D/AttackEffectSprite/AnimationPlayer");
                        stateAttack.Audio = player.GetNode<AudioStreamPlayer2D>("Audio/AudioStreamPlayer2D");
                        stateAttack.HurtBox = player.GetNode<HurtBox>("Sprite2D/HurtBox");
                        stateAttack.Idle = GetNode<StateIdle>("Idle");
                        stateAttack.Walk = GetNode<StateWalk>("Walk");
                    }

                    if (c.Name == "Stun")
                    {
                        var stateStun = state as StateStun;
                        stateStun.Idle = GetNode<StateIdle>("Idle");
                    }
                    
                }
            }

            if (_states.Count == 0)
            {
                return;
            }
            
            _states[0].Player = player;
            _states[0].StateMachine = this;

            foreach (var s in _states)
            {
                s.Init();
            }
            
            ChangeState(_states[0]);
            ProcessMode = ProcessModeEnum.Inherit;
        }

        public void ChangeState(State newState)
        {
            if (newState == null || newState == _currentState)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.Exit();
            }
            _pervState = _currentState;
            _currentState = newState;
            _currentState.Enter();
        }
    }
}

