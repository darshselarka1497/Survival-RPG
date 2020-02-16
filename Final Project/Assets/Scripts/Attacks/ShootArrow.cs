using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
{
    private MobStats mob;
    private string arrowType;
    void update()
    {

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        string tag = collider.gameObject.tag;
        //destroys arrow if it hits an object that it cannot pass through
        if (tag == "Tree" || tag == "Ore" || tag == "Collidable")
        {
            Destroy(this.gameObject);
        }
        if (arrowType == "player")
        {
            checkEnemyHit(tag,collider);
        }
        else if (arrowType == "enemy")
        {
            checkPlayerHit(tag,collider);
        }
    }
    void checkEnemyHit(string tag, Collider2D collider)
    {
        switch(tag)
        {
            /*
            case "Boss":
                collider.gameObject.GetComponent<Boss1>().updateHealth(-50);
                Destroy(this.gameObject);
                break;
                */
            case "Mob":
                PlayerStats pStat = Player.Instance.gameObject.GetComponent<PlayerStats>();
                DamageCalc.Instance.playerToMob(pStat, collider.gameObject.GetComponent<MobStats>());
                Destroy(this.gameObject);
                break;
        }
    }
    public void setArrowOwnerMob(MobStats mob)
    {
        this.mob = mob;
    }
    void checkPlayerHit(string tag, Collider2D collider)
    {   
        if (tag == "Player")
        {
            DamageCalc.Instance.mobToPlayer(collider.GetComponent<PlayerStats>(), mob);
            Destroy(this.gameObject);
        }
    }
    public void setType(string arrowType)
    {
        this.arrowType = arrowType;
    }
}
