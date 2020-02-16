using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Vector3 direction;
    private float attackSpeed = 5.0f;
    private bool phase2 = false;
    private MobStats mobStats;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15)
        {
            Destroy(this.gameObject);
        }
        transform.position += direction * attackSpeed * Time.deltaTime;
        if (phase2)
        {
            Vector3 scaleSize = new Vector3(Time.deltaTime,Time.deltaTime,0);
            transform.localScale += scaleSize;
        }
    }
    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collideObject = collision.gameObject;
        if (collideObject.tag == "Player")
        {
            collideObject.GetComponent<Player>().updateHealth(-10);
        }
        if (collideObject.tag != "Water")
        {
            Destroy(this.gameObject);
        }
    }
    */
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            DamageCalc.Instance.mobToPlayer(collider.GetComponent<PlayerStats>(), mobStats);
            Destroy(this.gameObject);
        }
    }
    public void setTargetPosition(Vector3 target)
    {
        direction = (target - transform.position).normalized;   
    }
    public void setFireBallPhase2()
    {
        phase2 = true;
    }
    public void setMobStats(MobStats value)
    {
        mobStats = value;
    }
}
