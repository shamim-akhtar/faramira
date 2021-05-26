using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimerA : MonoBehaviour
{
    public Text mTextTimer;
    public Image mBackground;
    public AudioSource mAudioSource;
    public AudioClip mAudioClip;

    public delegate void DelegateOnFinishCountDown();
    public DelegateOnFinishCountDown OnFinishCountDown;

    public IEnumerator Coroutine_StartCountDown(float totalTime)
    {
        mBackground.gameObject.SetActive(true);
        mTextTimer.gameObject.SetActive(true);
        int t = (int)totalTime;
        while (t != 0)
        {
            mTextTimer.text = t.ToString();
            StartCoroutine(AmbientSound.Coroutine_PlayShot(mAudioSource, mAudioClip));
            yield return new WaitForSeconds(1.0f);
            t -= 1;
        }
        mTextTimer.gameObject.SetActive(false);
        mBackground.gameObject.SetActive(false);
        OnFinishCountDown?.Invoke();
    }

    public void StartCountDown(float totalTime)
    {
        StartCoroutine(Coroutine_StartCountDown(totalTime));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
