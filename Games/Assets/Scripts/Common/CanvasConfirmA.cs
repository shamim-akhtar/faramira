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

    IEnumerator Coroutine_OnClickYes()
    {
        //yield return Puzzle.Utils.Coroutine_FadeIn(mBackground);
        yield return Puzzle.Utils.Coroutine_FadeIn(mForeground);
        onClickYes?.Invoke();
    }

    public void OnClickYes()
    {
        StartCoroutine(Coroutine_OnClickYes());
    }
}
