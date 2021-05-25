using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string GameToLoad;

    BottomMenu mBottomMenu;
    // Start is called before the first frame update
    void Start()
    {
        mBottomMenu = GameApp.Instance.mBottomMenu;
        mBottomMenu.SetActive(true);
        mBottomMenu.btnPrev.gameObject.SetActive(false);
        mBottomMenu.btnNext.gameObject.SetActive(false);

        mBottomMenu.btnNext.onClick.AddListener(OnClick_Next);
    }

    private void OnDisable()
    {
        mBottomMenu.btnNext.onClick.RemoveListener(OnClick_Next);
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

    public void OnClick_Play_Maze()
    {
        GameToLoad = "Maze";
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Play_Memory()
    {
        GameToLoad = "Memory";
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Play_Tetris()
    {
        GameToLoad = "Tetris";
        GameApp.Instance.mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Play_Quiz()
    {
        GameToLoad = "Quiz";
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
