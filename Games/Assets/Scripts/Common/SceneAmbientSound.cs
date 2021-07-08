using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAmbientSound : AmbientSound
{
    public AudioClip[] mAudioClips;
    public CanvasConfirmA mConfirmExit;
    public bool mPlaySoundOnStart = false;
    public bool mShuffleAudio = false;

    int mIndex = 0;
    void OnSceneExitFadeOut(float t)
    {
        AudioSource audioSource;
        if (active1) audioSource = mAudioSource1;
        else audioSource = mAudioSource2;
        audioSource.volume *= t;

        //Debug.Log("Volume: " + audioSource.volume);
    }

    private void Start()
    {
        AudioListener.volume = 0.8f;
        if (mConfirmExit != null)
        {
            mConfirmExit.OnSceneExiting += OnSceneExitFadeOut;
        }

        if (mPlaySoundOnStart)
        {
            mIndex= Random.Range(0, mAudioClips.Length);
            Play(mAudioClips[mIndex]);
        }

        if (mShuffleAudio)
        {
            StartCoroutine(Coroutine_ShuffleAudio());
        }
    }

    public void ChangeAudio()
    {
        int index = Random.Range(0, mAudioClips.Length);
        while(mIndex == index)
        {
            index = Random.Range(0, mAudioClips.Length);
        }
        mIndex = index;
        base.Play(mAudioClips[mIndex]);
    }

    IEnumerator Coroutine_ShuffleAudio()
    {
        while(true)
        {
            float secs = Random.Range(60, 180);
            yield return new WaitForSeconds(secs);
            ChangeAudio();
        }
    }
}
