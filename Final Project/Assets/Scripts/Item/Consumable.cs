using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Consumable : Item
{
    [SerializeField]
    private float hpRecovery;
    [SerializeField]
    private float hungerRecovery;
    public Consumable(int id, string name, string type, bool stackable, int quantity, string description, float hpRecovery,
    float hungerRecovery) : base(id,name,type,stackable,quantity,description)
    {
        this.hpRecovery = hpRecovery;
        this.hungerRecovery = hungerRecovery;
    }
    public float HpRecovery
    {
        get{return hpRecovery;}
    }
    public float HungerRecovery
    {
        get{return hungerRecovery;}
    }

    public override Item newItem()
    {
        return new Consumable(id,name,type,stackable,quantity,description,hpRecovery,hungerRecovery);
    }
    public override string display()
    {
        string itemInfo = 
        "Name: " + Name +
        "\nType: " + Type +
        "\nQuantity: " + Quantity +
        "\nHp Recovery: " + hpRecovery +
        "\nHunger Recovery: " + hungerRecovery +
        "\nDescription: " + Description;
        return itemInfo;
        
    }
}
