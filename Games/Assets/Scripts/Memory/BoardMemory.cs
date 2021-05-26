using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Patterns;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Memory
{

    public class BoardMemory : MonoBehaviour
    {
        public int mLevel = 1;

        List<Tuple<int, int>> mCardLayout = new List<Tuple<int, int>>();

        float SPRITE_W = 7.5f;
        float SPRITE_H = 11f;

        List<GameObject> mCards = new List<GameObject>();
        List<GameObject> mCardMasks = new List<GameObject>();
        List<Sprite> mCardSprites = new List<Sprite>();
        Sprite mCardMaskSprite = null;

        string[] SPRITE_NAMES =
        {
            "2C", "2D", "2H", "2S",
            "3C", "3D", "3H", "3S",
            "4C", "4D", "4H", "4S",
            "5C", "5D", "5H", "5S",
            "6C", "6D", "6H", "6S",
            "7C", "7D", "7H", "7S",
            "8C", "8D", "8H", "8S",
            "9C", "9D", "9H", "9S",
            "10C", "10D", "10H", "10S",
            "JC", "JD", "JH", "JS",
            "QC", "QD", "QH", "QS",
            "KC", "KD", "KH", "KS",
            "AC", "AD", "AH", "AS",
        };
        string[] SPRITE_MASK_NAMES =
        {
            "blue_back", "gray_back", "green_back", "purple_back",
            "red_back", "yellow_back"
        };
        private static System.Random rng = new System.Random();

        // FiniteStateMachine
        FiniteStateMachine mFsm = new FiniteStateMachine();

        bool mMadeFirstMove = false;
        bool mMadeSecondMove = false;

        string mFirstCardName;
        string mSecondCardName;
        int mFirstMaskIndex = -1;
        int mSecondMaskIndex = -1;

        private int mTotalCardsShown = 0;

        public Transform mMasksParent;
        public Transform mCardsParent;

        public Text mScoreText;
        private int mScore = 0;

        public CountDownTimerA mCountDownTimer;
        public BottomMenu mBottomMenu;
        public CanvasConfirmA mConfirmExit;

        bool mShowingConfirmExitMenu = false;

        private void Awake()
        {
            mCardLayout.Add(new Tuple<int, int>(2, 2));
            mCardLayout.Add(new Tuple<int, int>(3, 2));
            mCardLayout.Add(new Tuple<int, int>(4, 2));
            mCardLayout.Add(new Tuple<int, int>(4, 3));
            mCardLayout.Add(new Tuple<int, int>(4, 4));
            mCardLayout.Add(new Tuple<int, int>(4, 5));
            mCardLayout.Add(new Tuple<int, int>(4, 6));
            mCardLayout.Add(new Tuple<int, int>(5, 6));
            mCardLayout.Add(new Tuple<int, int>(6, 6));
            mCardLayout.Add(new Tuple<int, int>(6, 7));
            mCardLayout.Add(new Tuple<int, int>(7, 8));
            mCardLayout.Add(new Tuple<int, int>(8, 8));
        }

        // Start is called before the first frame update
        void Start()
        {
            mConfirmExit.onClickYes = LoadMenu;
            mConfirmExit.onClickNo = DisableExitMenu;
            mFsm.Add(
                new GameState(
                    GameState.StateID.NEW_GAME,
                    OnEnterNewGame)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.WAITNG_FOR_FIRST_CARD,
                    OnEnterWaitingForFirstCard,
                    null,
                    OnUpdateWaitingForFirstCard)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.FIRST_CARD,
                    OnEnterFirstCard,
                    OnExitFirstCard,
                    OnUpdateFirstCard)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.SECOND_CARD,
                    OnEnterSecondCard,
                    OnExitSecondCard,
                    OnUpdateSecondCard)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.WIN,
                    OnEnterWin,
                    OnExitWin,
                    OnUpdateWin)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.NO_INPUT_MODE,
                    null)
                );
            mFsm.Add(
                new GameState(
                    GameState.StateID.NEXT_GAME,
                    OnEnterNextGame)
                );

            mFsm.SetCurrentState((int)GameState.StateID.NEW_GAME);
        }

        #region NEW_GAME delegate calls
        void OnEnterNewGame()
        {
            mFirstCardName = "";
            mSecondCardName = "";
            mMadeFirstMove = false;
            mMadeSecondMove = false;
            mFirstMaskIndex = -1;
            mSecondMaskIndex = -1;
            mTotalCardsShown = 0;

            mBottomMenu.btnPrev.gameObject.SetActive(false);
            mBottomMenu.btnNext.gameObject.SetActive(false);

            //Debug.Log("Enter: NEW_GAME");
            CreateLevel(mLevel - 1);
        }
        #endregion

        #region WAITNG_FOR_FIRST_CARD delegate calls
        void OnEnterWaitingForFirstCard()
        {
            mFirstCardName = "";
            mSecondCardName = "";
            mMadeFirstMove = false;
            mMadeSecondMove = false;
            mFirstMaskIndex = -1;
            mSecondMaskIndex = -1;

            //Debug.Log("Enter: WAITNG_FOR_FIRST_CARD");
            StartCoroutine(Coroutine_AskPlayerToMakeFirstMove(2.0f));
        }

        // A coroutine to display message to the user every 
        // few seconds to make the move by clicking on the card.
        IEnumerator Coroutine_AskPlayerToMakeFirstMove(float waitTime)
        {
            while (!mMadeFirstMove)
            {
                //Debug.Log("Make your first move by clicking on any card");
                yield return new WaitForSeconds(waitTime);
            }
        }

        void OnUpdateWaitingForFirstCard()
        {
            HandleMouseClick();
        }
        #endregion

        #region FIRST_CARD delegate calls
        void OnEnterFirstCard()
        {
            //Debug.Log("Enter: FIRST_CARD");
        }
        void OnExitFirstCard()
        {
        }

        void OnUpdateFirstCard()
        {
            HandleMouseClick();
        }
        #endregion

        #region SECOND_CARD delegate calls
        IEnumerator Coroutine_FirstCardSecondCardSame()
        {
            mFsm.SetCurrentState((int)GameState.StateID.NO_INPUT_MODE);
            mScore += 20;
            mScoreText.text = mScore.ToString();

            yield return new WaitForSeconds(0.1f);
            mFsm.SetCurrentState((int)GameState.StateID.WAITNG_FOR_FIRST_CARD);
        }

        IEnumerator Coroutine_FirstCardSecondCardNotSame()
        {
            mFsm.SetCurrentState((int)GameState.StateID.NO_INPUT_MODE);
            yield return new WaitForSeconds(0.5f);
            ShowMask(mFirstMaskIndex);
            ShowMask(mSecondMaskIndex);

            mScore -= 5;
            if (mScore < 0) mScore = 0;
            mScoreText.text = mScore.ToString();

            mFsm.SetCurrentState((int)GameState.StateID.WAITNG_FOR_FIRST_CARD);
        }

        IEnumerator Coroutine_ChangeToWinState()
        {
            mFsm.SetCurrentState((int)GameState.StateID.NO_INPUT_MODE);

            mScore += 10 * mLevel;
            mScoreText.text = mScore.ToString();
            yield return new WaitForSeconds(1.0f);
            mFsm.SetCurrentState((int)GameState.StateID.WIN);
        }

        void OnEnterSecondCard()
        {
            //Debug.Log("Enter: SECOND_CARD");

            // check if the first card and the second card are the same.
            // if they are the same then keep them open and go to
            // WAITNG_FOR_FIRST_CARD state again.
            if(mFirstCardName == mSecondCardName)
            {
                if (mTotalCardsShown == mCards.Count)
                {
                    StartCoroutine(Coroutine_ChangeToWinState());
                }
                else
                {
                    //Debug.Log("Great Job. You have found a matching card");
                    StartCoroutine(Coroutine_FirstCardSecondCardSame());
                }
            }
            else
            {
                //Debug.Log("Oh no! You did not find a matching card");
                StartCoroutine(Coroutine_FirstCardSecondCardNotSame());
            }
        }
        void OnExitSecondCard()
        {
        }
        void OnUpdateSecondCard()
        {
            HandleMouseClick();
        }
        #endregion

        #region WIN delegate calls

        IEnumerator Coroutine_StartNewGame()
        {
            mFsm.SetCurrentState((int)GameState.StateID.NO_INPUT_MODE);
            yield return new WaitForSeconds(1.0f);
            mLevel += 1;
            if (mLevel >= mCardLayout.Count)
            {
                mLevel = mCardLayout.Count;
            }
            mFsm.SetCurrentState((int)GameState.StateID.NEW_GAME);
        }
        void OnEnterWin()
        {
            Debug.Log("Enter: WIN. Starting new game");
            //StartCoroutine(Coroutine_StartNewGame());
            mFsm.SetCurrentState((int)GameState.StateID.NEXT_GAME);
        }

        void OnExitWin()
        {
        }

        void OnUpdateWin()
        {
            //HandleMouseClick();
        }

        void OnEnterNextGame()
        {
            mBottomMenu.btnPrev.gameObject.SetActive(true);
            mBottomMenu.btnNext.gameObject.SetActive(true);
        }

               
        void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        #endregion

        Sprite LoadSprite(string name)
        {
            Sprite sp = Resources.Load<Sprite>(name);
            if (sp != null)
            {
                sp.name = name;
            }
            return sp;
        }

        void RepositionCamera(int level)
        {
            Tuple<int, int> layout = mCardLayout[level];
            float total_w = SPRITE_W * layout.Item1;
            float total_h = SPRITE_H * layout.Item2;
        }

        bool CheckTwoCardsSame(GameObject obj1, GameObject obj2)
        {
            return obj1.GetComponent<SpriteRenderer>().name == obj2.GetComponent<SpriteRenderer>().name;
        }

        void DestroyAll()
        {
            for (int i = 0; i < mCardMasks.Count; ++i)
            {
                mCards[i].transform.parent = null;
                Destroy(mCards[i]);
                mCards[i] = null;
                mCardMasks[i].transform.parent = null;
                Destroy(mCardMasks[i]);
                mCardMasks[i] = null;
            }
            for (int i = 0; i < mCardSprites.Count; ++i)
            {
                mCardSprites[i] = null;
            }
            mCardMasks.Clear();
            mCards.Clear();
            mCardSprites.Clear();
        }

        public void CreateLevel(int level)
        {
            DestroyAll();

            if (level >= mCardLayout.Count)
            {
                Debug.Log("Completed all levels for this game.");
                return;
            }

            Tuple<int, int> layout = mCardLayout[level];

            float total_w = SPRITE_W * layout.Item1;
            float total_h = SPRITE_H * layout.Item2;

            float start_x = -total_w / 2.0f + SPRITE_W / 2.0f;
            float start_y = -total_h / 2.0f + SPRITE_H / 2.0f;

            int index = 0;
            for (int i = 0; i < layout.Item1; ++i)
            {
                for (int j = 0; j < layout.Item2; ++j)
                {
                    //index = 
                    string name = index.ToString();// i + "_" + j;
                    GameObject go = new GameObject(name);
                    go.tag = "Card";
                    go.AddComponent<SpriteRenderer>();
                    BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
                    bc.offset = new Vector2(0, 0);
                    bc.size = new Vector2(SPRITE_W, SPRITE_H);
                    go.transform.position = new Vector3(i * SPRITE_W + start_x, j * SPRITE_H + start_y, 0.0f);
                    go.transform.parent = mCardsParent;
                    mCards.Add(go);

                    GameObject gom = new GameObject(name);
                    gom.tag = "Mask";
                    gom.AddComponent<SpriteRenderer>();
                    bc = gom.AddComponent<BoxCollider2D>();
                    bc.offset = new Vector2(0, 0);
                    bc.size = new Vector2(SPRITE_W, SPRITE_H);
                    gom.transform.position = new Vector3(i * SPRITE_W + start_x, j * SPRITE_H + start_y, -1.0f);
                    gom.transform.parent = mMasksParent;
                    mCardMasks.Add(gom);
                    index++;
                }
            }

            int maskRand = UnityEngine.Random.Range(0, SPRITE_MASK_NAMES.Length);
            mCardMaskSprite = LoadSprite("Images/Memory/" + SPRITE_MASK_NAMES[maskRand]);

            for (int i = 0; i < mCards.Count; i += 2)
            {
                int rand = UnityEngine.Random.Range(0, SPRITE_NAMES.Length);
                Sprite sp = LoadSprite("Images/Memory/" + SPRITE_NAMES[rand]);
                mCardSprites.Add(sp);
                mCardSprites.Add(sp);
            }

            Camera.main.orthographicSize = total_w + 1.0f + mLevel * 1.2f;

            // shuffle the cards.
            //Debug.Log("Shuffling");
            Shuffle();
        }

        /// <summary>
        /// A countdown after which teh cards are hidden by masks.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="totalTime"></param>
        /// <returns></returns>
        IEnumerator Coroutine_CountdownTimer(float dt, float totalTime)
        {
            float t = Time.deltaTime;
            while(t < totalTime)
            {
                t += dt;
                //Debug.Log("Coroutine_CountdownTimer: " + t);
                yield return new WaitForSeconds(dt);
            }

            //Debug.Log("Coroutine_SetSpritesToCardMasks");
            StartCoroutine(Coroutine_SetSpritesToCardMasks());
        }

        void SetSpritesToCardMasks()
        {
            StartCoroutine(Coroutine_SetSpritesToCardMasks());
            mBottomMenu.btnPrev.gameObject.SetActive(true);
        }

        IEnumerator Coroutine_SetSpritesToCards()
        {
            for (int i = 0; i < mCards.Count; ++i)
            {
                var spriteRenderer = mCards[i].GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = mCardSprites[i];
                mCards[i].name = mCardSprites[i].name;
                yield return new WaitForSeconds(0.05f);
            }

            //Debug.Log("Coroutine_CountdownTimer");
            //StartCoroutine(Coroutine_CountdownTimer(1.0f, 3.0f));
            //
            mCountDownTimer.OnFinishCountDown = SetSpritesToCardMasks;
            StartCoroutine(mCountDownTimer.Coroutine_StartCountDown(3.0f));
        }

        IEnumerator Coroutine_SetSpritesToCardMasks()
        {
            for (int i = 0; i < mCards.Count; ++i)
            {
                var spriteRenderer = mCardMasks[i].GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = mCardMaskSprite;
                yield return null;
            }

            // after all cards are masked we transition to WAITNG_FOR_FIRST_CARD state.
            mFsm.SetCurrentState((int)GameState.StateID.WAITNG_FOR_FIRST_CARD);
        }

        IEnumerator Coroutine_ShowMask(int id)
        {
            yield return new WaitForSeconds(0.5f);
            mCardMasks[id].SetActive(true);
        }

        void Shuffle()
        {
            for (int i = 0; i < mCards.Count; ++i)
            {
                var spriteRenderer = mCards[i].GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = null;
            }
            var shuffledSprites = mCardSprites.OrderBy(a => rng.Next()).ToList();
            mCardSprites = shuffledSprites;
            StartCoroutine(Coroutine_SetSpritesToCards());
        }

        void ApplyMasks()
        {
            StartCoroutine(Coroutine_SetSpritesToCardMasks());
        }

        void ShowMask(int id)
        {
            mTotalCardsShown -= 1;
            StartCoroutine(Coroutine_ShowMask(id));
        }

        void Update()
        {
            mFsm.Update();
        }

        void RemoveMask(GameObject obj)
        {
            obj.SetActive(false);
            mTotalCardsShown++;

            if (!mMadeFirstMove)
            {
                mMadeFirstMove = true;
                mFirstMaskIndex = Int32.Parse(obj.name);
                mFirstCardName = mCards[mFirstMaskIndex].name;
                mFsm.SetCurrentState((int)GameState.StateID.FIRST_CARD);

                //Debug.Log("Index = " + mFirstMaskIndex + " , First card name: " + mFirstCardName);
            }
            else if(!mMadeSecondMove)
            {
                mMadeFirstMove = true;
                mSecondMaskIndex = Int32.Parse(obj.name);
                mSecondCardName = mCards[mSecondMaskIndex].name;
                mFsm.SetCurrentState((int)GameState.StateID.SECOND_CARD);
                //Debug.Log("Index = " + mSecondMaskIndex + " , Second card name: " + mSecondCardName);
            }
        }

        void HandleMouseClick()
        {
            if (mShowingConfirmExitMenu) return;
            if (Input.GetMouseButtonDown(0))
            {
                GameObject selected = Puzzle.Utils.Pick2D();
                if (selected != null)
                {
                    Debug.Log("Selected: " + selected.name);
                    if (selected.tag == "Mask")
                    {
                        RemoveMask(selected);
                    }
                }
            }
        }

        public void OnClickNextGame()
        {
            StartCoroutine(Coroutine_StartNewGame());
        }

        public void OnClickExitGame()
        {
            mConfirmExit.gameObject.SetActive(true);
            mShowingConfirmExitMenu = true;
        }

        void DisableExitMenu()
        {
            mConfirmExit.gameObject.SetActive(false);
            mShowingConfirmExitMenu = false;
        }
    }
}