using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;

namespace Tetris
{ 
    public class Board : MonoBehaviour
    {
        public static int width = 10;
        public static int height = 20;
        public float mFallTime = 1.2f;
        public int mLevel = 1;
        public Transform mSpawnPoint;

        public GameObject[] mBlockPrefabs;

        private int mLinesRemoved = 0;
        private float mLastLineRemovedTime = 0.0f;
        public Transform[, ] mGrid;

        public Text mLevelText;
        public Text mScoreText;
        private int mScore = 0;

        FiniteStateMachine mFsm = new FiniteStateMachine();
        Block mCurrentBlock = null;

        public void InstantiateBlock(int id)
        {
            GameObject obj = Instantiate(mBlockPrefabs[id], mSpawnPoint.position, mSpawnPoint.rotation);
            mCurrentBlock = obj.GetComponent<Block>();
            if (mCurrentBlock != null)
            {
                mCurrentBlock.mBoard = this;
                if (!mCurrentBlock.ValidMove())
                {
                    Destroy(obj);
                    Lost();
                }
            }
            mCurrentBlock.mPrevTime = Time.time;
        }

        public void InstantiateRandomizeBlock()
        {
            int index = Random.Range(0, mBlockPrefabs.Length);
            InstantiateBlock(index);
        }

        public void AddToGrid(Transform child)
        {
            child.SetParent(null, true);

            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);
            mGrid[x, y] = child;
        }

        public void AddToGrid(Block blk)
        {
            // Manual reverse iteration is possible using the GetChild() and childCount variables
            // Using normal forward traversal we cannot remove the parent.
            for (int i = blk.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = blk.transform.GetChild(i);
                child.SetParent(null, true);

                int x = Mathf.RoundToInt(child.transform.position.x);
                int y = Mathf.RoundToInt(child.transform.position.y);
                mGrid[x, y] = child;
            }
        }

        void UpdateScore()
        {
            mScore += 20;
            if(mLastLineRemovedTime < 10)
            {
                mScore += 100;
            }
            else if (mLastLineRemovedTime < 20)
            {
                mScore += 80;
            }
            else if (mLastLineRemovedTime < 40)
            {
                mScore += 40;
            }
            else if (mLastLineRemovedTime < 60)
            {
                mScore += 20;
            }
            else
            {
                mScore += 10;
            }
            mScoreText.text = mScore.ToString();
        }

        void Lost()
        {
            mFsm.SetCurrentState((int)GameState.StateID.LOST);
        }

        void LevelUp()
        {
            //Debug.Log("Completed Level: " + mLevel + ". Leveling up.");
            mFallTime *= 0.75f;
            mLevel += 1;
            mLinesRemoved = 0;
            mLastLineRemovedTime = 0.0f;
            mLevelText.text = mLevel.ToString();
        }

