using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSword : MonoBehaviour
{
    private GameObject boss;//boss gameobject for position
    private bool recalling = false;//checks if the boss is recalling the sword
    private float radius;//radius distance
    private float angle;//angle of sprite rotation and between boss from x axis
    private bool reachedTarget = false;//checks if reached target position
    private bool rotated = false; //checks if rotated towards boss
    private float originalAngle; //original angle when created
    private float rotationSpeed = 90.0f; //degrees to rotate per second
    private float recallSpeed; //speed of recall swords
    private Vector3 lastBossPos;//stores the last boss position before moving
    private bool damageCD = false; //cooldown between damage ticks
    private MobStats mobStats;
    private Vector3 targetDir;
    private bool shootAtTarget = false;
    private float recallChance;

    void FixedUpdate()
    {
        //destroys the swords if the boss dies
        if(boss == null || !boss.activeSelf)
        {
            StopAllCoroutines();
            Destroy(this.gameObject);
            return;
        }

        if (!shootAtTarget)
        {
            //offsets the position of the sword based on boss mvoement
            Vector3 offset = boss.transform.position - lastBossPos;
            transform.position += offset;
            lastBossPos = boss.transform.position;
        }
        if (shootAtTarget)
        {
            float speed = (mobStats.Health/mobStats.MaxHealth)*15;
            transform.position += targetDir.normalized*Time.deltaTime*(20-speed);
        }
        else if (recalling)
        {
            if (recallChance <= .33)
            {
                rotateToPlayer();
            }
            else
            {
                recallSword();
            }
        }
        else if (!reachedTarget)
        {
            moveToTarget();
        }
        else if (!rotated)
        {
            rotateTowardsBoss();
        }
        else
        {
            rotateAroundBoss();
        }

        
    }

    //sets all the constants and waits for the recall time
    public void release(GameObject boss, float recallTime, float recallSpeed, float angle, float radius, float rotationSpeed, float recallChance)
    {
        transform.Rotate(new Vector3(0,0,angle));
        this.lastBossPos = boss.transform.position;
        this.boss = boss;
        this.angle = angle;
        this.radius = radius;
        this.rotationSpeed = rotationSpeed;
        this.recallSpeed = recallSpeed;
        this.mobStats = boss.GetComponent<MobStats>();
        this.recallChance = recallChance;
        originalAngle = angle;
        StartCoroutine(releaseSword(recallTime));
    }
    IEnumerator releaseSword(float recallTime)
    {
        yield return new WaitForSeconds(recallTime);
        recalling = true;
    }
    IEnumerator destroyTime()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        shootAtTarget = true;
        yield return new WaitForSeconds(4.0f);
        Destroy(this.gameObject);
    }
    //moves the sword towards its target radius and angle position
    void moveToTarget()
    {
        float radians = Mathf.PI*angle/180;
        Vector2 posFromBoss = new Vector2(Mathf.Cos(radians)*radius, Mathf.Sin(radians)*radius);
        Vector3 targetPos = boss.transform.position+(Vector3)posFromBoss;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, .3f*recallSpeed); 

        if(Vector3.Distance(transform.position,targetPos) < 1)
        {
            reachedTarget = true;
        }

    }
    //rotates the sword image so that it's pointing at the boss
    void rotateTowardsBoss()
    {
        angle = Mathf.MoveTowardsAngle(angle, originalAngle + 180, 5f);
        transform.eulerAngles = new Vector3(0,0,angle);
        if(angle == originalAngle + 180)
        {
            rotated = true;
        }
    }
    //rotates the swords around the boss counterclockwise
    void rotateAroundBoss()
    {
        angle += Time.deltaTime*rotationSpeed;
        transform.eulerAngles = new Vector3(0,0,angle);
        float radians = Mathf.PI*(angle-180)/180;
        Vector2 posFromBoss = new Vector2(Mathf.Cos(radians)*radius, Mathf.Sin(radians)*radius);
        Vector3 targetPos = boss.transform.position+(Vector3)posFromBoss;
        transform.position = targetPos;
    }
    //recalls the sword by moving them towards the boss
    void recallSword()
    {
        //transform.eulerAngles = Vector3.RotateTowards(transform.position, recallTarget.transform.position,360,1f);
        transform.position = Vector3.MoveTowards(transform.position,boss.transform.position, .2f*recallSpeed);
        if (transform.position == boss.transform.position)
        {
            Destroy(this.gameObject);
        }
    }
    void rotateToPlayer()
    {
        targetDir = Player.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation =  Quaternion.RotateTowards(transform.rotation, targetRotation, 5f);
        if (Quaternion.Angle(transform.rotation,targetRotation) < .001f)
        {
            StartCoroutine(destroyTime());
        }
    }
    //checks for player hit
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !damageCD)
        {
            DamageCalc.Instance.mobToPlayer(collision.gameObject.GetComponent<PlayerStats>(), mobStats);
            StartCoroutine(takeDamage());
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            DamageCalc.Instance.mobToPlayer(collider.gameObject.GetComponent<PlayerStats>(), mobStats);
            Destroy(this.gameObject);
        }

    }
    //cooldown between damage ticks
    IEnumerator takeDamage()
    {
        damageCD = true;
        yield return new WaitForSeconds(.5f);
        damageCD = false;
    }

}
