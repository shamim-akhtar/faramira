using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string GameToLoad;

    // Start is called before the first frame update
    void Start()
    {
        GameApp.Instance.mBottomMenu.btnPrev.gameObject.SetActive(false);
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(false);
        GameApp.Instance.mBottomMenu.btnTrophy.gameObject.SetActive(false);
        GameApp.Instance.mBottomMenu.btnSound.gameObject.SetActive(true);
        GameApp.Instance.mBottomMenu.btnLeader.gameObject.SetActive(false);

        GameApp.Instance.mBottomMenu.btnNext.onClick.AddListener(OnClick_Next);
    }

    private void OnDisable()
    {
        GameApp.Instance.mBottomMenu.btnNext.onClick.RemoveListener(OnClick_Next);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_Play_TicTacToe()
    {
        GameToLoad = "TicTacToe";
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Play_8Puzzle()
    {
        GameToLoad = "8Puzzle";
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Next()
    {
        if(GameToLoad != null)
        {
            SceneManager.LoadScene(GameToLoad);
        }
    }
}
