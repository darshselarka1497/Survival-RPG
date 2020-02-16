using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    void OnCollisionStay2D(Collision2D collision)
    {
        GameObject coll = collision.gameObject;
        Item activeItem =Toolbar.Instance.getActiveItem(); 
        if(coll.tag == "Player" && activeItem != null && activeItem.Id == 99)
        {
            StartCoroutine(usingBoat(collision));
        }
    }
    IEnumerator usingBoat(Collision2D collision)
    {
        Physics2D.IgnoreCollision(collision.collider, collision.otherCollider,true);
        Player.Instance.OnBoat();
        yield return new WaitForSeconds(.1f);
        while(onWater())
        {
            Player.Instance.OnBoat();
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider,true);
            yield return null;
        }
        Player.Instance.OffBoat();
        Physics2D.IgnoreCollision(collision.collider, collision.otherCollider,false);
    }
    bool onWater()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(Player.Instance.transform.position, Vector2.zero);
        for (int i = 0; i< hit.Length; i++)
        {
            if (hit[i].collider == GetComponent<Collider2D>())
            {
                return true;
            }
        }
        return false;
    }
}
