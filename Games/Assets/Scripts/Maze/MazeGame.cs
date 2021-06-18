using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;

namespace Maze
{
    public class MazeGame : MonoBehaviour
    {
        public GameMenuHandler mMenuHandler;
        public GameObject mNpcPrefab;
        public GameObject mGoldPrefab;
        public GameObject mExplosionPrefab;
        public GameObject mAmmoPrefab;
        public GameObject mShootItemPrefab;

        // The generator prefab that generates the Maze.
        public GameObject mGeneratorPrefab;
        public Transform mShootEffectPrefab;

        public Effects.csShowAllEffect mPSManager;

        [HideInInspector]
        public FiniteStateMachine mFsm = new FiniteStateMachine();

        private Generator mCurrentGenerator;
        private PlayerMovement mPlayerMovement;

        List<MazePathFinder> mShootItems = new List<MazePathFinder>();
        List<MazePathFinder> mNPCs = new List<MazePathFinder>();
        List<GameObject> mGolds = new List<GameObject>();
        List<GameObject> mAmmos = new List<GameObject>();

        #region SCORE
        int mGoldScore = 0;
        int mAmmoScore = 0;
        public Text mGoldScoreText;
        public Text mAmmoScoreText;
        #endregion

        int mLevel = 0;

        public Button mShootButton;

