using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfFire : MonoBehaviour
{
    [SerializeField]
    private GameObject fire;
    private float damagePercent = .1f; //deals 10% damage when hit

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0,15);
        int rand2 = Random.Range(18,33);
        for (int i = 0; i < 35; i++)
        {
            if (i != rand && i != rand +1 && i != rand+2 && i != rand2 && i != rand2+1 && i != rand2+2)
            {
                Instantiate(fire, new Vector3(i*.6f, 0,0), Quaternion.identity,this.transform);
                Instantiate(fire, new Vector3(11, i*.6f-10,0), Quaternion.identity,this.transform);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.forward * 10*Time.time);
    }    
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        StartCoroutine(tempDestroyFire(collision.otherCollider.gameObject));
        int damage = -(int)Mathf.Ceil(damagePercent * Player.Instance.MaxHealth);
        Player.Instance.updateHealth(damage);
        
    }
    IEnumerator tempDestroyFire(GameObject fire)
    {
        fire.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        fire.SetActive(true);
    }
}
