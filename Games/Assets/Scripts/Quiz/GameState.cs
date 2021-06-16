using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

namespace Trivia
{
    public class GameState : State
    {
        public enum StateID
        {
            NEW_QUESTION,
            TIMER,
            NO_RESPONSE,
            RESPONSE,
            SHOW_RESULTS,
            SHOW_FEEDBACK,
            EXIT,
        };
        //protected BoardMemory mBoard;
        //protected FiniteStateMachine mFsm;
        public GameState(StateID id,
            DelegateOnEnter onEnter,
            DelegateOnEnter onExit = null,
            DelegateOnEnter onUpdate = null,
            DelegateOnEnter onFixedUpdate = null) : base((int)id, onEnter, onExit, onUpdate, onFixedUpdate)
        {
            //ID = (int)id;
            //OnEnter = onEnter;
            //OnExit = onExit;
            //OnUpdate = onUpdate;
            //OnFixedUpdate = onFixedUpdate;
        }

        //// For this tutorial we will use delegates.
        //public delegate void DelegateOnEnter();
        //public DelegateOnEnter OnEnter;
        //public delegate void DelegateOnExit();
        //public DelegateOnEnter OnExit;
        //public delegate void DelegateOnUpdate();
        //public DelegateOnEnter OnUpdate;
        //public delegate void DelegateOnFixedUpdate();
        //public DelegateOnEnter OnFixedUpdate;

        //public override void Enter()
        //{
        //    OnEnter?.Invoke();
        //}
        //public override void Exit()
        //{
        //    OnExit?.Invoke();
        //}
        //public override void Update()
        //{
        //    OnUpdate?.Invoke();
        //}
        //public override void FixedUpdate()
        //{
        //    OnFixedUpdate?.Invoke();
        //}
    }
}
