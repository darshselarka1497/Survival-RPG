using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCalc : MonoBehaviour
{
    private static DamageCalc _instance;
    void Awake()
    {
        _instance = this;
    }
    public static DamageCalc Instance
    {
        get{return _instance;}
    }
    //calculates player damage to mob and updates mob's health
    public void playerToMob(PlayerStats player, MobStats mob)
    {
        bool criticalHit = false;
        //random value between min and max weapon attack
        float randomWeaponAttack = Random.Range(player.PStats.MinWeaponAttack,player.PStats.MaxWeaponAttack);
        
        //takes the log of the difference between attack and defense
        //and multiplies it by weapon attack and crit damage if a 
        //critical hit occured
        float damage = randomWeaponAttack * player.PStats.Attack/(1+mob.Defense);
        criticalHit = Random.Range(0,100) < player.PStats.CritChance;
        if (criticalHit)
        {
            damage *= player.PStats.CritDamage/100.0f;
        }
        MobUI.Instance.setDamageLayout(mob);
        mob.updateHealth(-damage);
        StartCoroutine(MobUI.Instance.displayDamage(mob, damage, criticalHit));
    }

    public void mobToPlayer(PlayerStats player, MobStats mob)
    {        
        float randomWeaponAttack = Random.Range(mob.MinAttack,mob.MaxAttack);
        
        //takes the log of the difference between attack and defense
        //and multiplies it by weapon attack and crit damage if a 
        //critical hit occured
        float damage = randomWeaponAttack*mob.Attack/(1+player.PStats.Defense);
        Player.Instance.updateHealth(-(int)Mathf.Ceil(damage));
    }
    
}
