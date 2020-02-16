using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gathering : MonoBehaviour
{
    private const float respawnTime = 30.0f; //how long for resource to respawn
    private const int fatigueCost = 3;
    public GameObject resource;
    public int id;//item id that the resource gives
    public string name;//name of resource
    private bool hasGathered = false; //checks if this resource has been gathered
    [SerializeField]
    private GameObject itemDrop;

    //sets hasGathered to true and starts the coroutine
    public void gather()
    {
        hasGathered = true;
        StartCoroutine(respawn());
        dropItem();
    }
    public int FatigueCost
    {
        get{return fatigueCost;}
    }
    //starts the respawn timer for the resource
    //hides sprite and boxcolliders
    IEnumerator respawn()
    {
        SpriteRenderer sprite = resource.GetComponent<SpriteRenderer>();
        BoxCollider2D boxColl = resource.GetComponent<BoxCollider2D>();
        sprite.enabled = false;
        boxColl.enabled = false;    
        yield return new WaitForSeconds(respawnTime);
        sprite.enabled = true;
        boxColl.enabled = true;
    }
    void dropItem()
    {
        GameObject itemDropped = Instantiate(itemDrop, transform.position, Quaternion.identity);
        itemDropped.GetComponent<ItemDrop>().drop(getResource());
    }
    //returns an item of this gathered resource
    public Item getResource()
    {
        return ItemDatabase.Instance.createItem(id);
    }
}
