using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tetris
{
    public class Block : MonoBehaviour
    {
        public Vector3 mRotationPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public float mFallTime = 0.8f;
        float mPrevTime = 0.0f;

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
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(-1.0f, 0.0f, 0.0f);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += new Vector3(1.0f, 0.0f, 0.0f);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(1.0f, 0.0f, 0.0f);
                }
            }

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.RotateAround(transform.TransformPoint(mRotationPoint), new Vector3(0.0f, 0.0f,1.0f), 90.0f);
                if (!ValidMove())
                {
                    transform.RotateAround(transform.TransformPoint(mRotationPoint), new Vector3(0.0f, 0.0f, 1.0f), -90.0f);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                mFallTime = 10;
            }

            float fallTime = mFallTime;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                fallTime /= 10.0f;
            }
            if (Time.time - mPrevTime > fallTime)
            {
                transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                if (!ValidMove())
                {
                    transform.position -= new Vector3(0.0f, -1.0f, 0.0f);
                    this.enabled = false;
                    mBoard.AddToGrid(this);
                    mBoard.CheckAndRemoveLine();
                    mBoard.InstantiateRandomizeBlock();
                }
                mPrevTime = Time.time;
            }
        }
    }
}