using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject portal;
    [SerializeField]
    private GameObject sceneTransition;
    [SerializeField]
    private float movementSpeed = 2.0f;
    private Vector3 center;
    private Vector3[] targetPool;
    private Vector3 targetPos;
    private bool isCharging = true;
    private bool reachedCenter = false;
    private Vector3 direction;
    [SerializeField]
    private RectTransform healthBar;
    private MobStats bossStat;
    private float minWaitTime = 3.0f;
    private float maxWaitTime = 6.0f;
    private bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3(2.5f,-1.5f,0f);
        targetPool = new Vector3[4];
        targetPool[0] = center + new Vector3(0,15,0);
        targetPool[1] = center + new Vector3(0,-15,0);
        targetPool[2] = center + new Vector3(15,0,0);
        targetPool[3] = center + new Vector3(-15,0,0);
        int randomIndex = Random.Range(0,4);
        targetPos = targetPool[randomIndex];
        animator.speed = animator.speed*movementSpeed;
        bossStat = GetComponent<MobStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            charge();
        }
        //updates phase state
        phaseUpdate();
        //updates animations
        setAnimations();
        //healthbar update
        healthBar.sizeDelta = new Vector2(300.0f/bossStat.MaxHealth*bossStat.Health,healthBar.sizeDelta.y);

        //checks death
        bossDeath();
    }

    void phaseUpdate()
    {
        float healthPercent = bossStat.Health/bossStat.MaxHealth;
        if (healthPercent > .75)
        {
            minWaitTime = 2.0f;
            maxWaitTime = 5.0f;
        }
        else if (healthPercent > .5)
        {
            minWaitTime = 2.0f;
            maxWaitTime = 4.0f;
        }
        else if (healthPercent > .25)
        {
            minWaitTime = 1.0f;
            maxWaitTime = 2.0f;
        }
        else 
        {
            minWaitTime = 0f;
            maxWaitTime = 0f;
        }

    }

    void setAnimations()
    {
        //animations
        animator.SetFloat("FacingX", direction.x);
        animator.SetFloat("FacingY", direction.y);
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Magnitude", direction.magnitude);
    }
    void bossDeath()
    {
        if (bossStat.Health <= 0)
        {
            StopAllCoroutines();
            MobUI.Instance.removeMob(bossStat);
            Destroy(healthBar.parent.gameObject);
            sceneTransition.SetActive(true);
            portal.SetActive(true);
            Destroy(this.gameObject);
        }

    }

    IEnumerator startCharge()
    {
        float randomWaitTime = Random.Range(minWaitTime,maxWaitTime);
        yield return new WaitForSeconds(randomWaitTime);
        isCharging = true;
        int randomIndex = Random.Range(0,4);
        targetPos = targetPool[randomIndex];
    }
    void charge()
    {
        if (!reachedCenter)
        {
            transform.position = Vector3.MoveTowards(transform.position, center, .1f*movementSpeed);
            direction = center - transform.position;
            if (transform.position == center)
            {
                reachedCenter = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, .1f*movementSpeed);
            direction = targetPos-transform.position;
            if (transform.position == targetPos)
            {
                direction = center - transform.position;
                reachedCenter = false;
                isCharging = false;
                StartCoroutine(startCharge());
            } 
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && !isAttacking)
        {
            DamageCalc.Instance.mobToPlayer(collision.gameObject.GetComponent<PlayerStats>(), GetComponent<MobStats>());
            StartCoroutine(damageCD());
        }
    }

    IEnumerator damageCD()
    {
        isAttacking = true;
        yield return new WaitForSeconds(.3f);
        isAttacking = false;
    }
}
