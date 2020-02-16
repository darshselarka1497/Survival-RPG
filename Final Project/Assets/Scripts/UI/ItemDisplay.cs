using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ItemDisplay : MonoBehaviour
{
    private Item item; //item to display
    [SerializeField]
    private GameObject itemDisplayInfo; //prefab for displaying item information
    private Text itemDisplayText; //text of item
    private bool onDisplay = false;
    public void setItem(Item item)
    {
        this.item = item;
    }
    public Item getItem()
    {
        return item;
    }
    public void displayInfo()
    {
        //displays item information when the mouse hovers over the item
        if (!onDisplay && !GetComponent<ItemDrag>().IsDragging)
        {
            //sets the item display to the mouse cursor's position
            Vector3[] corners = new Vector3[4];
            GetComponent<RectTransform>().GetWorldCorners(corners);
            itemDisplayInfo.GetComponent<RectTransform>().position = corners[3];

            //sets the display text information
            itemDisplayText = itemDisplayInfo.GetComponentInChildren<Text>();
            itemDisplayText.text = item.display();

            //sets the display gameobject active and also stores a boolean
            //to keep track of whether its active or not
            onDisplay = true;
            itemDisplayInfo.SetActive(true);
        }
    }

    public void hideInfo()
    {
        if(onDisplay)
        {
            onDisplay = false;
            itemDisplayInfo.SetActive(onDisplay);
        }
    }
    public void endDrag()
    {
        Inventory controller = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        controller.endDrag(this.gameObject);
    }
}
