using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Interact : MonoBehaviour
{
    private bool menuOpened = false; //checks if the menu is opened
    [SerializeField]
    private GameObject menuPrefab; //menu prefab
    private GameObject menu; //instantiated menu object
    protected string[] menuOptions; //list of button texts
    protected delegate void ButtonActions(); //button actions function type
    protected List<ButtonActions> buttonActions; //list of functions for each button

    //default menu text and actions
    void Start()
    {
        menuOptions = new string[]{"Cancel"};
        buttonActions = new List<ButtonActions>();
        buttonActions.Add(cancel);
    }

    //opens menu when player left clicks and is colliding with object
    void OnCollisionStay2D(Collision2D collision)
    {
        if (Input.GetMouseButtonDown(0) && !menuOpened && Player.Instance.facingCollision() == this.gameObject)
        {
            Time.timeScale = 0;
            Transform canvas = GameObject.FindGameObjectWithTag("UI").transform;
            menu = Instantiate(menuPrefab, canvas);
            setMenuButton(menu);
            menuOpened = true;
        }
    }

    //sets menu button texts and click functions
    void setMenuButton(GameObject menu)
    {
        GameObject buttonPrefab = menu.transform.GetChild(0).gameObject;
        for (int i = 0; i < menuOptions.Length; i++)
        {
            
            int buttonIndex = i;
            GameObject button = Instantiate(buttonPrefab, menu.transform);
            button.SetActive(true);

            //adds the button function for each button
            button.GetComponent<Button>().onClick.AddListener(delegate{buttonActions[buttonIndex]();});
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = menuOptions[i];
        }
    }

    //deletes the menu
    protected void cancel()
    {
        Time.timeScale = 1;
        Destroy(menu);
        menuOpened = false;
    }
}
