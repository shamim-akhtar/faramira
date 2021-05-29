using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TicTacToe
{

    public enum StateTypes
    {
        NEW_GAME,
        PLAYER_PLAY,
        AI_PLAY,
        DRAW,
        AI_WIN,
        PLAYER_WIN,
        RESET,
    }

    public class StatePlayAI : State
    {
        private Board mBoard;
        public StatePlayAI(Board board) : base()
        {
            ID = (int)StateTypes.AI_PLAY;
            Name = "StatePlayAI";
            mBoard = board;

            //int index = Random.Range(0, GameApp.Instance.mAudioClips.Length);
            //GameApp.Instance.mAmbientSound.Play(GameApp.Instance.mAudioClips[index], 0.8f);
        }

        override public void Enter()
        {
            base.Enter();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
            mBoard.SetAIMove();

            if (mBoard.IsGameOver())
            {
                mBoard.mFSM.SetCurrentState((int)StateTypes.AI_WIN);
            }
            else if (mBoard.IsGameDraw())
            {
                mBoard.mFSM.SetCurrentState((int)StateTypes.DRAW);
            }
            else
                mBoard.mFSM.SetCurrentState((int)StateTypes.PLAYER_PLAY);
        }
    }
    public class StatePlayPlayer : State
    {
        private Board mBoard;
        public StatePlayPlayer(Board board) : base()
        {
            ID = (int)StateTypes.PLAYER_PLAY;
            Name = "StatePlayPlayer";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
            Cell cell = mBoard.CheckMouseClick();
            if (cell && !cell.Used())
            {
                mBoard.SetPlayerMove(cell.GetRowIndex(), cell.GetColIndex());
            }
        }
    }

    public class StateDraw : State
    {
        private Board mBoard;
        public StateDraw(Board board) : base()
        {
            ID = (int)StateTypes.DRAW;
            Name = "StateDraw";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
            mBoard.Draw();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
        }
    }

    public class StateAIWin : State
    {
        private Board mBoard;
        public StateAIWin(Board board) : base()
        {
            ID = (int)StateTypes.AI_WIN;
            Name = "StateAIWin";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
            mBoard.AIWin();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
        }

    }
    public class StatePlayerWin : State
    {

        private Board mBoard;
        public StatePlayerWin(Board board) : base()
        {
            ID = (int)StateTypes.PLAYER_WIN;
            Name = "StatePlayerWin";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
            mBoard.PlayerWin();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
        }
    }

    public class StateReset : State
    {

        private Board mBoard;

        public StateReset(Board board) : base()
        {
            ID = (int)StateTypes.RESET;
            Name = "StateReset";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
            mBoard.Reset();
        }
        override public void Exit()
        {
            base.Exit();
        }
        override public void Update()
        {
            base.Update();
        }
    }
    public class StateNewGame : State
    {
        private bool mStartTurn = true; // player makes the first move.

        private Board mBoard;
        private float mWaitTime = 0.1f;
        private float dt = 0.0f;

        public StateNewGame(Board board) : base()
        {
            ID = (int)StateTypes.NEW_GAME;
            Name = "StateNewGame";
            mBoard = board;
        }
        override public void Enter()
        {
            base.Enter();
            dt = 0.0f;
        }
        override public void Exit()
        {
            base.Exit();
            mStartTurn = !mStartTurn;// toggle first move
        }
        override public void Update()
        {
            base.Update();
            dt += Time.deltaTime;
            if (dt >= mWaitTime)
            {
                if (mStartTurn)
                {
                    mBoard.mFSM.SetCurrentState((int)StateTypes.PLAYER_PLAY);
                }
                else
                {
                    mBoard.mFSM.SetCurrentState((int)StateTypes.AI_PLAY);
                }
            }
        }
    }

    public class Board : MonoBehaviour
    {
        public Cell[] mCells;
        private TicTacToe tt = new TicTacToe();
        // Start is called before the first frame update

        public Text AIScore;
        public Text PlayerScore;
        public Text WinText;

        //public Button mPlayBtn;

        int mPlayerScore = 0;
        int mAIScore = 0;
        [SerializeField] private float timeMultiplier = 1.0f;
        public FiniteStateMachine mFSM = new FiniteStateMachine();

        public AudioSource mAudioSource;
        public AudioClip mWinPlayer;
        public AudioClip mWinAI;
        public AudioClip mDraw;
        public AudioClip mClick;
        public AudioClip mCancel;

        public GameObject[] mLines;

        //public BottomMenu mBottomMenu;
        public GameMenuHandler mGameMenuHandler;

        void Start()
        {
            mFSM.Add(new StateAIWin(this));
            mFSM.Add(new StateDraw(this));
            mFSM.Add(new StateNewGame(this));
            mFSM.Add(new StatePlayAI(this));
            mFSM.Add(new StatePlayerWin(this));
            mFSM.Add(new StatePlayPlayer(this));
            mFSM.Add(new StateReset(this));

            mFSM.SetCurrentState((int)StateTypes.NEW_GAME);

            //StartCoroutine(FadeOutText(1f, WinText));
            StartCoroutine(WaitAndFadeOut(4.0f, 1.0f, WinText));

            mGameMenuHandler.onClickNextGame = OnClickPlay;
        }

        IEnumerator WaitAndFadeOut(float waitTime, float timeSpeed, Text text)
        {
            yield return new WaitForSeconds(waitTime);
            yield return StartCoroutine(FadeOutText(timeSpeed, text));
        }
        private IEnumerator FadeInText(float timeSpeed, Text text)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            while (text.color.a < 1.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * timeSpeed));
                yield return null;
            }
        }
        private IEnumerator FadeOutText(float timeSpeed, Text text)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * timeSpeed));
                yield return null;
            }
        }
        public void FadeInText(float timeSpeed = -1.0f)
        {
            if (timeSpeed <= 0.0f)
            {
                timeSpeed = timeMultiplier;
            }
            StartCoroutine(FadeInText(timeSpeed, WinText));
        }
        public void FadeOutText(float timeSpeed = -1.0f)
        {
            if (timeSpeed <= 0.0f)
            {
                timeSpeed = timeMultiplier;
            }
            StartCoroutine(FadeOutText(timeSpeed, WinText));
        }

        // Update is called once per frame
        void Update()
        {
            if (mGameMenuHandler.mShowingExitPopup) return;
            //CheckMouseClick();
            mFSM.Update();
        }

        IEnumerator PlayerWin_Coroutine()
        {
            yield return new WaitForSeconds(0.5f);
            mPlayerScore += 5;
            PlayerScore.text = mPlayerScore.ToString();
            WinText.text = "Player Wins";
            //show line
            mLines[tt.WinID].SetActive(true);

            //mPlayBtn.gameObject.SetActive(true);
            StartCoroutine(FadeInText(2.0f, WinText));
            mAudioSource.PlayOneShot(mWinPlayer);

            // change the menu.
            mGameMenuHandler.SetActiveBtnNext(true);
            mGameMenuHandler.SetActiveBtnHome(true);
        }

        public void PlayerWin()
        {
            StartCoroutine(PlayerWin_Coroutine());
        }

        IEnumerator AIWin_Coroutine()
        {
            yield return new WaitForSeconds(0.5f);
            mAIScore += 5;
            AIScore.text = mAIScore.ToString();
            WinText.text = "AI Wins";
            //show line
            mLines[tt.WinID].SetActive(true);
            //mPlayBtn.gameObject.SetActive(true);
            StartCoroutine(FadeInText(2.0f, WinText));
            mAudioSource.PlayOneShot(mWinAI);
            // change the menu.
            mGameMenuHandler.SetActiveBtnNext(true);
            mGameMenuHandler.SetActiveBtnHome(true);
        }

        public void AIWin()
        {
            StartCoroutine(AIWin_Coroutine());
        }

        IEnumerator Draw_Coroutine()
        {
            yield return new WaitForSeconds(0.5f);
            mAIScore += 1;
            mPlayerScore += 1;
            AIScore.text = mAIScore.ToString();
            PlayerScore.text = mPlayerScore.ToString();
            WinText.text = "Draw";

            //mPlayBtn.gameObject.SetActive(true);
            StartCoroutine(FadeInText(2.0f, WinText));
            mAudioSource.PlayOneShot(mDraw);

            // change the menu.
            mGameMenuHandler.SetActiveBtnNext(true);
            mGameMenuHandler.SetActiveBtnHome(true);
        }
        public void Draw()
        {
            StartCoroutine(Draw_Coroutine());
        }

        IEnumerator Reset_Coroutine()
        {
            for (int i = 0; i < 9; ++i)
            {
                mCells[i].Deactivate();
                yield return new WaitForSeconds(0.02f);
            }
            tt.Reset();

            // now to to new game.
            mFSM.SetCurrentState((int)StateTypes.NEW_GAME);
            //mPlayBtn.gameObject.SetActive(false);
            mGameMenuHandler.SetActiveBtnNext(false);
            StartCoroutine(FadeOutText(2.0f, WinText));

            for (int i = 0; i < 8; ++i)
            {
                mLines[i].SetActive(false);
            }

            // disable the main menu.
            //mBottomMenu.SetActive(false);
        }
        public void Reset()
        {
            StartCoroutine(Reset_Coroutine());
        }

        public Cell CheckMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    mAudioSource.PlayOneShot(mClick);
                    return hit.collider.gameObject.GetComponent<Cell>();
                }

                mAudioSource.PlayOneShot(mCancel);
            }
            return null;
        }

        IEnumerator Coroutine_OnMakeMove(float t, int r, int c, bool is_computer)
        {
            mCells[r * 3 + c].Activate(is_computer);
            float dt = 0.0f;
            while (dt <= t)
            {
                float scale = dt / t;
                mCells[r * 3 + c].SetScale(is_computer, scale);
                //Debug.Log(scale);
                dt += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }

        IEnumerator Coroutine_WaitBeforeAIMove(float t)
        {
            yield return new WaitForSeconds(t);

            if (IsGameOver())
            {
                mFSM.SetCurrentState((int)StateTypes.PLAYER_WIN);
            }
            else if (IsGameDraw())
            {
                mFSM.SetCurrentState((int)StateTypes.DRAW);
            }
            else
            {
                mFSM.SetCurrentState((int)StateTypes.AI_PLAY);
            }
        }

        public void SetPlayerMove(int r, int c)
        {
            tt.SetMove(r, c, false);
            //mCells[r * 3 + c].Activate(false);
            StartCoroutine(Coroutine_OnMakeMove(0.2f, r, c, false));
            //mCells[r * 3 + c].SetScale(false, 0.2f);
            StartCoroutine(Coroutine_WaitBeforeAIMove(0.5f));
        }

        public void SetAIMove()
        {
            CellIndex bestMove = tt.FindNextMove();

            tt.SetMove(bestMove.Row, bestMove.Col, true);
            //mCells[bestMove.Row * 3 + bestMove.Col].Activate(true);
            StartCoroutine(Coroutine_OnMakeMove(0.2f, bestMove.Row, bestMove.Col, true));
        }

        public bool IsGameOver()
        {
            bool flag = tt.IsGameOver();
            return flag;
        }

        public bool IsGameDraw()
        {
            return !tt.IsAnyMoveLeft();
        }

        public void OnClickPlay()
        {
            mAudioSource.PlayOneShot(mClick);
            mFSM.SetCurrentState((int)StateTypes.RESET);
        }

        public void OnClick_Back()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}