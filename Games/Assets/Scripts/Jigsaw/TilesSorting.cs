using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the facilitate sorting of tiles.
public class TilesSorting
{
    private List<SpriteRenderer> mSortIndices = new List<SpriteRenderer>();
    public TilesSorting()
    {

    }

    public void Add(SpriteRenderer ren)
    {
        mSortIndices.Add(ren);
        ren.sortingOrder = mSortIndices.Count;
    }

    public void Remove(SpriteRenderer ren)
    {
        mSortIndices.Remove(ren);
        for(int i = 0; i < mSortIndices.Count; ++i)
        {
            mSortIndices[i].sortingOrder = i + 1;
        }
    }

    public void BringToTop(SpriteRenderer ren)
    {
        // Find the index of ren.
        Remove(ren);
        Add(ren);
    }
}
