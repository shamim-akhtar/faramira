using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuHandler : MonoBehaviour
{
    public BottomMenu mBottomMenu;
    public CanvasConfirmA mConfirmExit;

    public delegate void DelegateOnClickNextGame();
    public DelegateOnClickNextGame onClickNextGame;

    void Start()
    {
        mConfirmExit.onClickYes = LoadMenu;
        mConfirmExit.onClickNo = DisableExitMenu;

        mBottomMenu.btnPrev.gameObject.SetActive(true);
        mBottomMenu.btnNext.gameObject.SetActive(false);
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
        onClickNextGame?.Invoke();
    }

    public void OnClickExitGame()
    {
        mConfirmExit.gameObject.SetActive(true);
    }

    public void DisableExitMenu()
    {
        mConfirmExit.gameObject.SetActive(false);
    }

    public void SetActiveBtnNext(bool flag)
    {
        mBottomMenu.btnNext.gameObject.SetActive(flag);
    }

    public void SetActiveBtnHome(bool flag)
    {
        mBottomMenu.btnPrev.gameObject.SetActive(flag);
    }
}
