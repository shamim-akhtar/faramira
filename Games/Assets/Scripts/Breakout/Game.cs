using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

namespace Breakout
{
    public class Game : MonoBehaviour
    {
        FiniteStateMachine mFsm = new FiniteStateMachine();
        private void Awake()
        {
            Physics2D.gravity = new Vector2(0.0f, -10.0f);
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            mFsm.Update();
        }

        public void SetState(GameState.StateID stateId)
        {
            mFsm.SetCurrentState((int)stateId);
        }
    }

}
