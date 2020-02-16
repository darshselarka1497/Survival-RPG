using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfFire : MonoBehaviour
{
    [SerializeField]
    private GameObject fire;
    private float damagePercent = .1f; //deals 10% damage when hit

    // Start is called before the first frame update
    void Start()
    {
        float rand = Mathf.Round(Random.Range(1,33));

        for (int i = 0; i < 35; i++)
        {
            if (rand != i && rand != i+1 && rand != i+2)
            {
                Instantiate(fire, new Vector3(i*.6f+1, 10,0), Quaternion.identity,this.transform);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0,1,0) * Time.deltaTime;
        if (transform.position.y < -20)
        {
            Destroy(this.gameObject);
        }
    }    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.otherCollider.gameObject);
        int damage = -(int)Mathf.Ceil(damagePercent * Player.Instance.MaxHealth);
        Player.Instance.updateHealth(damage);
    }
}
