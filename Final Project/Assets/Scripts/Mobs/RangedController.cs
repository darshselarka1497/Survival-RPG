using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedController : MobController
{
    [SerializeField]
    private float attackRange = 5.0f; //range that the mob can hit the player 
    [SerializeField]
    private ArrowFactory arrow; //arrow factory prefab
    [SerializeField]
    private float arrowSpeed = 5.0f; //speed of the arrow
    private float arrowImageOffset = 135.0f; //rotation offset of arrow image
    void Update()
    {
        //check if the player is in the mob's line of sight and within range
        Vector3 playerPos = player.transform.position;
        Vector3 mobPos = transform.position;
        Vector2 direction = new Vector2(playerPos.x-mobPos.x, playerPos.y-mobPos.y);
        Vector2 pos = new Vector2(mobPos.x, mobPos.y);
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, direction, attackRange);
        bool inLineOfSight = checkLineOfSight(hit);
        if(inLineOfSight && Vector3.Distance(playerPos, mobPos) < attackRange)
        {
            //checks if the mob is attacking and adjust the position it's facing to the player
            if (!attackCD)
            {
                facingDirection = playerPos - mobPos;
                animator.SetFloat("FacingX", facingDirection.x);
                animator.SetFloat("FacingY", facingDirection.y);
                StartCoroutine(arrowAttack(direction));
            }
        }
        else
        {
            //checks if player is within mob's aggro distance
            checkAggro(playerPos, mobPos);

            //normal mob behavior calls the parent class' walking and roaming behavior
            normalMobBehavior();
        }
        animator.SetBool("InAction", attackCD);
    }
    //creates an arrow and shoots it towards the players
    IEnumerator arrowAttack(Vector2 direction)
    {
        attackCD = true;
        animator.SetTrigger("Attack");
        animator.speed = attackSpeed;
        yield return new WaitForSeconds(1/(2*animator.speed));
        StartCoroutine(shootArrow(direction.normalized));
        yield return new WaitForSeconds(1/(2*animator.speed));
        attackCD = false;
        animator.speed=1;

    }
    //creates arrow and destroy it after a certain duration
    IEnumerator shootArrow(Vector2 direction)
    {
        GameObject newArrow = arrow.createEnemyArrow(transform.position, direction, arrowSpeed);
        newArrow.GetComponent<ShootArrow>().setArrowOwnerMob(GetComponent<MobStats>());
        yield return new WaitForSeconds(3.0f);
        Destroy(newArrow);
    }
    //checks if the player is in the mob's line of sight
    //and if there is an unpassable object between them
    bool checkLineOfSight(RaycastHit2D[] hit)
    {
        for (int i = 0; i < hit.Length; i++)
        {
            string tag = hit[i].collider.gameObject.tag;
            if (!arrow.isPassable(tag))
            {
                return false;
            }
            else if (tag == "Player")
            {
                return true;
            }

        }
        return false;

    }

}
