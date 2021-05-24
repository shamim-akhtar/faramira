using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tetris
{
    public class Block : MonoBehaviour
    {
        public Vector3 mRotationPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public float mPrevTime = 0.0f;

        //We will set the board that is holding these tetrominoes.
        public Board mBoard;

        public bool ValidMove()
        {
            foreach (Transform child in transform)
            {
                int x = Mathf.RoundToInt(child.transform.position.x);
                int y = Mathf.RoundToInt(child.transform.position.y);

                if (x < 0 || x >= Board.width || y < 0 || y >= Board.height)
                {
                    return false;
                }

                if(mBoard.mGrid[x,y] != null)
                {
                    return false;
                }
            }
            return true;
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}