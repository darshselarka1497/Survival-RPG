using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SaveFile
{
    public List<Item> items;
    public Item[] toolbarItems;
    public Armor[] equipment;
    public int toolbarIndex;
    public Weapon equippedWeapon;
    public Stats stats;
    public Vector3 position;
    public string sceneName;
    public int health;
    public int maxHealth;
    public int hunger;
    public int thirst;
    public int fatigue;    
}
