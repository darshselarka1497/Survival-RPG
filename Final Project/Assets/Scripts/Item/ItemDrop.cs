using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    private Item item;
    private float up = 0;
    private float multiplier = 1;
    [SerializeField]
    private int id = 0;
    [SerializeField]
    private bool destroy = true;
    [SerializeField]
    private float destroyTimer = 30.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (id != 0)
        {
            item = ItemDatabase.Instance.createItem(id);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (up > 1)
        {
            multiplier = -1;
        }
        if (up < 0)
        {
            multiplier = 1;
        }
            up += multiplier * Time.deltaTime;
            transform.position += new Vector3(0,.1f,0) * multiplier * Time.deltaTime;
            transform.localScale += new Vector3(.2f ,.2f,0) * multiplier * Time.deltaTime;
    }
    public void drop(Item item)
    {
        this.item = item;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(item.SpritePath);
        if(destroy)
        {
            StartCoroutine(destroyItem());
        }
    }
    IEnumerator destroyItem()
    {
        yield return new WaitForSeconds(destroyTimer);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            PlayerItem.Instance.addItem(item);
            Destroy(this.gameObject);
        }
    }
}
