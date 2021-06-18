using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

namespace Breakout
{
    public class GameState : State
    {
        public enum StateID
        {
            NEW_GAME,
            GENERATING,
            PUSHBALL,
            PLAYING,
            WIN,
            LOSE,
        };

        public GameState(StateID id,
            DelegateOnEnter onEnter,
            DelegateOnEnter onExit = null,
            DelegateOnEnter onUpdate = null,
            DelegateOnEnter onFixedUpdate = null) : base((int)id, onEnter, onExit, onUpdate, onFixedUpdate)
        {
        }

    }
}