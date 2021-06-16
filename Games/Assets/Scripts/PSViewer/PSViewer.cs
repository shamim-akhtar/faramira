using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSViewer : MonoBehaviour
{
    public PSManager mPSManager;
    public GameMenuHandler mMenuHandler;

    int id = 0;

    void Start()
    {
        mMenuHandler.onClickNextGame += NextEffect;
        PlayEffect();
        mMenuHandler.SetActiveBtnNext(true);
    }

    public void NextEffect()
    {
        id++;
        if (id == mPSManager.MaxID) id = 0;
        //mPSManager.DestroyCurrentPS();
        mPSManager.ShowEFX(id, Vector3.zero, 5.0f);
    }

    public void PrevEffect()
    {
        //mPSManager.DestroyCurrentPS();

        if (id == 0) id = mPSManager.MaxID;
        id -= 1;
        mPSManager.ShowEFX(id, Vector3.zero, 5.0f);
    }

    public void PlayEffect()
    {
        //mPSManager.DestroyCurrentPS();
        mPSManager.ShowEFX(id, Vector3.zero, 5.0f);
    }
}
