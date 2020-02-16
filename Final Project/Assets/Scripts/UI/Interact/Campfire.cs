using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : Interact
{
    [SerializeField]
    private GameObject fadeScreen;
    private Image blackScreen;
    // Start is called before the first frame update
    void Start()
    {
        menuOptions = new string[]{"Sleep", "Cancel"};
        buttonActions = new List<ButtonActions>();
        buttonActions.Add(sleep);
        buttonActions.Add(cancel);
    }

    //updates player survival stats when the player clicks sleep
    void sleep()
    {
        cancel();
        StartCoroutine(fadeTime());
    }


    IEnumerator fadeTime()
    {
        Fade fade = Instantiate(fadeScreen).GetComponent<Fade>();
        fade.setFadeTimer(3,3,0,0);
        StartCoroutine(fade.fadeIn());
        yield return new WaitForSecondsRealtime(3.0f);

        //updates player save spot and survival stats
        Player player = Player.Instance;
        SceneLoad.Instance.setSaveSpot(player.transform.position);
        player.updateFatigue(-50);
        player.updateHunger(-50);   
        player.updateThirst(-50); 

        StartCoroutine(fade.fadeOut());

    }
}
