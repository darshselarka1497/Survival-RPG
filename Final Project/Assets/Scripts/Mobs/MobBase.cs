using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobBase : MonoBehaviour
{
    //pushing basic mob class because current more updated class has compile issues

    //movement will be based on pathfinding algorithm, not implemented yet

    public GameObject Player;
    private Vector2 playerLoc;
    //Damage, health, range assigned based on type of mob
    public static float Damage;
    private static float attackSpeed;
    public float Health;
    public static float Range;
    private float speed; 
    GameObject itemDrop;
    private static Vector2 startPos;
    private static float targetDistance; //distance player must be from mob for mob to engage
    private bool canAttack = false;
    private float distToPlayer;
    void Awake(){
        startPos = gameObject.transform.position;
        decideItemDrop();
        //decide health, damage, range, speed, attackspeed here too
    }

    void Update(){
        playerLoc = Player.transform.position;
        distToPlayer = Mathf.Sqrt(Mathf.Pow(startPos.y - playerLoc.y, 2) + Mathf.Pow(startPos.x - playerLoc.x, 2));
        checkForPlayer();

        if(canAttack){
            //use pathfinding algorithm to move to player
            if(distToPlayer < 2){ //2 will be changed through playtesting
                attack(); //delay attack using attack speed
            }
        }
    }

    void checkForPlayer(){
        if(distToPlayer < targetDistance){
            canAttack = true;
        } else if(distToPlayer < targetDistance + 3){// 3 is tentative to change
            canAttack = false;
            returnToStartPos();
        }
    }
    private void decideItemDrop(){
        //decides what item mob will drop, if any. Also based on type of mob
    }
    private void returnToStartPos(){
        //use pathfinding to return to starting position startPos
    }
    private void attack(){
        //take health from player   
    }
}