        void Start()
        {
            mGrid = new Transform[width, height];
            mLevelText.text = mLevel.ToString();


            mFsm.Add(
                new GameState(
                    GameState.StateID.NEW_LEVEL,
                    OnEnterNewLevel,
                    OnExitNewLevel,
                    OnUpdateNewLevel)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.CLEAR_LINES,
                    OnEnterClearLines,
                    OnExitClearLines,
                    OnUpdateClearLines)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.PLAYING,
                    OnEnterPlaying,
                    OnExitPlaying,
                    OnUpdatePlaying)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.LOST,
                    OnEnterLost,
                    OnExitLost,
                    OnUpdateLost)
                );

            mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
        }

        void Update()
        {
            mFsm.Update();
        }

        void OnEnterLost()
        {
            Debug.Log("Game ended");
        }

        void OnExitLost()
        {

        }

        void OnUpdateLost()
        {

        }

        IEnumerator Coroutine_ClearBoard()
        {
            // clear board.
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    if (mGrid[i, j] != null)
                    {
                        Destroy(mGrid[i, j].gameObject);
                    }
                    mGrid[i, j] = null;
                    yield return null;
                }
            }
            LevelUp();
            mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
        }
        void OnEnterNewLevel()
        {
            StartCoroutine(Coroutine_ClearBoard());
        }
        void OnExitNewLevel()
        {

        }
        void OnUpdateNewLevel()
        {

        }

        void OnEnterPlaying()
        {
            //Debug.Log("OnEnterPlaying");
            InstantiateRandomizeBlock();
        }
        void OnExitPlaying()
        {
        }
        void OnUpdatePlaying()
        {
#if TESTING
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                InstantiateBlock(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InstantiateBlock(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InstantiateBlock(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InstantiateBlock(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                InstantiateBlock(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                InstantiateBlock(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                InstantiateBlock(6);
            }
#endif
            if (mCurrentBlock == null)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                mCurrentBlock.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
                if (!mCurrentBlock.ValidMove())
                {
                    mCurrentBlock.transform.position -= new Vector3(-1.0f, 0.0f, 0.0f);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                mCurrentBlock.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
                if (!mCurrentBlock.ValidMove())
                {
                    mCurrentBlock.transform.position -= new Vector3(1.0f, 0.0f, 0.0f);
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                mCurrentBlock.transform.RotateAround(
                    mCurrentBlock.transform.TransformPoint(mCurrentBlock.mRotationPoint), 
                    new Vector3(0.0f, 0.0f, 1.0f), 
                    90.0f
                );

                if (!mCurrentBlock.ValidMove())
                {
                    mCurrentBlock.transform.RotateAround(
                        mCurrentBlock.transform.TransformPoint(mCurrentBlock.mRotationPoint), 
                        new Vector3(0.0f, 0.0f, 1.0f), 
                        -90.0f
                    );
                }
            }

            float fallTime = mFallTime;
            if (Input.GetKey(KeyCode.DownArrow))
            {
                fallTime /= 10.0f;
            }

            if (Time.time - mCurrentBlock.mPrevTime > fallTime)
            {
                mCurrentBlock.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                if (!mCurrentBlock.ValidMove())
                {
                    mCurrentBlock.transform.position -= new Vector3(0.0f, -1.0f, 0.0f);
                    mCurrentBlock.enabled = false;
                    AddToGrid(mCurrentBlock);
                    mFsm.SetCurrentState((int)GameState.StateID.CLEAR_LINES);
                }
                mCurrentBlock.mPrevTime = Time.time;
            }
        }

        public int CheckIfNeedToClearLines()
        {
            for (int j = 0; j < height; ++j)
            {
                bool removeLine = true;
                for (int i = 0; i < width; ++i)
                {
                    removeLine = removeLine && mGrid[i, j] != null;
                }
                if (removeLine)
                {
                    return j;
                }
            }
            return -1;
        }

        public bool ValidMove(Transform child)
        {
            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);

            if (x < 0 || x >= Board.width || y < 0 || y >= Board.height)
            {
                return false;
            }

            if (mGrid[x, y] != null)
            {
                return false;
            }
            return true;
        }

        void RemoveLine(int id)
        {
            for (int i = 0; i < width; ++i)
            {
                Transform t = mGrid[i, id];
                Destroy(t.gameObject);
                mGrid[i, id] = null;
            }

            // bring down all the above tiles.
            // how many blocks are there above this height.
            List<Transform> blocks = new List<Transform>();
            for (int j = id; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    if (mGrid[i, j] != null)
                    {
                        blocks.Add(mGrid[i, j]);
                        mGrid[i, j] = null;
                    }
                }
            }

            for (int i = 0; i < blocks.Count; ++i)
            {
                bool flag = true;
                while (flag)
                {
                    blocks[i].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                    if (!ValidMove(blocks[i]))
                    {
                        blocks[i].transform.position -= new Vector3(0.0f, -1.0f, 0.0f);
                        AddToGrid(blocks[i]);
                        flag = false;
                    }
                }
            }
            mLinesRemoved += 1;
            Debug.Log("mLinesRemoved: " + mLinesRemoved);
            UpdateScore();
            mLastLineRemovedTime = Time.time - mLastLineRemovedTime;
        }

        void OnEnterClearLines()
        {
            int id = CheckIfNeedToClearLines();
            //Debug.Log("OnEnterClearLines. ID: " + id);
            while (id != -1)
            {
                RemoveLine(id);
                id = CheckIfNeedToClearLines();
            }

            if (mLinesRemoved >= 10)
            {
                mFsm.SetCurrentState((int)GameState.StateID.NEW_LEVEL);
            }
            else
            {
                mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
            }
        }
        void OnExitClearLines()
        {
        }
        void OnUpdateClearLines()
        {
        }
    }
}
