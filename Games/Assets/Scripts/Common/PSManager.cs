using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple facade to hide csShowAllEffect
/// </summary>
public class PSManager : MonoBehaviour
{
    public Effects.csShowAllEffect mEfx;

    public IEnumerator Coroutine_ShowEFX(int i, Vector3 position, float duration = 1.0f)
    {
        return mEfx.Coroutine_ShowEFX(i, position, duration);
    }

    public void ShowEFX(int i, Vector3 position, float duration = 0.2f)
    {
        mEfx.ShowEFX(i, position, duration);
    }

    public int MaxID {  get { return mEfx.Effect.Length; } }
}
