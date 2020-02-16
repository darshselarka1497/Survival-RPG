using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Weapon : Item
{
    [SerializeField]
    private float minAttack;
    [SerializeField]
    private float maxAttack;
    [SerializeField]
    private float attack;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float critChance;
    [SerializeField]
    private float critDamage;
    
    public Weapon(int id, string name, string type, bool stackable, int quantity, string description, float minAttack,
    float maxAttack, float attack, float attackSpeed, float critChance, float critDamage) : base(id,name,type,stackable,quantity,description)
    {
        //generates random values for minattack, maxattack, and attack
        //within a certain range of the passed in values
        this.minAttack = minAttack;
        this.maxAttack = maxAttack;
        this.attack = attack;
        this.attackSpeed = attackSpeed;
        this.critChance = critChance;
        this.critDamage = critDamage;
    }

    public float MinAttack
    {
        get{return minAttack;}
    }
    public float MaxAttack
    {
        get{return maxAttack;}
    }
    public float Attack
    {
        get{return attack;}
    }
    public float AttackSpeed
    {
        get{return attackSpeed;}
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
        float newMinAttack = Mathf.Round(Random.Range(Mathf.Ceil(minAttack * .7f), minAttack));
        float newMaxAttack = Mathf.Round(Random.Range(maxAttack, Mathf.Ceil(maxAttack * 1.3f)));
        float newAttack = Mathf.Round(Random.Range(Mathf.Floor(attack * .7f), Mathf.Ceil(attack * 1.3f)));
        return new Weapon(id,name,type,stackable,quantity,description,newMinAttack,newMaxAttack,newAttack,attackSpeed,critChance,critDamage);
    }
    public override string display()
    {
        string itemInfo = 
        "Name: " + Name +
        "\nType: " + Type +
        "\nQuantity: " + Quantity +
        "\nWeapon Attack: " + minAttack + "~" + maxAttack +
        "\nAttack: " + attack +
        "\nAttackSpeed: " + attackSpeed +
        "\nCrit Chance: " + critChance + "%" +
        "\nCrit Damage: " + critDamage + "%" +
        "\nDescription: " + Description;
        return itemInfo;
        
    }

}
