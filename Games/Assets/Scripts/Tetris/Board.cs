using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{ 
    public class Board : MonoBehaviour
    {
        public static int width = 10;
        public static int height = 20;
        public Transform mSpawnPoint;

        public GameObject[] mBlockPrefabs;
        public Transform[, ] mGrid;

        public void InstantiateRandomizeBlock()
        {
            int index = Random.Range(0, mBlockPrefabs.Length);
            GameObject obj = Instantiate(mBlockPrefabs[index], mSpawnPoint.position, mSpawnPoint.rotation);
            Block blk = obj.GetComponent<Block>();
            if(blk != null)
            {
                blk.mBoard = this;
                if(!blk.ValidMove())
                {
                    Destroy(obj);
                    Lost();
                }
            }
        }

        public void AddToGrid(Block blk)
        {
            // Manual reverse iteration is possible using the GetChild() and childCount variables
            // Using normal forward traversal we cannot remove the parent.
            for (int i = blk.transform.childCount - 1; i >= 0; --i)
            {
                Transform child = blk.transform.GetChild(i);
                //child.SetParent(null, true);

                int x = Mathf.RoundToInt(child.transform.position.x);
                int y = Mathf.RoundToInt(child.transform.position.y);
                mGrid[x, y] = child;
            }
        }

        IEnumerator Coroutine_RemoveLine(int id, float dt = 0.2f)
        {
            for (float t = dt; t >= 0; t -= Time.deltaTime)
            {
                // apply fade out
                yield return null;
            }
            for (int i = 0; i < width; ++i)
            {
                Transform t = mGrid[i, id];
                t.SetParent(null, true);
                Destroy(t.gameObject);
                mGrid[i, id] = null;
            }
            // bring down all the above tiles.
            // how many blocks are there above this height.
            List<Block> blocks = new List<Block>();
            for (int j = id; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    if(mGrid[i, j] != null)
                    {
                        blocks.Add(mGrid[i, j].parent.GetComponent<Block>());
                        mGrid[i, j] = null;
                    }
                }
            }

            for(int i = 0; i < blocks.Count; ++i)
            {
                bool flag = true;
                while (flag)
                {
                    blocks[i].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                    if (!blocks[i].ValidMove())
                    {
                        blocks[i].transform.position -= new Vector3(0.0f, -1.0f, 0.0f);
                        flag = false;
                        AddToGrid(blocks[i]);
                    }
                }
                yield return null;
            }
            CheckAndRemoveLine();
        }

        void RemoveLine(int id)
        {
            StartCoroutine(Coroutine_RemoveLine(id));
        }

        public void CheckAndRemoveLine()
        {
            for (int j = 0; j < height; ++j)
            {
                bool removeLine = true;
                for (int i = 0; i < width; ++i)
                {
                    removeLine = removeLine && mGrid[i, j] != null;
                }
                if(removeLine)
                {
                    RemoveLine(j);
                    break;
                }
            }
        }

        void Lost()
        {
            Debug.Log("Game ended");
        }

        void Start()
        {
            mGrid = new Transform[width, height];
            InstantiateRandomizeBlock();
        }

        void Update()
        {
            // check for tiles removal.

        }
    }
}
