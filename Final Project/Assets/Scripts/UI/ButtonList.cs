using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonList : MonoBehaviour
{
    [SerializeField]
    private Text buttonText; //button text
    [SerializeField]
    private ButtonControl control; //button controller  
    private int buttonId; //index of recipe in recipe list
    private Button thisButton; //reference to this button
    void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(() => {OnClick();});
    }
    //set button name and id
    public void SetButton(string buttonName, int id)
    {
        buttonId = id;
        buttonText.text = buttonName;
    }
    
    void OnClick()
    {
        //creates text for recipe material and quantity
        control.buttonClick(buttonId);
    }
}
