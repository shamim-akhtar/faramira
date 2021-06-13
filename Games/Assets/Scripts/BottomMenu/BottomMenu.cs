using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BottomMenu : MonoBehaviour
{
    public Button btnPrev;
    public Button btnNext;
    public Button btnTrophy;
    public Button btnSound;
    public Button btnLeader;
    public Slider sliderVolume;

    // Start is called before the first frame update
    void Start()
    {
        //btnTrophy.gameObject.SetActive(false);
        //btnSound.gameObject.SetActive(false);
        //btnLeader.gameObject.SetActive(false);
        //sliderVolume.gameObject.SetActive(false);

        btnSound.onClick.AddListener(OnClick_Sound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool flag)
    {
        if (flag)
        {
            btnPrev.gameObject.SetActive(true);
            btnNext.gameObject.SetActive(true);
        }
        else
        {
            btnPrev.gameObject.SetActive(false);
            btnNext.gameObject.SetActive(false);
        }
    }

    public IEnumerator Coroutine_FadeIn(Image img, float t = 1.0f)
    {
        float dt = 0.0f;
        while (dt <= t)
        {
            dt += Time.deltaTime;
            Color c = img.color;
            c.a = dt / t;
            img.color = c;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnClick_Sound()
    {
        //sliderVolume.gameObject.SetActive(true);
        //StartCoroutine(Coroutine_FadeIn(sliderVolume.fillRect.GetComponent<Image>(), 1.0f));
        //StartCoroutine(Coroutine_FadeIn(sliderVolume.handleRect.GetComponent<Image>(), 1.0f));
    }
}
