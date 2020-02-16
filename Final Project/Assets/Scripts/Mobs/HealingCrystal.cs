using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCrystal : MonoBehaviour
{
    private MobStats boss;

    // Update is called once per frame
    void Update()
    {
        boss.updateHealth(boss.MaxHealth*.01f*Time.deltaTime);
    }   
    public void setBoss(MobStats boss)
    {
        this.boss = boss;
    }

}
