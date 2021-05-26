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

    [HideInInspector]
    public bool mShowingExitPopup = false;
    public bool mOpaqueBackground = false;

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
        mShowingExitPopup = true;
        if(mOpaqueBackground)
        {
            mConfirmExit.mBackground.gameObject.SetActive(true);
        }
    }

    public void DisableExitMenu()
    {
        mConfirmExit.gameObject.SetActive(false);
        mShowingExitPopup = false;
        if (mOpaqueBackground)
        {
            mConfirmExit.mBackground.gameObject.SetActive(false);
        }
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
