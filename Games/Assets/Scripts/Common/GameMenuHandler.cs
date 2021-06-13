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

    public bool mShowButton1_OnStart = true;
    public bool mShowButton2_OnStart = false;
    public bool mShowButton3_OnStart = false;
    public bool mShowButton4_OnStart = false;
    public bool mShowButton5_OnStart = false;
    //public bool mShowHomeOnStart = true;

    void Start()
    {
        mConfirmExit.onClickYes = LoadMenu;
        mConfirmExit.onClickNo = DisableExitMenu;

        mBottomMenu.btnPrev.gameObject.SetActive(mShowButton1_OnStart);
        mBottomMenu.btnTrophy.gameObject.SetActive(mShowButton2_OnStart);
        mBottomMenu.btnSound.gameObject.SetActive(mShowButton3_OnStart);
        mBottomMenu.btnLeader.gameObject.SetActive(mShowButton4_OnStart);
        mBottomMenu.btnNext.gameObject.SetActive(mShowButton5_OnStart);
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

    public void SetActiveBtnPrev(bool flag)
    {
        mBottomMenu.btnSound.gameObject.SetActive(flag);
    }

    public void SetActiveBtnPlay(bool flag)
    {
        mBottomMenu.btnTrophy.gameObject.SetActive(flag);
    }
}
