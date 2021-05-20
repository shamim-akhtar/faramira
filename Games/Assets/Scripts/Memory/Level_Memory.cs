using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Memory : MonoBehaviour
{
    BottomMenu mBottomMenu;
    // Start is called before the first frame update
    void Start()
    {
        mBottomMenu = GameApp.Instance.mBottomMenu;
        mBottomMenu.SetActive(true);
        mBottomMenu.btnNext.gameObject.SetActive(false);
        mBottomMenu.btnPrev.onClick.AddListener(LoadMenu);
    }

    private void OnDisable()
    {
        mBottomMenu.btnPrev.onClick.RemoveListener(LoadMenu);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
