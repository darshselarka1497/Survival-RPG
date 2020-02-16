using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject itemTemplate; //template for creating item
    [SerializeField]
    private PlayerItem playerItem;
    [SerializeField]
    private Profile profile;
    [SerializeField]
    private Toolbar toolbar;
    private List<Item> items; //list of items
    private Dictionary<Item, GameObject> itemToUI;
    void Awake()
    {   
        itemToUI = new Dictionary<Item, GameObject>();
        items = new List<Item>();
    }
    public void addInvItem(Item _item)
    { 
        items.Add(_item);
        GameObject itemUI = Instantiate(itemTemplate) as GameObject;
        itemToUI.Add(_item, itemUI);
        itemUI.SetActive(true);

        //Sets the text for the quantity of the item
        Text itemText = itemUI.GetComponentInChildren<Text>();
        if (_item.Stackable)
        {
            itemText.text = "x" + _item.Quantity;
        }
        //Sets what item information to display when hovered over
        itemUI.GetComponentInChildren<ItemDisplay>().setItem(_item);
        Image img = itemUI.GetComponentInChildren<Image>();
        img.sprite = Resources.Load<Sprite>(_item.SpritePath);
        img.preserveAspect = true;
        
        itemUI.transform.SetParent(itemTemplate.transform.parent, false);
    }
    //(endDrag is called when the mouse stops dragging an item)
    //The gameobject containing the item is passed in and
    //the function checks if the mouse if over a toolbar slot.
    //If yes, it'll move the item from the player's inventory
    //into the toolbar.
    public void endDrag(GameObject draggedObject)
    {
        draggedObject.GetComponent<ItemDisplay>().hideInfo();
        //finds the list of gameobject that the mouse is hovering over
        PointerEventData pData = new PointerEventData(EventSystem.current);
        pData.position = Input.mousePosition;
        GameObject[] slots = toolbar.ToolBarSlots;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, results);

        for (int i = 0; i < results.Count; i++)
        {
            Item draggedItem = draggedObject.GetComponent<ItemDisplay>().getItem();
            //the slot gameobject is equal to the gameobject the mouse is hovering over
            if(results[i].gameObject.tag == "ItemSlot")
            {
                int slotIndex = toolbar.findObjectIndex(results[i].gameObject);
                Item toolbarItem = toolbar.ItemList[slotIndex];
                //checks if toolbar slot already contains an item
                if (toolbarItem != null)
                {
                    toolbar.removeItem(toolbarItem);
                    playerItem.addItem(toolbarItem);
                }
                draggedObject.GetComponent<ItemDrag>().destroyObject();

                //puts the item into the toolbar item list and remove it from player inventory
                toolbar.putInToolbar(draggedItem,slotIndex);
                playerItem.removeItem(draggedItem);
                toolbar.updateWeaponStats();
                return;
                
            }
            else if (results[i].gameObject.name == draggedItem.Type && draggedItem.Type != "Item")
            {
                playerItem.removeItem(draggedItem);
                Armor equippedItem = profile.getArmorAtSlot(results[i].gameObject);
                if (equippedItem != null)
                {
                    playerItem.addItem(equippedItem);
                    profile.unequip((Armor)equippedItem);
                }
                profile.equip((Armor) draggedItem, results[i].gameObject);
                draggedObject.GetComponent<ItemDrag>().destroyObject();
                return;
            }

        }
        //returns the item to its original position in the inventory
        ItemDrag drag = draggedObject.GetComponent<ItemDrag>();
        drag.IsDragging = false;
        drag.returnToOriginal();
    }
    public void updateQuantityText(Item item)
    {
        GameObject itemUI;
        if(itemToUI.TryGetValue(item, out itemUI))
        {
            Text itemText = itemUI.GetComponentInChildren<Text>();
            itemText.text = "x" + item.Quantity;
        }
        print(itemUI);
        
    }

    public void removeItem(Item item)
    {
        GameObject itemUI;
        if(itemToUI.TryGetValue(item, out itemUI))
        {
            Destroy(itemUI);
        }

        items.Remove(item);
        itemToUI.Remove(item);

    }
}
