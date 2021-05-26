using System.IO;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System.Linq;

namespace Puzzle
{
    public class Utils
    {

        public static void SetImageTransparency(Image image, float a)
        {
            Color c = image.color;
            c.a = a;
            image.color = c;
        }
        public static Sprite LoadNewSprite(Texture2D SpriteTexture, int x, int y, int w, int h, float PixelsPerUnit = 1.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(x, y, w, h), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
            return NewSprite;
        }
        public static Texture2D LoadTexture(string resourcePath)
        {
            Texture2D tex = Resources.Load<Texture2D>(resourcePath);
            return tex;
        }

        public static Texture2D LoadTextureFromFile(string FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(16, 16);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }

        public static Sprite[] LoadImageAndCreateSprites(Texture2D SpriteTexture, int rows, int cols, int padding = 1)
        {
            int twidth = SpriteTexture.width / cols;
            int theight = SpriteTexture.height / rows;

            Sprite[] sprites = new Sprite[rows * cols];

            int x, y;
            for (int i = 0; i < cols; i++)
            {
                y = (rows-1-i) * theight;
                for (int j = 0; j < rows; j++)
                {
                    x = j * twidth;
                    int index = i * cols + j;
                    sprites[index] = Utils.LoadNewSprite(SpriteTexture, x, y, twidth - padding, theight - padding);
                }
            }
            return sprites;
        }

        public static Sprite[] LoadPuzzleImageAsSprites(int id, int PuzzleRowsOrCols)
        {
            int padding = 4;

            int[] arr = new int[PuzzleRowsOrCols * PuzzleRowsOrCols];
            for (int i = 0; i < PuzzleRowsOrCols * PuzzleRowsOrCols; i++)
            {
                arr[i] = i;
            }

            string FilePath = "Textures\\" + id.ToString();
            Texture2D EmptyTexture = Utils.LoadTexture("Textures\\empty");

            Texture2D SpriteTexture = Utils.LoadTexture(FilePath);
            Assert.IsNotNull(SpriteTexture);

            int twidth = SpriteTexture.width / PuzzleRowsOrCols;
            int theight = SpriteTexture.height / PuzzleRowsOrCols;

            float tile_w = twidth;
            float tile_h = theight;

            // load the image and split into multiple sprites.
            Sprite[] sprites = Utils.LoadImageAndCreateSprites(SpriteTexture, PuzzleRowsOrCols, PuzzleRowsOrCols, padding);
            Sprite empty_sprite = Utils.LoadNewSprite(EmptyTexture, 0, 0, twidth, theight);
            sprites[sprites.Length - 1] = empty_sprite;

            return sprites;
        }

        public static GameObject Pick2D()
        {
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (hit)
            {
                return hit.transform.gameObject;
            }
            else return null;
        }

        public static GameObject Pick3D()
        {
            Ray rayPos = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool flag = Physics.Raycast(rayPos, out hit);

            if (flag)
            {
                return hit.transform.gameObject;
            }
            else return null;
        }

        public static bool IsPointerOverGameObject(GameObject gameObject)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults.Any(x => x.gameObject == gameObject);
        }


        //Fade in and Fade out of UI items
        public static IEnumerator Coroutine_FadeIn(Image img, float t = 1.0f)
        {
            // loop over 1 second
            for (float i = 0; i <= t; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(img.color.r, img.color.g, img.color.b, i);
                yield return null;
            }
        }

        // Fade in and Fade out of UI items
        public static IEnumerator Coroutine_FadeOut(Image img, float t = 1.0f)
        {
            // loop over 1 second
            for (float i = t; i >= 0; i -= Time.deltaTime)
            {
                img.color = new Color(img.color.r, img.color.g, img.color.b, i);
                yield return null;
            }
        }
    }
}