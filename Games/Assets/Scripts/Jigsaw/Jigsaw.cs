using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Patterns;
using UnityEngine.SceneManagement;

public class Jigsaw : MonoBehaviour
{
    SplitImage mSplitImage = new SplitImage();

    public string mImageFilename;
    public SpriteRenderer mSpriteRenderer;
    public int mSortingLayer;
    public Transform TilesParent;
    public Button mPlayButton;
    public List<Rect> mRegions = new List<Rect>();
    public Material mShadowMaterial;
    public FixedButton mShowImage;

    public Text mTextTotalTiles;
    public Text mTextInPlaceTiles;
    public Text mTextTime;

    public static TilesSorting sTilesSorting = new TilesSorting();
    public static bool sCameraPanning = true;

    private FiniteStateMachine mFsm = new FiniteStateMachine();
    enum GameStates
    {
        LOADING,
        SHUFFLING,
        PLAYING,
        WIN,
        SHOW_SOLUTION,
    }

    int mTotalTilesInCorrectPosition = 0;

    #region Jigsaw Game Data
    #endregion

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        mImageFilename = GameApp.Instance.JigsawImageFilename;

        mSplitImage.mImageFilename = mImageFilename;
        mSplitImage.mSpriteRenderer = mSpriteRenderer;
        mSplitImage.TilesParent = TilesParent;
        mSplitImage.mShadowMaterial = mShadowMaterial;
        //mSplitImage.mSortingLayer = mSortingLayer;

        mFsm.Add(new State((int)GameStates.LOADING, OnEnterLoading, null, null, null));
        mFsm.Add(new State((int)GameStates.SHUFFLING, OnEnterShuffling, null, null, null));
        mFsm.Add(new State((int)GameStates.PLAYING, OnEnterPlaying, null, OnUpdatePlaying, null));
        mFsm.Add(new State((int)GameStates.WIN, OnEnterWin, null, null, null));
        mFsm.Add(new State((int)GameStates.SHOW_SOLUTION, OnEnterShowSolution, OnExitShowSolution, OnUpdateShowSolution, null));

        mFsm.SetCurrentState((int)GameStates.LOADING);
    }

    void OnDestroy()
    {
        sTilesSorting.Clear();
        if (mFsm.GetCurrentState().ID == (int)GameStates.PLAYING)
            mSplitImage.SaveGame();
    }

    bool LoadLevel()
    {
        // Load data asscociated with the game.
        if (!mSplitImage.LoadGame())
        {
            mSplitImage.CreateJigsawTiles();

            for (int i = 0; i < mSplitImage.mTilesX; i++)
            {
                for (int j = 0; j < mSplitImage.mTilesY; ++j)
                {
                    SplitTile tile = mSplitImage.mGameObjects[i, j].GetComponent<SplitTile>();
                    tile.mOnSetCorrectPosition += OnSetCorrectPosition;
                    sTilesSorting.Add(tile);
                }
            }
        }
        else
        {
            // directly go to PLAY mode.
            mPlayButton.gameObject.SetActive(false);
            mFsm.SetCurrentState((int)GameStates.PLAYING);

            for (int i = 0; i < mSplitImage.mTilesX; i++)
            {
                for (int j = 0; j < mSplitImage.mTilesY; ++j)
                {
                    SplitTile tile = mSplitImage.mGameObjects[i, j].GetComponent<SplitTile>();
                    tile.mOnSetCorrectPosition += OnSetCorrectPosition;
                    sTilesSorting.Add(tile);
                    if (tile.IsInCorrectPosition())
                    {
                        OnSetCorrectPosition(tile);
                    }
                }
            }
        }

        mTextTotalTiles.text = (mSplitImage.mTilesX * mSplitImage.mTilesY).ToString();

        StartCoroutine(Coroutime_Timer());
        return false;
    }

    void OnSetCorrectPosition(SplitTile tile)
    {
        mTotalTilesInCorrectPosition += 1;
        tile.enabled = false;
        sTilesSorting.Remove(tile);
        tile.SetRenderOrder(1);
        tile.mSpriteRenderer.sortingLayerName = "TilesInPlace";
        if (mTotalTilesInCorrectPosition == mSplitImage.mGameObjects.Length)
        {
            mFsm.SetCurrentState((int)GameStates.WIN);
        }
        mTextInPlaceTiles.text = mTotalTilesInCorrectPosition.ToString();
    }

    void OnEnterLoading()
    {
        mPlayButton.gameObject.SetActive(true);
        LoadLevel();
    }

    void OnEnterShuffling()
    {
        Shuffle();
    }

    void OnEnterPlaying()
    {
        mShowImage.gameObject.SetActive(true);
    }

    void OnUpdatePlaying()
    {
        if (mShowImage.Pressed)
        {
            mFsm.SetCurrentState((int)GameStates.SHOW_SOLUTION);
        }
    }

    void OnEnterWin()
    {
        mPlayButton.gameObject.SetActive(true);
        mShowImage.gameObject.SetActive(false);
    }

    void OnEnterShowSolution()
    {
        mSplitImage.ShowNonTransparentImage();
    }

    void OnExitShowSolution()
    {
        mSplitImage.ShowTransparentImage();
    }

    void OnUpdateShowSolution()
    {
        if (!mShowImage.Pressed)
        {
            mFsm.SetCurrentState((int)GameStates.PLAYING);
        }
    }

    public void OnClickShowImageButton()
    {
        mFsm.SetCurrentState((int)GameStates.SHOW_SOLUTION);
    }

    public void OnClicplPlayButton()
    {
        mFsm.SetCurrentState((int)GameStates.SHUFFLING);
    }

    IEnumerator Coroutine_Shuffle()
    {
        for (int i = 0; i < mSplitImage.mTilesX; ++i)
        {
            for (int j = 0; j < mSplitImage.mTilesY; ++j)
            {
                Shuffle(mSplitImage.mGameObjects[i, j]);
                yield return null;
            }
        }
    }

    void Shuffle(GameObject obj)
    {
        // determine the final position of the tile after shuffling.
        // which region.
        int regionIndex = Random.Range(0, mRegions.Count);
        // get a random point within the region.
        float x = Random.Range(mRegions[regionIndex].xMin, mRegions[regionIndex].xMax);
        float y = Random.Range(mRegions[regionIndex].yMin, mRegions[regionIndex].yMax);

        // final position of the tile.
        Vector3 pos = new Vector3(x, y, 0.0f);

        StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
    }

    // coroutine to swap tiles smoothly
    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
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

    IEnumerator Coroutine_Delay(float duration)
    {
        yield return new WaitForSeconds(duration);
        mPlayButton.gameObject.SetActive(false);
        mFsm.SetCurrentState((int)GameStates.PLAYING);
    }

    void Shuffle()
    {
        StartCoroutine(Coroutine_Shuffle());
        StartCoroutine(Coroutine_Delay(1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        mFsm.Update();
    }

    IEnumerator Coroutime_Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            mSplitImage.mSecondsSinceStart += 1;

            System.TimeSpan t = System.TimeSpan.FromSeconds(mSplitImage.mSecondsSinceStart);

            string time = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            mTextTime.text = time;
        }
    }

    public void LoadJigsawMenu()
    {
        SceneManager.LoadScene("JigsawMenu");
    }
}
