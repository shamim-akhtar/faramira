using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasConfirmA : MonoBehaviour
{
    public Button mYes;
    public Button mNo;
    public Image mBackground;
    public Image mForeground;

    public delegate void DelegateOnClick();
    public DelegateOnClick onClickYes;
    public DelegateOnClick onClickNo;

    public delegate void DelegateOnSceneExiting(float time);
    public DelegateOnSceneExiting OnSceneExiting;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickNo()
    {
        gameObject.SetActive(false);
        onClickNo?.Invoke();
    }

    //Fade in and Fade out of UI items
    public IEnumerator Coroutine_ScenExiting1(float t = 1.0f)
    {
        // loop over 1 second
        for (float i = 0; i <= t; i += Time.deltaTime)
        {
            // set color with i as alpha
            mForeground.color = new Color(mForeground.color.r, mForeground.color.g, mForeground.color.b, i);
            OnSceneExiting?.Invoke(1.0f - i);
            yield return null;
        }
    }

    //Fade in and Fade out of UI items
    public IEnumerator Coroutine_ScenExiting(float t = 1.0f)
    {
        float startTime = Time.time;
        float dt = Time.time - startTime;
        while(dt < t)
        {
            dt = Time.time - startTime;
            // set color with i as alpha
            float factor = dt / t;
            mForeground.color = new Color(mForeground.color.r, mForeground.color.g, mForeground.color.b, factor);
            OnSceneExiting?.Invoke(1.0f - factor);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Coroutine_OnClickYes()
    {
        //yield return Puzzle.Utils.Coroutine_FadeIn(mBackground);
        //yield return Puzzle.Utils.Coroutine_FadeIn(mForeground);
        yield return Coroutine_ScenExiting();
        onClickYes?.Invoke();
    }

    public void OnClickYes()
    {
        StartCoroutine(Coroutine_OnClickYes());
    }
}
