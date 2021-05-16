using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    // All public variables to be changed at runtime
    public GameObject menu;
    public GameObject buttons;
    public GameObject pannel;
    public GameObject dialogs;

    public GameObject popup1;
    public GameObject popup2;


    public GameObject R1, R2, R3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // back function for button to go to main screen
    public void back() {
        // get only one screen active and other inactive
        menu.SetActive(true);// get only one screen active and other inactive
        buttons.SetActive(false);// get only one screen active and other inactive
        pannel.SetActive(false);// get only one screen active and other inactive
        dialogs.SetActive(false);// get only one screen active and other inactive
    }
    //To go on buttons screen
    public void buttonsM()
    {
        // get only one screen active and other inactive
        menu.SetActive(false);
        buttons.SetActive(true);
        pannel.SetActive(false);
        dialogs.SetActive(false);
    }
    //togo on pannel screen
    public void pannelM()
    {
        // get only one screen active and other inactive
        menu.SetActive(false);
        buttons.SetActive(false);
        pannel.SetActive(true);
        dialogs.SetActive(false);
    }
    //togo on dialog screen
    public void dialogM()
    {
        // get only one screen active and other inactive
        menu.SetActive(false);
        buttons.SetActive(false);
        pannel.SetActive(false);
        dialogs.SetActive(true);
    }
    //radio buttons functionality

    // if one button is clicked others are set tobe in active
    public void RadioButtons1() {
    // if one button is clicked others are set tobe in active
        R1.transform.GetChild(0).transform.gameObject.SetActive(true);
    // if one button is clicked others are set tobe in active
        R2.transform.GetChild(0).transform.gameObject.SetActive(false);
    // if one button is clicked others are set tobe in active
        R3.transform.GetChild(0).transform.gameObject.SetActive(false);
    }
    public void RadioButtons2()
    {
    // if one button is clicked others are set tobe in active
        R1.transform.GetChild(0).transform.gameObject.SetActive(false);
    // if one button is clicked others are set tobe in active
        R2.transform.GetChild(0).transform.gameObject.SetActive(true);
    // if one button is clicked others are set tobe in active
        R3.transform.GetChild(0).transform.gameObject.SetActive(false);

    }
    public void RadioButtons3()
    {
    // if one button is clicked others are set tobe in active
        R1.transform.GetChild(0).transform.gameObject.SetActive(false);
    // if one button is clicked others are set tobe in active
        R2.transform.GetChild(0).transform.gameObject.SetActive(false);
    // if one button is clicked others are set tobe in active
        R3.transform.GetChild(0).transform.gameObject.SetActive(true);

    }
    //show or hide pop up menu
    public void POPUP1() {
        //if menu is opened
        if (popup1.transform.GetChild(0).transform.gameObject.activeSelf)
        {
            //close the menu by setting the values to set active false
            for (int i=0; i< popup1.transform.childCount-1; i++)
            {
                popup1.transform.GetChild(i).transform.gameObject.SetActive(false);
            }
        }
        //other vise set the gameobjects to true
        else {
            for (int i = 0; i < popup1.transform.childCount; i++)
            {
                popup1.transform.GetChild(i).transform.gameObject.SetActive(true);
            }
        }

    }
    //show or hide pop up 2ndmenu
    public void POPUP2()
    {
        //if menu is opened
        if (popup2.transform.GetChild(0).transform.gameObject.activeSelf)
        {
            //close the menu by setting the values to set active false
            for (int i = 0; i < popup2.transform.childCount - 1; i++)
            {
                popup2.transform.GetChild(i).transform.gameObject.SetActive(false);
            }
        }
        //other vise set the gameobjects to true
        else
        {
            for (int i = 0; i < popup2.transform.childCount; i++)
            {
                popup2.transform.GetChild(i).transform.gameObject.SetActive(true);
            }
        }

    }
}
