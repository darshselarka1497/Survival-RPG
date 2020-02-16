using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class Save : MonoBehaviour
{
    private SaveFile save;

    void Start()
    {
        save = new SaveFile();
    }
    //saves player stats and items, toolbar slots, and profile
    public void saveGame()
    {
        Player player = Player.Instance;
        save.items = PlayerItem.Instance.ItemList;
        save.stats = player.GetComponent<PlayerStats>().PStats;
        save.position = player.transform.position;
        save.toolbarItems = Toolbar.Instance.ItemList;
        save.toolbarIndex = Toolbar.Instance.ToolbarIndex;
        save.equippedWeapon = Toolbar.Instance.EquippedWeapon;
        save.equipment = Profile.Instance.Equipment;
        save.sceneName = SceneLoad.Instance.ActiveSceneName;

        save.hunger = player.getHunger();
        save.health = player.getHealth();
        save.thirst = player.getThirst();
        save.maxHealth = player.MaxHealth;
        
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string json = JsonConvert.SerializeObject(save, Formatting.Indented, settings);
        print(Application.dataPath);
        Directory.CreateDirectory(Application.dataPath + "/Saves");
        File.WriteAllText(Application.dataPath + "/Saves/save.json", json);
    }
}
