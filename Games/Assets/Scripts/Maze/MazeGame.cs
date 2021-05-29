using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

namespace Maze
{
    public class MazeGame : MonoBehaviour
    {
        public GameMenuHandler mMenuHandler;
        public GameObject mNpcPrefab;

        // The generator prefab that generates the Maze.
        public GameObject mGeneratorPrefab;

        [HideInInspector]
        public FiniteStateMachine mFsm = new FiniteStateMachine();

        private Generator mCurrentGenerator;
        private PlayerMovement mPlayerMovement;

        void Start()
        {
            mPlayerMovement = GetComponent<PlayerMovement>();
            mPlayerMovement.mOnReachDestination += OnReachDestination;

            mFsm.Add(
                new GameState(
                    GameState.StateID.NEW_GAME,
                    OnEnterNewGame)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.GENERATING_MAZE,
                    OnEnterGeneratingMaze,
                    null,
                    OnUpdateGeneratingMaze)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.PLAYING,
                    OnEnterPlaying,
                    null,
                    OnUpdatePlaying)
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

        void OnReachDestination()
        {
            mFsm.SetCurrentState((int)GameState.StateID.WIN);
        }

        void NextGame()
        {
            if(mCurrentGenerator!=null)
            {
                Destroy(mCurrentGenerator.gameObject);
            }
            mFsm.SetCurrentState((int)GameState.StateID.NEW_GAME);
        }

        #region FSM State methods.
        void OnEnterNewGame()
        {
            GameObject obj = Instantiate(mGeneratorPrefab);
            mCurrentGenerator = obj.GetComponent<Generator>();

            mPlayerMovement.mGenerator = mCurrentGenerator;
            mPlayerMovement.mPlayer = mCurrentGenerator.mMouse;

            mFsm.SetCurrentState((int)GameState.StateID.GENERATING_MAZE);

        }

        void OnEnterGeneratingMaze()
        {
            // hide the joystick.
        }

        void OnUpdateGeneratingMaze()
        {
            if(mCurrentGenerator.mMazeGenerated)
            {
                mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
            }

        }
        void OnEnterPlaying()
        {
            // show the joystick.
        }

        void OnUpdatePlaying()
        {
            mPlayerMovement.Tick();

            // TEST
            // handle input to create NPC.
            HandleMouseClick();
        }

        void HandleMouseClick()
        {
            if (mMenuHandler.mShowingExitPopup)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 rayPos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y
                );

                Debug.Log("POSR: " + rayPos.x + ", " + rayPos.y);
                int x = (int)rayPos.x - mCurrentGenerator.START_X;
                int y = (int)rayPos.y - mCurrentGenerator.START_Y;
                Debug.Log("POS : " + x + ", " + y);

                if (x < 0 || x >= mCurrentGenerator.cols || y < 0 || y >= mCurrentGenerator.rows) return;
                Maze.Cell cell = mCurrentGenerator.maze.GetCell(x, y);

                GameObject npc = Instantiate(mNpcPrefab, new Vector3((int)rayPos.x, (int)rayPos.y, 0.0f), Quaternion.identity);
                MazePathFinder mpf = npc.AddComponent<MazePathFinder>();
                mpf.mGenerator = mCurrentGenerator;
                mpf.mNpc = npc;
                mpf.mSpeed = 2.0f;

                // player position.
                int dx = (int)mPlayerMovement.mPlayer.transform.position.x - mCurrentGenerator.START_X;
                int dy = (int)mPlayerMovement.mPlayer.transform.position.y - mCurrentGenerator.START_Y;
                mpf.FindPath(cell, mCurrentGenerator.maze.GetCell(dx, dy));

            }
        }

        void OnEnterWin()
        {
            mMenuHandler.SetActiveBtnNext(true);
        }

        void OnEnterLose()
        {
            mMenuHandler.SetActiveBtnNext(true);
        }
        #endregion
    }
}
