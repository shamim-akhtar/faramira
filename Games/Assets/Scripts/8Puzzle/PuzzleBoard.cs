using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Puzzle;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

#if FARAMIRA_USE_ADS
public class PuzzleBoard : MonoBehaviour, IUnityAdsListener
#else
public class PuzzleBoard : MonoBehaviour
#endif
{
    public int PuzzleRowsOrCols = 3;
    public string FrameName1 = "frame";
    public string FrameName2 = "frame2";

    //
    [HideInInspector]
    public State _state;
    [HideInInspector]
    public State _goal;

    [HideInInspector]
    public Sprite[] _sprites;
    [HideInInspector]
    public PuzzleLayout _puzzleLayout;
    [HideInInspector]
    public PuzzleLayout _puzzleLayout2;

    [HideInInspector]
    public GameObject _frame;
    public GameObject _frame2;
    private int ID = 0;

    public AudioClip audioWin;
    public AudioClip audioHint;
    public AudioClip audioCancel;
    public AudioClip audioClickTile;
    public AudioClip audioRandomize;

    public GameObject FillerGameObject;
    [HideInInspector]
    public Image Filler;

    [HideInInspector]
    public AudioSource audioSource;

    public FixedButton buttonStart;
    public FixedButton buttonHint;
    public FixedButton buttonRetry;
    public FixedButton buttonBack;
    public FixedButton buttonNext;
    public FixedButton buttonCancel;
    public FixedButton buttonCompare;
    public FixedButton buttonNextAStar;
    public FixedButton buttonPrevAStar;
    public FixedButton buttonNextSol;
    public FixedButton buttonPrevSol;

    public Text textYourScore;
    public Text textAStarScore;
    public Text totalScore;

    [HideInInspector]
    public int yourScore = 0;
    [HideInInspector]
    public int yourTotalScore = 0;

    private Patterns.FiniteStateMachine _fsm;

    public int MaxImageCount = 20;

    [HideInInspector]
    public bool m_astarSolved = false;
    [HideInInspector]
    public List<Node> m_astarSolution = new List<Node>();
    [HideInInspector]
    public List<State> astarSolutionStates = new List<State>();
    [HideInInspector]
    public int m_astarSolutionIndex = 0;

    [HideInInspector]
    public List<State> yourSolutionStates = new List<State>();

    [HideInInspector]
    public GameObject _puzzleSet1;
    [HideInInspector]
    public GameObject _puzzleSet2;

    public GameObject image_sol_you;
    public GameObject image_sol_astar;
    public Image image_message;

    public Text textMessage;

#if FARAMIRA_USE_ADS
    #region Unity Ads
    public readonly string GameID_Android = "3605936";
    public readonly string GameID_iOS = "3605937";
    public readonly string placementId = "ingame_banner";

    private readonly bool testMode = true;

    string adsPlacementRewarded = "rewardedVideo";

    public enum AdRunningState
    {
        RUNNING,
        FAILED,
        SUCCESS,
        NO_AD_STARTED,
    }
    [HideInInspector]
    public AdRunningState adState = AdRunningState.NO_AD_STARTED;
    #endregion
#endif

    //public BottomMenu mBottomMenu;
    public GameMenuHandler mGameMenuHandler;
    public SceneAmbientSound mAmbientSound;

    // Start is called before the first frame update
    void Awake()
    {
        _puzzleSet1 = new GameObject("PuzzleSet1");
        _puzzleSet2 = new GameObject("PuzzleSet2");

        Filler = FillerGameObject.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        _frame = PuzzleLayout.CreateFrame(FrameName1);
        _puzzleLayout = new PuzzleLayout(170.0f, 170.0f, PuzzleRowsOrCols);
        _frame.transform.SetParent(_puzzleSet1.transform);
        _puzzleLayout.gameObject.transform.SetParent(_puzzleSet1.transform);

        _frame2 = PuzzleLayout.CreateFrame(FrameName2);
        _frame2.transform.SetParent(_puzzleSet2.transform);
        _puzzleLayout2 = new PuzzleLayout(170.0f, 170.0f, PuzzleRowsOrCols);
        _puzzleLayout2.gameObject.transform.SetParent(_puzzleSet2.transform);

        _puzzleSet2.transform.position = new Vector3(0.0f, -600.0f, 0.0f);
        _puzzleSet2.SetActive(false);

        ID = Random.Range(0, MaxImageCount);
        LoadPuzzleImage();

        Neighbours.Instance.CreateGraphForNPuzzle(PuzzleRowsOrCols);

        _fsm = new Patterns.FiniteStateMachine();
        _fsm.Add(new GameState_FADEIN(_fsm, (int)GameState.StateID.FADEIN, this));
        _fsm.Add(new GameState_WAIT(_fsm, (int)GameState.StateID.WAIT, this));
        _fsm.Add(new GameState_PLAYING(_fsm, (int)GameState.StateID.PLAYING, this));
        _fsm.Add(new GameState_WIN(_fsm, (int)GameState.StateID.WIN, this));
        _fsm.Add(new GameState_SHOW_AD(_fsm, (int)GameState.StateID.SHOW_AD, this));
        _fsm.Add(new GameState_COMPARE(_fsm, (int)GameState.StateID.COMPARE, this));
        _fsm.Add(new GameState_RANDOMIZE(_fsm, (int)GameState.StateID.RANDOMIZE, this));
        _fsm.Add(new GameState_NEXT_PUZZLE_IMAGE(_fsm, (int)GameState.StateID.NEXT_PUZZLE_IMAGE, this));
        _fsm.Add(new GameState_SHOW_HINT(_fsm, (int)GameState.StateID.SHOW_HINT, this));
        _fsm.Add(new GameState_CANCEL(_fsm, (int)GameState.StateID.CANCEL, this));
        _fsm.Add(new GameState_SHOW_REWARD_AD(_fsm, (int)GameState.StateID.SHOW_REWARD_AD, this));
        _fsm.Add(new GameState_ASTAR_SOLUTION(_fsm, (int)GameState.StateID.ASTAR_SOLUTION, this));

        _fsm.SetCurrentState((int)GameState.StateID.FADEIN);
        audioSource.Play();

        //mBottomMenu = GameApp.Instance.mBottomMenu;
        //mBottomMenu.SetActive(true);
        //mBottomMenu.btnNext.gameObject.SetActive(false);
        //mBottomMenu.btnPrev.onClick.AddListener(LoadMenu);
        mGameMenuHandler.SetActiveBtnHome(true);

#if FARAMIRA_USE_ADS
        // initialize ADs
#if UNITY_IPHONE
        Advertisement.AddListener (this);
        Advertisement.Initialize(GameID_iOS, testMode);
#endif

#if UNITY_ANDROID
        Advertisement.AddListener (this);
        Advertisement.Initialize(GameID_Android, testMode);
#endif
#endif
    }

#if FARAMIRA_USE_ADS
    public void Start()
    {
        Debug.Log("Unity Ads initialized: " + Advertisement.isInitialized);
        Debug.Log("Unity Ads is supported: " + Advertisement.isSupported);
    }

    public void ShowInterstitialAds()
    {
        Advertisement.Show();
    }
#endif

    public void LoadPuzzleImage()
    {
        if (ID >= MaxImageCount)
        {
            ID = 0;
        }
        _sprites = Utils.LoadPuzzleImageAsSprites(ID, PuzzleRowsOrCols);
        _puzzleLayout.SetSprites(_sprites);
        _puzzleLayout2.SetSprites(_sprites);
        //ID++;
        int index = Random.Range(0, MaxImageCount);
        while(index == ID)
        {
            index = Random.Range(0, MaxImageCount);
        }
        ID = index;

        //mBottomMenu.SetActive(false);
        mAmbientSound.ChangeAudio();
    }

    // Update is called once per frame
    void Update()
    {
        if(_fsm != null)
        {
            _fsm.Update();
        }
    }

    // coroutine to swap tiles smoothly
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    public void SwapTiles(int first, State puzzle, bool useCoroutine)
    {
        int second = puzzle.GetEmptyTileIndex();

        // swap positions.
        Vector3 newPos = _puzzleLayout.Tiles[puzzle.Arr[second]].transform.position;
        newPos.z = 0.0f;
        Vector3 newEmptyPos = _puzzleLayout.Tiles[puzzle.Arr[first]].gameObject.transform.position;
        newEmptyPos.z = 1.0f;
        _puzzleLayout.Tiles[puzzle.Arr[second]].transform.position = newEmptyPos;

        if (useCoroutine)
        {
            IEnumerator coroutine = MoveOverSeconds(_puzzleLayout.Tiles[puzzle.Arr[first]], newPos, 0.2f);
            StartCoroutine(coroutine);
        }
        else
        {
            _puzzleLayout.Tiles[puzzle.Arr[first]].transform.position = newPos;
        }
        puzzle.SwapWithEmpty(first);
    }

    public void SwapTilesBackground(int first, State puzzle, bool useCoroutine)
    {
        int second = puzzle.GetEmptyTileIndex();
        puzzle.SwapWithEmpty(first);
    }

    //public void LoadMenu()
    //{
    //    audioSource.Stop();
    //    // show popup.
    //    SceneManager.LoadScene("MainMenu");
    //}

    private static bool IsStateInList(State state, List<Node> li)
    {
        int i = 0;
        for (; i < li.Count; ++i)
        {
            if (State.Equals(state, li[i].State))
                return true;
        }
        return false;
    }

    private IEnumerator SearchUsingAStar(State start, State goal)
    {
        m_astarSolutionIndex = 0;
        m_astarSolution.Clear();
        m_astarSolved = false;

        PriorityQueue openlist = new PriorityQueue();
        List<Node> closedlist = new List<Node>();

        Node root = new Node(start, 0, null);
        root.Parent = null;

        openlist.Add(root);
        closedlist.Add(root);

        while (openlist.Count > 0 && !m_astarSolved)
        {
            Node current = openlist.GetAndRemoveTop();

            if (State.Equals(current.State, goal))
            {
                // fil the solution.
                Node s = current;
                do
                {
                    m_astarSolution.Add(s);
                    s = s.Parent;
                } while (s != null);


                m_astarSolved = true;
                m_astarSolutionIndex = m_astarSolution.Count - 1;
                Debug.Log("Solution found.." + "Total moves needed = " + m_astarSolutionIndex);
                textAStarScore.text = m_astarSolutionIndex.ToString();

                textMessage.text += " Click on the second button to see the best solution generated by ASTAR.";

                buttonCancel.gameObject.SetActive(true);
                break;
            }

            int zero = current.State.FindEmptyTileIndex();
            int[] neighbours = Neighbours.Instance.GetNeighbors(zero);

            foreach (int next in neighbours)
            {
                State state = new State(current.State);
                SwapTilesBackground(next, state, false);

                if (!IsStateInList(state, closedlist))
                {
                    Node n = new Node(state, current.Depth + 1);
                    n.Parent = current;
                    openlist.Add(n);
                    closedlist.Add(n);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        astarSolutionStates.Clear();
        for (int i = m_astarSolution.Count - 1; i >= 0; --i)
        {
            astarSolutionStates.Add(new State(m_astarSolution[i].State));
        }

        //_layout.SetState(_solution[0].State);
        Debug.Log("Least number of moves: " + m_astarSolutionIndex);
    }

    public void StartAStarSearchInBackground(State start)
    {
        IEnumerator func = SearchUsingAStar(start, _goal);
        StartCoroutine(func);
    }


#if FARAMIRA_USE_ADS
    public void ShowSolutionAd()
    {
        Advertisement.Show(adsPlacementRewarded);
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            adState = AdRunningState.SUCCESS;
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
            adState = AdRunningState.FAILED;
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
            adState = AdRunningState.FAILED;
        }
    }
#endif

    private IEnumerator ShowTextMessage_Coroutine(float fadeInDuration = 2.0f, float showDuration = 3.0f, float fadeOutDuration = 2.0f)
    {
        float dt = 0.0f;

        textMessage.gameObject.SetActive(true);
        Color c = textMessage.color;
        Color nc = new Color(c.r, c.g, c.b, 0.0f);

        while (dt < (fadeInDuration + showDuration + fadeOutDuration))
        {
            dt += Time.deltaTime;

            if(dt <= fadeInDuration)
            {
                nc.a = dt / fadeInDuration;
            }
            else if(dt > fadeInDuration && dt <= (fadeInDuration + showDuration))
            {
                nc.a = 1.0f;
            }
            else if (dt > (fadeInDuration + showDuration) && dt <= (fadeInDuration + showDuration + fadeOutDuration))
            {
                nc.a = 1.0f - (dt - (fadeInDuration + showDuration)) / fadeOutDuration;
            }
            textMessage.color = nc;
            //Debug.Log("color: " + nc.ToString() + " | dt: " + dt);

            yield return new WaitForEndOfFrame();
        }
        textMessage.gameObject.SetActive(false);
    }

    public void ShowTextMessage(string text)
    {
        textMessage.text = text;
        //StartCoroutine(ShowTextMessage_Coroutine());
    }
}
