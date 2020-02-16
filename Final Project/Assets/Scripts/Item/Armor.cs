using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Armor : Item
{
    [SerializeField]
    private float defense;
    [SerializeField]
    private float critChance;
    [SerializeField]
    private float critDamage;
    public Armor(int id, string name, string type, bool stackable, int quantity, string description, float defense, float critChance, 
    float critDamage) : base(id,name,type,stackable,quantity,description)
    {
        this.defense = defense;
        this.critChance = critChance;
        this.critDamage = critDamage;
    }
    public float Defense
    {
        get{return defense;}
    }
    public float CritChance
    {
        get{return critChance;}
    }
    public float CritDamage
    {
        get{return critDamage;}
    }

    public override Item newItem()
    {
        return new Armor(id,name,type,stackable,quantity,description,defense,critChance,critDamage);
    }
    //displays the stat of the armor
    public override string display()
    {
        string itemInfo = 
        "Name: " + Name +
        "\nType: " + Type +
        "\nQuantity: " + Quantity +
        "\nDefense: " + defense +
        "\nCrit Chance:" + critChance + "%" +
        "\nCrit Damage:" + critDamage + "%" +
        "\nDescription: " + Description;
        return itemInfo;
    }
}
