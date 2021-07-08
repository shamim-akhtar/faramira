using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the facilitate sorting of tiles.
public class TilesSorting
{
    private List<SplitTile> mSortIndices = new List<SplitTile>();
    public TilesSorting()
    {

    }

    public void Clear()
    {
        mSortIndices.Clear();
    }

    public void Add(SplitTile ren)
    {
        mSortIndices.Add(ren);
        ren.SetRenderOrder(mSortIndices.Count);
    }

    public void Remove(SplitTile ren)
    {
        mSortIndices.Remove(ren);
        for(int i = 0; i < mSortIndices.Count; ++i)
        {
            mSortIndices[i].SetRenderOrder(i + 1);
        }
    }

    public void BringToTop(SplitTile ren)
    {
        // Find the index of ren.
        Remove(ren);
        Add(ren);
    }
}
