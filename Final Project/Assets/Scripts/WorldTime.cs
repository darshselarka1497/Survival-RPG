using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTime : MonoBehaviour
{
    //Constants Variables
    private const float healthDecayTime = 1.0f;//interval between health decay
    private const float survivalDelay = 2.0f; //how often hunger/thirst are decreased in seconds
    private const float waitTimeDelay = 1.0f; //how fast the ingame time gets updated(currently 1minute=1second)

    private Player player;
    private bool survivalWait = false;
    private bool healthDecreaseDelay = false; //wait for health delay timer

    //ingame time variables(24 hour clock)
    [SerializeField]
    private GameObject clock;
    private Text clockText;
    private int hour;
    private int minute;
    private bool timeDelay = false;
    
    void Awake()
    {
        clockText = clock.GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (survivalWait == false)
        {
            StartCoroutine(survivalUpdate());
        }

        //Decreases health over time if fatigue/hunger/thirst is 0 or less
        if (healthDecreaseDelay == false && (player.getFatigue() >= 100 || 
        player.getHunger() <= 0 || player.getThirst()  <= 0))
        {
            StartCoroutine(healthDecay());
        }

        //updates the ingame clock if a certain amount of time has passed
        if (timeDelay == false)
        {
            StartCoroutine(updateTime());
        }

        //displays the ingame time as a UI text component
        if (minute < 10) 
        {
            clockText.text = hour.ToString() + ":0" + minute.ToString();
        }
        else
        {
            clockText.text = hour.ToString() + ":" + minute.ToString();
        }
    }

    IEnumerator survivalUpdate()
    {
        survivalWait = true;
        yield return new WaitForSeconds(survivalDelay);
        player.updateHunger(-1);
        player.updateThirst(-1);
        survivalWait = false;
    }

    IEnumerator healthDecay()
    {
        healthDecreaseDelay = true;
        yield return new WaitForSeconds(healthDecayTime);
        player.updateHealth(-(int)(player.MaxHealth*.01f));
        healthDecreaseDelay = false;

    }
    IEnumerator updateTime() 
    {
        timeDelay = true;
        yield return new WaitForSeconds(waitTimeDelay);
        minute++;
        if (minute >= 60)
        {
            minute = 0;
            hour++;
        }
        if (hour >= 24)
        {
            hour = 0;
        }
        timeDelay = false;
    }
}
