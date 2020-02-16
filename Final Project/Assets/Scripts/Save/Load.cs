using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Load : MonoBehaviour
{
    [SerializeField]
    private Text error;
    private SaveFile save;
    public void loadNewGame()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(loadingNewGame());
    }
    IEnumerator loadingNewGame()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("InitialScene");
        while (!sceneLoading.isDone)
        {
            yield return null;
        }
        SceneManager.LoadScene("M1Scene");
        List<Item> startingItems = new List<Item>();
        startingItems.Add(ItemDatabase.Instance.createItem(1));
        startingItems.Add(ItemDatabase.Instance.createItem(2));
        startingItems.Add(ItemDatabase.Instance.createItem(300));
        startingItems.Add(ItemDatabase.Instance.createItem(301));
        startingItems.Add(ItemDatabase.Instance.createItem(302));
        startingItems.Add(ItemDatabase.Instance.createItem(303));
        startingItems.Add(ItemDatabase.Instance.createItem(304));
        startingItems.Add(ItemDatabase.Instance.createItem(305));
        startingItems.Add(ItemDatabase.Instance.createItem(306));
        startingItems.Add(ItemDatabase.Instance.createItem(500));
        PlayerItem.Instance.loadItems(startingItems);
        Destroy(this.gameObject);

    }
    public void loadGame()
    {
        try
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string json = File.ReadAllText(Application.dataPath + "/Saves/save.json");
            save = JsonConvert.DeserializeObject<SaveFile>(json, settings);
            if (save != null)
            {
                DontDestroyOnLoad(this.gameObject);
                StartCoroutine(loading());
            }
            else
            {    
                error.transform.parent.gameObject.SetActive(true);
                error.text = "Corrupt Save File";
            }
        }
        catch
        {   
            error.transform.parent.gameObject.SetActive(true);
            error.text = "No Save File Found";
        }
    }
    IEnumerator loading()
    {
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync("InitialScene");
        while (!sceneLoading.isDone)
        {
            yield return null;
        }
        Player player = Player.Instance;
        player.transform.position = save.position;
        player.GetComponent<PlayerStats>().PStats = save.stats;
        player.setHealth(save.health);
        player.setHunger(save.hunger);
        player.setThirst(save.thirst);
        player.setFatigue(save.fatigue);
        player.MaxHealth = save.maxHealth;

        Profile.Instance.loadEquips(save.equipment);
        Toolbar.Instance.loadToolbar(save.toolbarItems, save.toolbarIndex, save.equippedWeapon);
        PlayerItem.Instance.loadItems(save.items);
        SceneLoad.Instance.load(save.sceneName);
        Destroy(this.gameObject);
    }

    public void turnOffError()
    {
        error.transform.parent.gameObject.SetActive(false);
    }

}
