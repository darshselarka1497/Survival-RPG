using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;
    private float arrowImageOffset = 135.0f;
    private string[] unPassableTags; //tags of objects that the arrow cannot pass through
    // Start is called before the first frame update\
    public GameObject createEnemyArrow(Vector3 position, Vector2 arrowDirection, float arrowSpeed)
    {
        Vector3 offset = new Vector3(arrowDirection.x, arrowDirection.y, 0);
        GameObject newArrow = Instantiate(arrowPrefab, position+offset, Quaternion.FromToRotation(Vector3.zero,offset));
        //sets arrow's velocity
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowDirection.normalized*arrowSpeed;
        newArrow.GetComponent<ShootArrow>().setType("enemy");
        //rotates the arrow image towards direction 
        newArrow.transform.Rotate(0f,0f,Mathf.Atan2(arrowDirection.y, arrowDirection.x)*Mathf.Rad2Deg+arrowImageOffset);
        return newArrow;
    }
    public GameObject createPlayerArrow(Vector3 position, Vector2 arrowDirection, float arrowSpeed)
    {
        Vector3 offset = new Vector3(arrowDirection.x, arrowDirection.y, 0);
        GameObject newArrow = Instantiate(arrowPrefab, position+offset, Quaternion.FromToRotation(Vector3.zero,offset));
        newArrow.GetComponent<ShootArrow>().setType("player");
        //sets arrow's velocity
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowDirection.normalized*arrowSpeed;
        //rotates the arrow image towards direction 
        newArrow.transform.Rotate(0f,0f,Mathf.Atan2(arrowDirection.y, arrowDirection.x)*Mathf.Rad2Deg+arrowImageOffset);
        return newArrow;
    }
    //checks if the arrow cannot pass through the tag of an object
    public bool isPassable(string tag)
    {
        unPassableTags= new string[3]{"Ore", "Tree", "Collidable"};
        for (int i = 0; i < unPassableTags.Length; i++)
        {
            if (tag == unPassableTags[i])
            {
                return false;
            }
        }
        return true;
    }
}
