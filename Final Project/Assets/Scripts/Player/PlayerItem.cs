using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, Item> itemDict; //uses id to get item address if in list
    [SerializeField]
    private List<Item> itemList; //list of items
    private static PlayerItem _instance; //this instance of the script
    private int inventoryLimit = 3000; //maximum items in inventory
    [SerializeField]
    private Toolbar toolbar;
    [SerializeField]
    private Inventory inventory;
    void Awake()
    {
        _instance = this;
        itemDict = new Dictionary<int,Item>();
        itemList = new List<Item>();
    }
    void Start()
    {
        /*
        addItem(ItemDatabase.Instance.createItem(1));
        addItem(ItemDatabase.Instance.createItem(2));
        addItem(ItemDatabase.Instance.createItem(96));
        addItem(ItemDatabase.Instance.createItem(98));
        addItem(ItemDatabase.Instance.createItem(99));
        addItem(ItemDatabase.Instance.createItem(303));
        addItem(ItemDatabase.Instance.createItem(300));
        addItem(ItemDatabase.Instance.createItem(301));
        addItem(ItemDatabase.Instance.createItem(302));
        addItem(ItemDatabase.Instance.createItem(304));
        addItem(ItemDatabase.Instance.createItem(305));
        addItem(ItemDatabase.Instance.createItem(306));

        
        addItem(ItemDatabase.Instance.createItem(343));
        addItem(ItemDatabase.Instance.createItem(340));
        addItem(ItemDatabase.Instance.createItem(341));
        addItem(ItemDatabase.Instance.createItem(342));
        addItem(ItemDatabase.Instance.createItem(344));
        addItem(ItemDatabase.Instance.createItem(345));
        addItem(ItemDatabase.Instance.createItem(346));
        addItem(ItemDatabase.Instance.createItem(90));
        addItem(ItemDatabase.Instance.createItem(91));
        addItem(ItemDatabase.Instance.createItem(93));

        
        addItem(ItemDatabase.Instance.createItem(312));
        
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(500));
        addItem(ItemDatabase.Instance.createItem(501));
        addItem(ItemDatabase.Instance.createItem(501));
        addItem(ItemDatabase.Instance.createItem(501));
        */
    }
    public static PlayerItem Instance
    {
        get
        {
            return _instance;
        }
    }
    //returns a list of items
    public List<Item> ItemList
    {
        get{return itemList;}
    }
    public int InventoryLimit
    {
        get{return inventoryLimit;}
    }
    //adds the item and quantity given as a key value pair in the
    //dictionary
    public void addItem(Item item)
    {
        //checks toolbar for item if inv is full
        if (invFull(item))
        {
            toolbar.updateQuantity(item.Id, item.Quantity);
            return;
        }

        int id = item.Id;
        if (item.Stackable)
        {
            Item itemInList;
            itemDict.TryGetValue(id, out itemInList);

            if (itemInList != null)
            {
                itemInList.Quantity += item.Quantity;
                inventory.updateQuantityText(itemInList);
            }
            else
            {
                if (toolbar.itemInToolbar(id))
                {
                    toolbar.updateQuantity(id, item.Quantity);
                }
                else
                {
                    itemDict.Add(id, item);
                    itemList.Add(item);
                    inventory.addInvItem(item);
                }
            }
        }
        else
        {
            //just stores in list if unstackable
            itemList.Add(item);
            inventory.addInvItem(item);
        }
        
    }

    public int getQuantity(int id)
    {
        Item item;
        itemDict.TryGetValue(id, out item);
        if (item != null)
        {
            return item.Quantity;
        }
        else if (toolbar.itemInToolbar(id))
        {
            return toolbar.getQuantity(id);
        }
        else
        {
            return 0;
        }
    }

    public void decreaseQuantity(int id, int amount)
    {
        Item item;
        itemDict.TryGetValue(id, out item);
        if (item != null)
        {
            item.Quantity += -amount;
            
            inventory.updateQuantityText(item);
            if (item.Quantity <= 0)
            {
                removeItem(item);
            }
        }
        else
        {
            toolbar.updateQuantity(id, -amount);
        }
    }

    //checks if adding an item will make the inventory full
    //inventory size will always be <= inventorylimit
    public bool invFull(Item item)
    {
        if (itemList.Count < inventoryLimit)
        {
            return false;
        }
        else
        {
            int id = item.Id;
            if (item.Stackable)
            {
                Item itemInDict;
                itemDict.TryGetValue(id, out itemInDict);

                if (itemInDict != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }

    //adds an item and removes a list of items if
    //adding an item won't exceed the size of the 
    //inventory
    public void addAndRemove(Item item, List<int> removeId, List<int> removeQuantity)
    {
        if (invFull(item))
        {
            return;
        }
        addItem(item);
        for (int i = 0; i < removeId.Count; i++)
        {
            decreaseQuantity(removeId[i], removeQuantity[i]);
        }
    }

    public void removeItem(Item item)
    {
        itemDict.Remove(item.Id);
        itemList.Remove(item);
        inventory.removeItem(item);
    }

    public void loadItems(List<Item> _items)
    {
        itemList = new List<Item>();
        itemDict = new Dictionary<int, Item>();
        for (int i = 0; i < _items.Count; i++)
        {
            addItem(_items[i]);
        }
    }
}
