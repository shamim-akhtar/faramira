using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string GameToLoad;

    public Button btnPrev;
    public Button btnNext;
    public Button btnTrophy;
    public Button btnSound;
    public Button btnLeader;

    // Start is called before the first frame update
    void Start()
    {
        btnPrev.gameObject.SetActive(false);
        btnNext.gameObject.SetActive(false);
        btnTrophy.gameObject.SetActive(false);
        btnSound.gameObject.SetActive(true);
        btnLeader.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick_Play_TicTacToe()
    {
        GameToLoad = "TicTacToe";
        btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Play_8Puzzle()
    {
        GameToLoad = "8Puzzle";
        btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Next()
    {
        if(GameToLoad != null)
        {
            SceneManager.LoadScene(GameToLoad);
        }
    }
}
