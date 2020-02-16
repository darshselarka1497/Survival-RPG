
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BarGauge : MonoBehaviour
{
    private RectTransform barSize; //the bar's size based on the value
    [SerializeField]
    private GameObject barValue; //bar gameobject for value
    private Text barValueText; //bar text 
    [SerializeField]
    private int barType; //type of bar
    [SerializeField]
    private GameObject playerObject;
    private Player player;
    void Awake()
    {
        barSize = GetComponent<RectTransform>();
        barValueText = barValue.GetComponent<Text>();
        player = playerObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //gets the value for the bar gauge depending on what type it is
        //and displays it as a text
        int value = 0;
        if (barType == 0)
        {
            value = (int)(((float)player.getHealth()/player.MaxHealth)*100);
            barValueText.text = "Health: " + player.getHealth().ToString() + "/" + player.MaxHealth;

        }
        else if (barType == 1)
        {
            value = player.getHunger();
            barValueText.text = "Hunger: " + value.ToString() + "/100";
        }
        else if (barType == 2)
        {
            value = player.getThirst();
            barValueText.text = "Thirst: " + value.ToString() + "/100";
        }
        else if (barType == 3)
        {
            value = player.getFatigue();
            barValueText.text = "Fatigue: " + value.ToString() + "/100";
        }
        
        //decreases bar gauge based on its current value
        barSize.sizeDelta = new Vector2(value*3,barSize.sizeDelta.y);
    }
}
