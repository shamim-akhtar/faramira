using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSourceGetSpectrumDataExample : MonoBehaviour
{
    public AudioSource mAudioSource;
    public AudioClip mClip;
    bool mDisplay = false;

    public Text mDisplayText;

    public float RmsValue;
    public float DbValue;
    public float PitchValue;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private float mMinFreq = Mathf.Infinity;
    private float mMaxFreq = -Mathf.Infinity;

    public GameObject mTile;

    private List<SpriteRenderer> mImages = new List<SpriteRenderer>();
    int mLastId = 0;

    int X = 10;
    int Y = 1;

    void Start()
    {
        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;

        float SPRITE_W = 1.0f;
        float SPRITE_H = 1.0f;
        for(int i = 0; i < X; ++i)
        {
            for (int j = 0; j < Y; ++j)
            {
                string name = i + "_" + j;
                GameObject go = Instantiate(mTile);
                SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
                go.transform.position = new Vector3(i * SPRITE_W, j * SPRITE_H, 0.0f);

                // randomize sprite color.
                float r = Random.Range(0, 1.0f);
                float g = Random.Range(0, 1.0f);
                float b = Random.Range(0, 1.0f);

                spriteRenderer.color = new Color(r, g, b, 1.0f);
                go.SetActive(false);

                mImages.Add(spriteRenderer);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            mAudioSource.volume = 1.0f;
            mAudioSource.loop = true;
            mAudioSource.clip = mClip;
            mAudioSource.Play();
        }
        AnalyzeSound();

        if (mDisplayText != null)
        {
            mDisplayText.text = "RMS: " + RmsValue.ToString("F2") + "\n" +
            " (" + DbValue.ToString("F1") + " dB)\n" +
            "Pitch: " + PitchValue.ToString("F0") + " Hz\n" +
            "MinP: " + mMinFreq.ToString("F0") + " Hz\n" +
            "MaxP: " + mMaxFreq.ToString("F0") + " Hz";

        }
    }

    void AnalyzeSound()
    {
        mAudioSource.GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
                                            // get sound spectrum
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { 
            // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { 
            // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency

        if (PitchValue < mMinFreq) mMinFreq = PitchValue;
        if (PitchValue > mMaxFreq) mMaxFreq = PitchValue;

        InstantiateNotes(PitchValue);
    }
    //Fade in and Fade out of UI items

    IEnumerator Coroutine_EnableForDuration(SpriteRenderer img)
    {
        //img.gameObject.SetActive(true);
        yield return StartCoroutine(Puzzle.Utils.Coroutine_FadeIn(img, 0.1f));
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(Puzzle.Utils.Coroutine_FadeOut(img, 0.1f));
        //img.gameObject.SetActive(false);
    }


    public float duration = 0.1f;
    float mLastTime = 0.0f;
    void InstantiateNotes(float v)
    {
        int group = 1 + (int)(mMaxFreq / (mImages.Count - 1));
        int index = 0;
        bool flag = true;
        while(flag)
        {
            if (v > index * group) index++;
            else flag = false;
        }
        int id = index;

        if (id != mLastId && Time.time - mLastTime > duration)
        {
            //Debug.Log("Group: " + group + " ID: " + id + "mMaxFreq: " + mMaxFreq + "F: " + v);
            //Debug.Log("ID: " + id);

            SpriteRenderer img = mImages[mLastId];
            img.gameObject.SetActive(false);
            //StartCoroutine(Puzzle.Utils.Coroutine_FadeOut(img, 0.2f));
            //StartCoroutine(Coroutine_EnableForDuration(img));
            mImages[id].gameObject.SetActive(true);
            //StartCoroutine(Puzzle.Utils.Coroutine_FadeIn(mImages[id], 0.2f));
            mLastId = id;
            mLastTime = Time.time;
        }
    }
}