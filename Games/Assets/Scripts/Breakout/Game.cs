using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;
using System;

namespace Breakout
{
    public class Game : MonoBehaviour
    {
        public float MinX = -4.5f;
        public float MaxX =  4.5f;
        public float MinY = 2.0f;
        public float MaxY = 10.0f;
        public GameMenuHandler mMenuHandler;
        public GameObject[] PrefabBricks;
        public Button mPushButton;
        public Text mScoreText;
        private int mScore = 0;

        public PSManager mPSManager;

        FiniteStateMachine mFsm = new FiniteStateMachine();
        private void Awake()
        {
            Physics2D.gravity = new Vector2(0.0f, -5.0f);
        }

        public GameObject mBat;
        public GameObject mBall;

        Vector3 mBallResetPos;
        Vector3 mBatResetPos;

        float probabilityToHideBrick = 0.1f;

        private Dictionary<TwoDPoint, GameObject> mBricks = new Dictionary<TwoDPoint, GameObject>();

        void Start()
        {
            mBallResetPos = mBall.transform.position;
            mBatResetPos = mBat.transform.position;

            mFsm.Add(
                new GameState(
                    GameState.StateID.NEW_GAME,
                    OnEnterNewGame)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.GENERATING,
                    OnEnterGenerating)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.PUSHBALL,
                    OnEnterPushball)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.PLAYING,
                    OnEnterPlaying)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.WIN,
                    OnEnterWin)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.LOSE,
                    OnEnterLose)
                );
            mMenuHandler.onClickNextGame += NextGame;
            mFsm.SetCurrentState((int)GameState.StateID.NEW_GAME);
        }

        void Update()
        {
            mFsm.Update();
        }

        public void SetState(GameState.StateID stateId)
        {
            mFsm.SetCurrentState((int)stateId);
        }

        void NextGame()
        {
            if (mBricks.Count == 0)
            {
                mFsm.SetCurrentState((int)GameState.StateID.NEW_GAME);
            }
            else
            {
                SetState(GameState.StateID.PUSHBALL);
            }
        }

        #region State Machine Functions
        void OnEnterNewGame()
        {
            mFsm.SetCurrentState((int)GameState.StateID.GENERATING);
            mMenuHandler.SetActiveBtnNext(false);
        }

        void OnEnterPushball()
        {
            ShowPushButton(true);
        }

        IEnumerator Coroutine_FillUpBricks()
        {
            float posx = MinX;
            float posy = MinY;

            probabilityToHideBrick = UnityEngine.Random.Range(0.0f, 0.5f);

            int x = 0;
            int y = 0;
            for(posx = MinX; posx <= MaxX; posx += 1.5f)
            {
                for (posy = MinY; posy <= MaxY; posy += 1.0f)
                {
                    int index = UnityEngine.Random.Range(0, PrefabBricks.Length);
                    GameObject brick = Instantiate(PrefabBricks[index], new Vector3(posx, posy, 0.0f), Quaternion.identity);
                    Brick brickScript = brick.GetComponent<Brick>();

                    //float prob = UnityEngine.Random.Range(0.0f, 1.0f);
                    //if (prob < probabilityToHideBrick)
                    //{
                    //    brick.SetActive(false);
                    //}

                    TwoDPoint pt = new TwoDPoint(x, y);
                    brickScript.mPoint = pt;

                    mBricks.Add(pt, brick);
                    y += 1;
                    yield return null;
                }
                x += 1;
            }
        }

        IEnumerator Coroutine_Generating()
        {
            yield return StartCoroutine(Coroutine_FillUpBricks());
            SetState(GameState.StateID.PUSHBALL);
        }

        IEnumerator Coroutine_DestroyAllBricks()
        {
            foreach (KeyValuePair<TwoDPoint, GameObject> item in mBricks)
            {
                if(item.Value != null)
                {
                    Destroy(item.Value);
                }
                yield return null;
            }
            mBricks.Clear();
        }

        void ShowPushButton(bool flag)
        {
            mPushButton.gameObject.SetActive(flag);
        }

        void OnEnterGenerating()
        {
            StartCoroutine(Coroutine_Generating());
        }

        IEnumerator Coroutine_ResetBall()
        {
            yield return new WaitForSeconds(0.5f);
            mBall.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            mBat.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            mBat.transform.position = mBatResetPos;
            mBall.transform.position = mBallResetPos;
            yield return new WaitForSeconds(0.5f);
        }
        void OnEnterPlaying()
        {
            ShowPushButton(false);
            mMenuHandler.SetActiveBtnNext(false);
        }

        IEnumerator Coroutine_CleanBoard()
        {
            //yield return StartCoroutine(Coroutine_DestroyAllBricks());
            yield return StartCoroutine(Coroutine_ResetBall());
            mMenuHandler.SetActiveBtnNext(true);
        }

        void OnEnterWin()
        {
            StartCoroutine(Coroutine_CleanBoard());
        }

        void OnEnterLose()
        {
            StartCoroutine(Coroutine_CleanBoard());
        }
        #endregion

        #region Ball Hitting Bricks
        public void BallHitBrick(Brick brick)
        {
            //Debug.Log("Hit Brick: " + brick.mPoint.X + ", " + brick.mPoint.Y);
            mPSManager.mEfx.ShowEFX(17, brick.transform.position, 0.2f);
            mBricks.Remove(brick.mPoint);
            mScore += brick.mScoreWhenHit;
            Destroy(brick.gameObject);

            mScoreText.text = mScore.ToString();

            if(mBricks.Count == 0)
            {
                SetState(GameState.StateID.WIN);
            }
        }
        #endregion
    }

}