        public FixedButton mFireButton;

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
            UpdateScoreDisplay();
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
                StartCoroutine(Coroutine_Spawn_Gold(10));
                StartCoroutine(Coroutine_Spawn_Ammo(5));
                mFsm.SetCurrentState((int)GameState.StateID.PLAYING);
            }
        }
        void OnEnterPlaying()
        {
            mPlayerMovement.mPlayer.SetActive(true);
            float duration = 10.0f - mLevel;

            if (duration < 2.0f) duration = 2.0f;
            StartCoroutine(Coroutine_Spawn_NPC(duration));
            if(mAmmoScore > 0)
            {
                mShootButton.gameObject.SetActive(true);
            }
        }
        void OnExitPlaying()
        {
            StopCoroutine(Coroutine_Spawn_NPC());
            mShootButton.gameObject.SetActive(false);
        }

        IEnumerator Coroutine_DestroyAfter(float duration, GameObject obj)
        {
            yield return new WaitForSeconds(duration);
            Destroy(obj);
        }

        void CheckForNPC_Player_Collision()
        {
            for (int i = 0; i < mNPCs.Count; ++i)
            {
                if (
                Mathf.Abs(mNPCs[i].transform.position.x - mPlayerMovement.mPlayer.transform.position.x) < 0.5f &&
                Mathf.Abs(mNPCs[i].transform.position.y - mPlayerMovement.mPlayer.transform.position.y) < 0.5f)
                {
                    //GameObject exp = Instantiate(mExplosionPrefab, mNPCs[i].transform.position, Quaternion.identity);
                    //exp.SetActive(true);
                    //StartCoroutine(Coroutine_DestroyAfter(1.0f, exp));

                    mPSManager.ShowEFX(14, mGolds[i].transform.position);
                    mFsm.SetCurrentState((int)GameState.StateID.LOSE);
                }
            }
        }
        void UpdateScoreDisplay()
        {
            mGoldScoreText.text = mGoldScore.ToString();
            mAmmoScoreText.text = mAmmoScore.ToString();
        }

        void CheckForGold_Player_Collision()
        {
            List<GameObject> toRemove = new List<GameObject>();
            for (int i = 0; i < mGolds.Count; ++i)
            {
                if (
                Mathf.Abs(mGolds[i].transform.position.x - mPlayerMovement.mPlayer.transform.position.x) < 0.5f &&
                Mathf.Abs(mGolds[i].transform.position.y - mPlayerMovement.mPlayer.transform.position.y) < 0.5f)
                {
                    mPSManager.ShowEFX(17, mGolds[i].transform.position);
                    toRemove.Add(mGolds[i]);

                    mGoldScore += 10;
                }
            }

            for(int i = 0; i < toRemove.Count; ++i)
            {
                Destroy(toRemove[i]);
                mGolds.Remove(toRemove[i]);
            }
            toRemove.Clear();
        }

        void CheckForAmmo_Player_Collision()
        {
            List<GameObject> toRemove = new List<GameObject>();
            for (int i = 0; i < mAmmos.Count; ++i)
            {
                if (
                Mathf.Abs(mAmmos[i].transform.position.x - mPlayerMovement.mPlayer.transform.position.x) < 0.5f &&
                Mathf.Abs(mAmmos[i].transform.position.y - mPlayerMovement.mPlayer.transform.position.y) < 0.5f)
                {
                    mPSManager.ShowEFX(18, mAmmos[i].transform.position);
                    toRemove.Add(mAmmos[i]);

                    mAmmoScore += 20;
                }
            }

            for (int i = 0; i < toRemove.Count; ++i)
            {
                Destroy(toRemove[i]);
                mAmmos.Remove(toRemove[i]);
            }
            toRemove.Clear();
        }

        void CheckForShootItem_NPC_Collision()
        {
            List<MazePathFinder> toRemoveNPC = new List<MazePathFinder>();
            List<MazePathFinder> toRemoveShootItems = new List<MazePathFinder>();
            for (int i = 0; i < mShootItems.Count; ++i)
            {
                for (int j = 0; j < mNPCs.Count; ++j)
                {
                    if (
                    Mathf.Abs(mShootItems[i].transform.position.x - mNPCs[j].transform.position.x) < 0.5f &&
                    Mathf.Abs(mShootItems[i].transform.position.y - mNPCs[j].transform.position.y) < 0.5f)
                    {
                        mPSManager.ShowEFX(32, mShootItems[i].transform.position);
                        //toRemoveShootItems.Add(mShootItems[i]);
                        toRemoveNPC.Add(mNPCs[j]);
                    }
                }
            }

            //for (int i = 0; i < toRemoveShootItems.Count; ++i)
            //{
            //    mShootItems.Remove(toRemoveShootItems[i]);
            //    Destroy(toRemoveShootItems[i].gameObject);
            //}
            toRemoveShootItems.Clear();

            for (int i = 0; i < toRemoveNPC.Count; ++i)
            {
                mNPCs.Remove(toRemoveNPC[i]);
                Destroy(toRemoveNPC[i].gameObject);
            }
            toRemoveNPC.Clear();
        }

        void OnUpdatePlaying()
        {
            if (mAmmoScore > 0)
            {
                mShootButton.gameObject.SetActive(true);
            }
            mPlayerMovement.Tick();

            CheckForNPC_Player_Collision();
            CheckForGold_Player_Collision();
            CheckForAmmo_Player_Collision();
            CheckForShootItem_NPC_Collision();

            if (mFireButton.Pressed)
            {
                //StartCoroutine(Coroutine_Fire());
            }
        }

        IEnumerator Coroutine_Fire()
        {
            yield return StartCoroutine(mPSManager.Coroutine_ShowEFX(29, mPlayerMovement.mPlayer.transform.position, 0.2f));
        }

        #region HANDLE CLICKS TEST
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
        #endregion

        #region SPAWN_ENEMIES
        IEnumerator Coroutine_Spawn_NPC(float duration = 10.0f)
        {
            while (mFsm.GetCurrentState().ID == (int)GameState.StateID.PLAYING)
            {

                int rx = Random.Range(2, mCurrentGenerator.cols - 2);
                int ry = Random.Range(2, mCurrentGenerator.rows - 2);

                int sx = rx + mCurrentGenerator.START_X;
                int sy = ry + mCurrentGenerator.START_Y;

                Maze.Cell startCell = mCurrentGenerator.maze.GetCell(rx, ry);

                GameObject npc = Instantiate(mNpcPrefab, new Vector3(sx, sy, 0.0f), Quaternion.identity);
                MazePathFinder mpf = npc.AddComponent<MazePathFinder>();
                mpf.mGenerator = mCurrentGenerator;
                mpf.mNpc = npc;
                mpf.mSpeed = 0.5f;

                // player position.
                int dx = (int)mPlayerMovement.mPlayer.transform.position.x - mCurrentGenerator.START_X;
                int dy = (int)mPlayerMovement.mPlayer.transform.position.y - mCurrentGenerator.START_Y;
                mpf.FindPath(startCell, mCurrentGenerator.maze.GetCell(dx, dy));
                mpf.onReachGoal += NPCOnReachGoal;

                mNPCs.Add(mpf);
                yield return new WaitForSeconds(duration);
            }
        }

        IEnumerator Coroutine_NPCOnReachGoal(MazePathFinder pf)
        {
            yield return StartCoroutine(mPSManager.Coroutine_ShowEFX(14, pf.gameObject.transform.position, 0.2f));
            mNPCs.Remove(pf);
            Destroy(pf.gameObject);
        }

        IEnumerator Coroutine_ShootItemOnReachGoal(MazePathFinder pf)
        {
            yield return StartCoroutine(mPSManager.Coroutine_ShowEFX(14, pf.gameObject.transform.position, 0.2f));
            mShootItems.Remove(pf);
            Destroy(pf.gameObject);
        }

        void NPCOnReachGoal(MazePathFinder pf)
        {
            StartCoroutine(Coroutine_NPCOnReachGoal(pf));
        }

        void ShootItemOnReachGoal(MazePathFinder pf)
        {
            StartCoroutine(Coroutine_ShootItemOnReachGoal(pf));
        }

        IEnumerator Coroutine_Spawn_Gold(int count = 5)
        {
            int i = 0;
            while (i < count)
            {
                int rx = Random.Range(2, mCurrentGenerator.cols - 2);
                int ry = Random.Range(2, mCurrentGenerator.rows - 2);

                Maze.Cell cell = mCurrentGenerator.maze.GetCell(rx, ry);
                while (!cell.containsGold && !cell.containsAmmo)
                {
                    int sx = rx + mCurrentGenerator.START_X;
                    int sy = ry + mCurrentGenerator.START_Y;

                    GameObject gold = Instantiate(mGoldPrefab, new Vector3(sx, sy, 0.0f), Quaternion.identity);
                    mGolds.Add(gold);

                    mCurrentGenerator.maze.GetCell(rx, ry).containsGold = true;
                    i++;
                    yield return null;
                }
            }
        }
        IEnumerator Coroutine_Spawn_Ammo(int count = 5)
        {
            int i = 0;
            while(i < count)
            {
                int rx = Random.Range(2, mCurrentGenerator.cols - 2);
                int ry = Random.Range(2, mCurrentGenerator.rows - 2);

                Maze.Cell cell = mCurrentGenerator.maze.GetCell(rx, ry);
                while (!cell.containsGold && !cell.containsAmmo)
                {
                    int sx = rx + mCurrentGenerator.START_X;
                    int sy = ry + mCurrentGenerator.START_Y;

                    GameObject ammo = Instantiate(mAmmoPrefab, new Vector3(sx, sy, 0.0f), Quaternion.identity);
                    mAmmos.Add(ammo);

                    mCurrentGenerator.maze.GetCell(rx, ry).containsAmmo = true;
                    i++;
                    yield return null;
                }
            }
        }

        #endregion

        void DestroyAllObjects()
        {
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
            for (int i = 0; i < mAmmos.Count; ++i)
            {
                Destroy(mAmmos[i]);
            }
            mAmmos.Clear();
            mPlayerMovement.mPlayer.SetActive(false);
        }

        IEnumerator Coroutine_OnEnterWin()
        {
            yield return StartCoroutine(mPSManager.Coroutine_ShowEFX(4, Vector3.zero, 1.0f));
            DestroyAllObjects();
            mMenuHandler.SetActiveBtnNext(true);
        }

        void OnEnterWin()
        {
            StartCoroutine(Coroutine_OnEnterWin());
            mLevel += 1;
        }

        void OnEnterLose()
        {
            DestroyAllObjects();
            mMenuHandler.SetActiveBtnNext(true);
        }
        #endregion

        #region SHOW_GOLD_COLLECTION_EFFECT

        void OnEnterShowGoldCollectionEffect()
        {
        }
        #endregion

        public void Shoot()
        {
            if (mNPCs.Count == 0) return;
            if (mAmmoScore < 2) return;

            mAmmoScore -= 2;

            // npc pos
            int ex = (int)mNPCs[0].transform.position.x - mCurrentGenerator.START_X;
            int ey = (int)mNPCs[0].transform.position.y - mCurrentGenerator.START_Y;

            // player position.
            int dx = (int)mPlayerMovement.mPlayer.transform.position.x - mCurrentGenerator.START_X;
            int dy = (int)mPlayerMovement.mPlayer.transform.position.y - mCurrentGenerator.START_Y;

            GameObject npc = Instantiate(mShootItemPrefab, mPlayerMovement.mPlayer.transform.position, Quaternion.identity);
            MazePathFinder mpf = npc.AddComponent<MazePathFinder>();
            mpf.mGenerator = mCurrentGenerator;
            mpf.mNpc = npc;
            mpf.mSpeed = 10.0f;

            // attach a PS to it.
            Transform obj = Instantiate(mShootEffectPrefab, npc.transform.position, Quaternion.identity);
            obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            obj.SetParent(npc.transform);

            // apply the path finder.
            mpf.FindPath(mCurrentGenerator.maze.GetCell(dx, dy), mCurrentGenerator.maze.GetCell(ex, ey));
            mShootItems.Add(mpf);

            mpf.onReachGoal += ShootItemOnReachGoal;
        }
    }
}
