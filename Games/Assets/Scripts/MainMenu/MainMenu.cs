using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    string GameToLoad;

    public BottomMenu mBottomMenu;
    public Transform mGameButtonList;
    public SceneAmbientSound mAmbientSound;
    public CanvasConfirmA mExitConfirm;

    void Start()
    {
        mBottomMenu.btnPrev.gameObject.SetActive(false);
        mBottomMenu.btnNext.gameObject.SetActive(false);

        //mBottomMenu.btnNext.onClick.AddListener(OnClick_Next);

        for(int i = 0; i <mGameButtonList.childCount; ++i)
        {
            Transform child = mGameButtonList.GetChild(i);
            Button btn = child.gameObject.GetComponent<Button>();
            if(btn != null)
            {
                btn.onClick.AddListener(OnClickPlayGame);
            }
        }

        mExitConfirm.onClickYes += OnClick_Next;
    }

    private void OnDisable()
    {
        //mBottomMenu.btnNext.onClick.RemoveListener(OnClick_Next);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlayGame()
    {
        GameToLoad = EventSystem.current.currentSelectedGameObject.name;
        mBottomMenu.btnNext.gameObject.SetActive(true);
    }

    public void OnClick_Next()
    {
        if(GameToLoad != null)
        {
            SceneManager.LoadScene(GameToLoad);
        }
    }
}
