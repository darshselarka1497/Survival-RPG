using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ItemDatabase : MonoBehaviour
{
    private Dictionary<int, Item> itemDict;
    private static ItemDatabase _instance;

    void Awake()
    {
        _instance = this;
    }
    //Need a better solution for loading item database...
    void Start()
    {
        itemDict = new Dictionary<int, Item>();
        //reads all the recipes from the json file and stores it
        
        var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        string json = Resources.Load<TextAsset>("Json/ItemDatabase").text;
        itemDict = JsonConvert.DeserializeObject<Dictionary<int, Item>>(json, settings);
    }
    public static ItemDatabase Instance
    {
        get 
        {
            return _instance;
        }
    }
    
    //creates an instance of the item in the database
    public Item createItem(int id)
    { 
        Item item;
        itemDict.TryGetValue(id, out item);
        return item.newItem();
    }
}
