using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private Stats stats;
    private string[] statNames;
    // Start is called before the first frame update
    void Awake()
    {
        stats = new Stats();
        statNames = new string[]{"MinWeaponAttack", "MaxWeaponAttack", "Attack", "AttackSpeed", "Defense", "Crit Chance", "Crit Damage"};
    }
    void Start()
    {
        stats.ExpToNxtLv = calcExp();
        addExp(0);
    }
    public Stats PStats
    {
        get{return stats;}
        set{stats = value;}
    }
    //adds exp to the player and updates the exp bar
    public void addExp(float exp)
    {
        stats.CurrentExp += exp;
        stats.TotalExp += exp;
        ExperienceUI.Instance.updateExpBar(stats.CurrentExp, stats.ExpToNxtLv, (int)stats.Level);
        StartCoroutine(updateExpOverflow());
    }

    //updates level and next level exp when current exp exceed required exp
    //waits until the exp bar is finished animating before updating current exp
    IEnumerator updateExpOverflow()
    {
        while(stats.CurrentExp >= stats.ExpToNxtLv)
        {
            if (!ExperienceUI.Instance.Updating)
            {
                increaseLevel();
                stats.CurrentExp = stats.CurrentExp-stats.ExpToNxtLv;
                stats.ExpToNxtLv = calcExp();
                ExperienceUI.Instance.updateExpBar(stats.CurrentExp, stats.ExpToNxtLv, (int)stats.Level);
            }
            yield return null;
        }
    }

    void increaseLevel()
    {
        stats.Level++;
        Player.Instance.MaxHealth += (int)stats.Level * 10;
        stats.Attack += 1;
        stats.Defense += 1;
        if (stats.Level % 10 == 0)
        {
           
            stats.CritChance += 1;
            stats.CritDamage += 1;
        }
        Profile.Instance.setStats();
    }
    float calcExp()
    {
        return Mathf.Pow((stats.Level+5),3);
    }
    public void addArmorStats(Armor armor)
    {
        changeArmorStats(1, armor);
    }

    public void removeArmorStats(Armor armor)
    {
        changeArmorStats(-1, armor);
    }

    void changeArmorStats(int multiplier, Armor armor)
    {
        stats.Defense += multiplier*armor.Defense;
        stats.CritChance += multiplier*armor.CritChance;
        stats.CritDamage += multiplier*armor.CritDamage;
    } 
    public void addWeaponStats(Weapon weapon)
    {
        changeWeaponStats(1, weapon);
    }

    public void removeWeaponStats(Weapon weapon)
    {
        changeWeaponStats(-1, weapon);
    }

    void changeWeaponStats(int multiplier, Weapon weapon)
    {
        stats.MinWeaponAttack += multiplier*weapon.MinAttack;
        stats.MaxWeaponAttack += multiplier*weapon.MaxAttack;
        stats.Attack += multiplier*weapon.Attack;
        stats.AttackSpeed += multiplier*weapon.AttackSpeed;
        stats.CritChance += multiplier*weapon.CritChance;
        stats.CritDamage += multiplier*weapon.CritDamage;
    }
    public string[] getStatName()
    {
        return statNames;
    }
    public float[] getStatAmount()
    {
        return new float[]{stats.MinWeaponAttack,stats.MaxWeaponAttack,stats.Attack,
        stats.AttackSpeed,stats.Defense,stats.CritChance,stats.CritDamage};
    }
}
