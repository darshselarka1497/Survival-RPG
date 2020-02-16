using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    private float minWeaponAttack = 0;
    private float maxWeaponAttack = 0;
    private float attack = 0;
    private float attackSpeed = 0;
    private float defense = 0;
    private float critChance = 5;
    private float critDamage = 150;
    private float totalExp = 0;
    private float currentExp = 0;
    private float level = 1;
    private float expToNxtLv = 0;
    /*
    private int hunger = 100; //hunger counter(100=full)
    private int thirst = 100; //thirst counter(100=full)
    private int fatigue = 0; //fatigue counter(100=fatigued)
    private int health = 100000; //health bar(0=death)
    private int maxHunger = 100;
    private int maxThirst = 100;
    private int maxFatigue = 100;
    private int maxHealth = 100000;
    */

    public Stats(){}

    public float MinWeaponAttack
    {
        get{return minWeaponAttack;}
        set{minWeaponAttack = value;}
    }
    public float MaxWeaponAttack
    {
        get{return maxWeaponAttack;}
        set{maxWeaponAttack = value;}
    }
    public float Attack
    {
        get{return attack;}
        set{attack = value;}
    }
    public float AttackSpeed
    {
        get{return attackSpeed;}
        set{attackSpeed = value;}
    }
    public float Defense
    {
        get{return defense;}
        set{defense = value;}
    }
    public float CritChance
    {
        get{return critChance;}
        set{critChance = value;}
    }
    public float CritDamage
    {
        get{return critDamage;}
        set{critDamage = value;}
    }
    public float TotalExp
    {
        get{return totalExp;}
        set{totalExp = value;}
    }
    public float CurrentExp
    {
        get{return currentExp;}
        set{currentExp = value;}
    }
    public float Level
    {
        get{return level;}
        set{level = value;}
    }
    public float ExpToNxtLv
    {
        get{return expToNxtLv;}
        set{expToNxtLv = value;}
    }
    /*
    public int Hunger
    {
        get{return hunger;}
        set{hunger = value;}
    }
    public int MaxHunger
    {
        get{return maxHunger;}
        set{maxHunger = value;}
    }
    public int Thirst
    {
        get{return thirst;}
        set{thirst = value;}
    }
    public int MaxThirst
    {
        get{return maxThirst;}
        set{maxThirst = value;}
    }
    public int Fatigue
    {
        get{return fatigue;}
        set
        {
            fatigue = value;
            if (fatigue < 0)
            {
                fatigue = 0;
            } 
            else if (fatigue > 100)
            {
                fatigue = 100;
            }
        }
    }
    public int MaxFatigue
    {
        get{return maxFatigue;}
        set{maxFatigue = value;}
    }
    public int Health
    {
        get{return health;}
        set{health = value;}
    }
    public int MaxHealth
    {
        get{return maxHealth;}
        set{maxHealth = value;}
    }
    */
}
