using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakout
{
    public class FrameBottom : MonoBehaviour
    {
        public Game mBreakout;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("You have lost the game");
            mBreakout.SetState(GameState.StateID.LOSE);
        }
    }
}
