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
        public GameObject mGoldPrefab;
        public GameObject mExplosionPrefab;

        // The generator prefab that generates the Maze.
        public GameObject mGeneratorPrefab;

        [HideInInspector]
        public FiniteStateMachine mFsm = new FiniteStateMachine();

        private Generator mCurrentGenerator;
        private PlayerMovement mPlayerMovement;

        List<MazePathFinder> mNPCs = new List<MazePathFinder>();
        List<GameObject> mGolds = new List<GameObject>();

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
                StartCoroutine(Coroutine_Spawn_Gold());
                mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
            }
        }
        void OnEnterPlaying()
        {
            mPlayerMovement.mPlayer.SetActive(true);
            StartCoroutine(Coroutine_Spawn_NPC());
        }
        void OnExitPlaying()
        {
            StopCoroutine(Coroutine_Spawn_NPC());
        }

        IEnumerator Coroutine_DestroyAfter(float duration, GameObject obj)
        {
            yield return new WaitForSeconds(duration);
            Destroy(obj);
        }

        void OnUpdatePlaying()
        {
            mPlayerMovement.Tick();

            // TEST
            // handle input to create NPC.
            //HandleMouseClick();

            // check for collision with NPCs
            for(int i = 0; i < mNPCs.Count; ++i)
            {
                if(
                    Mathf.Abs(mNPCs[i].transform.position.x - mPlayerMovement.mPlayer.transform.position.x) < 0.5f &&
                    Mathf.Abs(mNPCs[i].transform.position.y - mPlayerMovement.mPlayer.transform.position.y) < 0.5f)
                {
                    GameObject exp = Instantiate(mExplosionPrefab, mNPCs[i].transform.position, Quaternion.identity);
                    exp.SetActive(true);
                    StartCoroutine(Coroutine_DestroyAfter(1.0f, exp));
                    mFsm.SetCurrentState((int)GameState.StateID.LOSE);
                }
            }
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

                //Debug.Log("POSR: " + rayPos.x + ", " + rayPos.y);
                int x = (int)rayPos.x - mCurrentGenerator.START_X;
                int y = (int)rayPos.y - mCurrentGenerator.START_Y;
                //Debug.Log("POS : " + x + ", " + y);

                if (x < 0 || x >= mCurrentGenerator.cols || y < 0 || y >= mCurrentGenerator.rows) return;
                Maze.Cell cell = mCurrentGenerator.maze.GetCell(x, y);

                GameObject npc = Instantiate(mNpcPrefab, new Vector3((int)rayPos.x, (int)rayPos.y, 0.0f), Quaternion.identity);
                MazePathFinder mpf = npc.AddComponent<MazePathFinder>();
                mpf.mGenerator = mCurrentGenerator;
                mpf.mNpc = npc;
                mpf.mSpeed = 1.0f;

                // player position.
                int dx = (int)mPlayerMovement.mPlayer.transform.position.x - mCurrentGenerator.START_X;
                int dy = (int)mPlayerMovement.mPlayer.transform.position.y - mCurrentGenerator.START_Y;
                mpf.FindPath(cell, mCurrentGenerator.maze.GetCell(dx, dy));

                mNPCs.Add(mpf);

            }
        }

        #region SPAWN_ENEMIES
        IEnumerator Coroutine_Spawn_NPC(float duration = 10.0f)
        {
            while (mFsm.GetCurrentState().ID == (int)GameState.StateID.PLAYING)
            {
                //
                //Maze.Cell goal = mCurrentGenerator.maze.GetCell(mCurrentGenerator.cols - 1, mCurrentGenerator.rows - 1);
                //int sx = goal.x + mCurrentGenerator.START_X;
                //int sy = goal.y + mCurrentGenerator.START_Y;

                int rx = Random.Range(2, mCurrentGenerator.cols - 2);
                int ry = Random.Range(2, mCurrentGenerator.rows - 2);

                int sx = rx + mCurrentGenerator.START_X;
                int sy = ry + mCurrentGenerator.START_Y;

                Maze.Cell startCell = mCurrentGenerator.maze.GetCell(rx, ry);

                GameObject npc = Instantiate(mNpcPrefab, new Vector3(sx, sy, 0.0f), Quaternion.identity);
                MazePathFinder mpf = npc.AddComponent<MazePathFinder>();
                mpf.mGenerator = mCurrentGenerator;
                mpf.mNpc = npc;
                mpf.mSpeed = 1.0f;

                // player position.
                int dx = (int)mPlayerMovement.mPlayer.transform.position.x - mCurrentGenerator.START_X;
                int dy = (int)mPlayerMovement.mPlayer.transform.position.y - mCurrentGenerator.START_Y;
                mpf.FindPath(startCell, mCurrentGenerator.maze.GetCell(dx, dy));

                mNPCs.Add(mpf);
                yield return new WaitForSeconds(duration);
            }
        }
        IEnumerator Coroutine_Spawn_Gold(int count = 5)
        {
            for(int i = 0; i < count; ++i)
            {
                int rx = Random.Range(2, mCurrentGenerator.cols - 2);
                int ry = Random.Range(2, mCurrentGenerator.rows - 2);

                int sx = rx + mCurrentGenerator.START_X;
                int sy = ry + mCurrentGenerator.START_Y;

                GameObject gold = Instantiate(mGoldPrefab, new Vector3(sx, sy, 0.0f), Quaternion.identity);
                mGolds.Add(gold);

                yield return null;
            }
        }

        #endregion

        void OnEnterWin()
        {
            // remove all the NPCs.
            for (int i = 0; i < mNPCs.Count; ++i)
            {
                Destroy(mNPCs[i].gameObject);
            }
            mNPCs.Clear();
            for (int i = 0; i < mGolds.Count; ++i)
            {
                Destroy(mGolds[i]);
            }
            mGolds.Clear();
            mPlayerMovement.mPlayer.SetActive(false);
            mMenuHandler.SetActiveBtnNext(true);
        }

        void OnEnterLose()
        {
            Debug.Log("Lost");
            // remove all the NPCs.
            for(int i = 0; i < mNPCs.Count; ++i)
            {
                Destroy(mNPCs[i].gameObject);
            }
            mNPCs.Clear();
            for (int i = 0; i < mGolds.Count; ++i)
            {
                Destroy(mGolds[i]);
            }
            mGolds.Clear();
            mPlayerMovement.mPlayer.SetActive(false);
            mMenuHandler.SetActiveBtnNext(true);
        }
        #endregion


    }
}
