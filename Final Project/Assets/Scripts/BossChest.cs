using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChest : MonoBehaviour
{
    [SerializeField]
    private int[] itemDrops; //list of item drops
    // Start is called before the first frame update
    
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && Input.GetMouseButtonDown(0))
        {
            addItemsToPlayer();
            Destroy(this.gameObject);
        }
    }

    void addItemsToPlayer()
    {
        for (int i = 0; i < itemDrops.Length; i++)
        {
            Item item = ItemDatabase.Instance.createItem(itemDrops[i]);
            PlayerItem.Instance.addItem(item);
        }

    }
}
