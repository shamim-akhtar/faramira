using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Maze_Level : MonoBehaviour
{
    public BottomMenu mBottomMenu;
    public CanvasConfirmA mConfirmExit;

    void Start()
    {
        mConfirmExit.onClickYes = LoadMenu;
        mConfirmExit.onClickNo = DisableExitMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClickNextGame()
    {
    }

    public void OnClickExitGame()
    {
        mConfirmExit.gameObject.SetActive(true);
        PauseGame();
    }

    public void DisableExitMenu()
    {
        mConfirmExit.gameObject.SetActive(false);
        ResumeGame();
    }

    void PauseGame()
    {
        //Time.timeScale = 0;
    }

    void ResumeGame()
    {
        //Time.timeScale = 1;
    }
}
