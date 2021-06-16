using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpriteUtils
{
    public static Sprite CreateSpriteFromTexture2D(
        Texture2D SpriteTexture, 
        int x, 
        int y, 
        int w, 
        int h, 
        float PixelsPerUnit = 1.0f, 
        SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        Sprite NewSprite = Sprite.Create(
            SpriteTexture, 
            new Rect(x, y, w, h), 
            new Vector2(0, 0), 
            PixelsPerUnit, 
            0, 
            spriteType);
        return NewSprite;
    }

    public static Texture2D LoadTexture(string resourcePath)
    {
        Texture2D tex = Resources.Load<Texture2D>(resourcePath);
        return tex;
    }

    public static Texture2D LoadTextureFromFile(string FilePath)
    {
        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(16, 16);
            if (Tex2D.LoadImage(FileData))
                return Tex2D;
        }
        return null;
    }
}
