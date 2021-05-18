using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Puzzle
{
    public class PuzzleLayout
    {
        private Dictionary<int, Vector3> Locations = new Dictionary<int, Vector3>();
        public GameObject gameObject { get; set; }

        public PuzzleLayout(float w, float h, int numRowsOrCols)
        {
            gameObject = new GameObject("PuzzleLayout");
            Tiles = new GameObject[numRowsOrCols * numRowsOrCols];
            for (int i = 0; i < numRowsOrCols * numRowsOrCols; i++)
            {
                Tiles[i] = new GameObject();
                Tiles[i].transform.SetParent(gameObject.transform);
            }

            TileWidth = w;
            TileHeight = h;

            float x, y;
            float startx = -TileWidth * numRowsOrCols / 2.0f;
            float starty = TileHeight * numRowsOrCols / 2.0f - TileHeight;
            for (int i = 0; i < numRowsOrCols; i++)
            {
                y = -i * TileHeight;
                for (int j = 0; j < numRowsOrCols; j++)
                {
                    x = j * TileWidth;
                    int index = i * numRowsOrCols + j;
                    Locations.Add(index, new Vector3(startx + x, y + starty, 0.0f));
                    Tiles[index].name = index.ToString();
                }
            }
            SetState(new Puzzle.State(numRowsOrCols));
        }

        public void SetActive(bool flag)
        {
            gameObject.SetActive(flag);
        }

        public float TileWidth
        {
            get;
        }
        public float TileHeight
        {
            get;
        }

        public GameObject[] Tiles
        {
            get;
            set;
        }

        public void SetState(State state)
        {
            for (int i = 0; i < state.Arr.Length; i++)
            {
                GameObject obj = Tiles[state.Arr[i]];
                obj.transform.localPosition = Locations[i];
            }
        }

        public void SetSprites(Sprite[] sprites, bool addBoxCollider = true)
        {
            Assert.AreEqual(sprites.Length, Tiles.Length);
            for(int i = 0; i < Tiles.Length; i++)
            {
                if(Tiles[i].GetComponent<SpriteRenderer>() == null)
                {
                    Tiles[i].AddComponent<SpriteRenderer>();
                }
                SpriteRenderer spriteRenderer = Tiles[i].GetComponent<SpriteRenderer>();
                Assert.IsNotNull(sprites[i]);
                spriteRenderer.sprite = sprites[i];

                if(addBoxCollider && Tiles[i].GetComponent<BoxCollider2D>() == null)
                {
                    BoxCollider2D box = Tiles[i].AddComponent<BoxCollider2D>();
                }
            }
        }

        public static GameObject CreateFrame(string frameName, float px = 0.0f, float py = 0.0f)
        {
            // create the frame.
            string framePath = "Textures\\" + frameName;
            Texture2D frameTexture = Utils.LoadTexture(framePath);

            int framesize = 550;

            GameObject frame = new GameObject("frameName");
            frame.transform.position = new Vector3(0.0f, 0.0f, 2.0f);
            frame.transform.localScale = new Vector3((float)framesize / (float)frameTexture.width,
                (float)framesize / (float)frameTexture.height, 0.0f);
            frame.transform.Translate(px - framesize/2.0f, py - framesize/2.0f, 0.0f);

            frame.AddComponent<SpriteRenderer>();
            SpriteRenderer spriteRenderer = frame.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(frameTexture);
            Sprite frameSprite = Utils.LoadNewSprite(frameTexture, 0, 0, frameTexture.width, frameTexture.height);
            spriteRenderer.sprite = frameSprite;
            return frame;
        }
    };
}