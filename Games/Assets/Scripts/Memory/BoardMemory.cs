using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

    private void Awake()
    {
        mCardLayout.Add(new Tuple<int, int>(2, 2));
        mCardLayout.Add(new Tuple<int, int>(3, 2));
        mCardLayout.Add(new Tuple<int, int>(4, 2));
        mCardLayout.Add(new Tuple<int, int>(4, 3));
        mCardLayout.Add(new Tuple<int, int>(4, 4));
        mCardLayout.Add(new Tuple<int, int>(4, 5));
        //mCardLayout.Add(new Tuple<int, int>(4, 6));
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel(mLevel - 1);
    }

    //private void CreateSprite(GameObject gameObject, int id)
    //{
    //    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    //    //var sprite = Resources.Load<Sprite>("Images/Memory/2D");
    //    var sprite = Resources.Load<Sprite>("Images/Memory/" + SPRITE_NAMES[id]);
    //    sprite.name = SPRITE_NAMES[id];
    //    spriteRenderer.sprite = sprite;
    //    spriteRenderer.name = SPRITE_NAMES[id];
    //}

    //private void CreateCardMask(GameObject gameObject, int id)
    //{
    //    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    //    var sprite = Resources.Load<Sprite>("Images/Memory/" + SPRITE_MASK_NAMES[id]);
    //    sprite.name = SPRITE_MASK_NAMES[id];
    //    spriteRenderer.sprite = sprite;
    //    spriteRenderer.name = SPRITE_MASK_NAMES[id];
    //}

    Sprite LoadSprite(string name)
    {
        Sprite sp = Resources.Load<Sprite>(name);
        if(sp != null)
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
            mCards[i] = null;
            mCardMasks[i].transform.parent = null;
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
        //DestroyAll();

        if(level >= mCardLayout.Count)
        {
            Debug.Log("Completed all levels for this game.");
            return;
        }

        Tuple<int, int> layout = mCardLayout[level];

        float total_w = SPRITE_W * layout.Item1;
        float total_h = SPRITE_H * layout.Item2;

        float start_x = -total_w / 2.0f + SPRITE_W / 2.0f;
        float start_y = -total_h / 2.0f + SPRITE_H / 2.0f;
        for (int i = 0; i < layout.Item1; ++i)
        {
            for(int j = 0; j < layout.Item2; ++j)
            {
                string name = i + "_" + j;
                GameObject go = new GameObject(name);
                go.AddComponent<SpriteRenderer>();
                go.transform.position = new Vector3(i * SPRITE_W + start_x, j * SPRITE_H + start_y, 0.0f);
                go.transform.parent = transform;
                mCards.Add(go);

                GameObject gom = new GameObject(name + "_mask");
                gom.AddComponent<SpriteRenderer>();
                gom.transform.position = new Vector3(i * SPRITE_W + start_x, j * SPRITE_H + start_y, -1.0f);
                gom.transform.parent = transform;
                mCardMasks.Add(gom);
            }
        }

        int maskRand = UnityEngine.Random.Range(0, SPRITE_MASK_NAMES.Length - 1);
        mCardMaskSprite = LoadSprite("Images/Memory/" + SPRITE_MASK_NAMES[maskRand]);

        for (int i = 0; i < mCards.Count; i+=2)
        {
            int rand = UnityEngine.Random.Range(0, SPRITE_NAMES.Length - 1);
            Sprite sp = LoadSprite("Images/Memory/" + SPRITE_NAMES[rand]);
            mCardSprites.Add(sp);
            mCardSprites.Add(sp);
        }

        Camera.main.orthographicSize = total_w + 1.0f + mLevel * 1.2f;

        // shuffle the cards.
        Shuffle();
    }

    IEnumerator Coroutine_SetSpritesToCards()
    {
        for (int i = 0; i < mCards.Count; ++i)
        {
            var spriteRenderer = mCards[i].GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = mCardSprites[i];
            spriteRenderer.name = mCardSprites[i].name;
            yield return new WaitForSeconds(0.05f);
            //yield return null;
        }
    }

    IEnumerator Coroutine_SetSpritesToCardMasks()
    {
        for (int i = 0; i < mCards.Count; ++i)
        {
            var spriteRenderer = mCardMasks[i].GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = mCardMaskSprite;
            //spriteRenderer.name = mCardMaskSprite.name;
            //yield return new WaitForSeconds(0.05f);
            yield return null;
        }
    }

    void Shuffle()
    {
        for (int i = 0; i < mCards.Count; ++i)
        {
            var spriteRenderer = mCards[i].GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = null;
        }
        var shuffledGameObjects = mCards.OrderBy(a => rng.Next()).ToList();
        mCards = shuffledGameObjects;
        StartCoroutine(Coroutine_SetSpritesToCards());
    }

    void ApplyMasks()
    {
        StartCoroutine(Coroutine_SetSpritesToCardMasks());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shuffle();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ApplyMasks();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CreateLevel(UnityEngine.Random.Range(0, mCardLayout.Count - 1));
        }
    }
}
