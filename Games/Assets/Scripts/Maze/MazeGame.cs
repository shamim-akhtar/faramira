using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

namespace Maze
{
    public class MazeGame : MonoBehaviour
    {
        public FiniteStateMachine mFsm = new FiniteStateMachine();
        public GameObject mGeneratorPrefab;

        public GameMenuHandler mMenuHandler;

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
