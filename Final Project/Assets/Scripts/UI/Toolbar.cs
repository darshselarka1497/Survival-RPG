using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Toolbar : MonoBehaviour
{
    [SerializeField]
    private GameObject slotSelected;
    [SerializeField]
    private GameObject toolBarPrefab;
    [SerializeField]
    private int toolBarSize = 10;
    [SerializeField]
    private PlayerItem playerItem;
    [SerializeField]
    private Profile profile;
    private GameObject[] toolbarSlots;
    private Item[] itemList;
    private GameObject[] cdBar;
    private Dictionary<int,Item> itemDict;
    private int toolbarIndex = 0;
    private static Toolbar _instance;
    private Weapon equippedWeapon;
    void Awake()
    {
        _instance = this;
        toolbarSlots = new GameObject[toolBarSize];
        cdBar = new GameObject[toolBarSize];
        itemList = new Item[toolBarSize];
        itemDict = new Dictionary<int, Item>();
        initializeToolbar();
        updateToolbarSlot();
        //toolbarSlots = GameObject.FindGameObjectsWithTag("ItemSlot");
    }
    public GameObject[] ToolBarSlots
    {
        get{return toolbarSlots;}
    }
    public Item[] ItemList
    {
        get{return itemList;}
    }
    public int ToolbarIndex
    {
        get{return toolbarIndex;}
    }
    public Weapon EquippedWeapon
    {
        get{return equippedWeapon;}
    }
    public static Toolbar Instance
    {
        get
        {
            return _instance;
        }
    }

    //initializes all the toolbar slots
    void initializeToolbar()
    {
        for (int i = 0; i < toolBarSize; i++)
        {
            GameObject toolBar = Instantiate(toolBarPrefab, this.transform);
            toolBar.SetActive(true);
            toolbarSlots[i] = toolBar.transform.GetChild(0).gameObject;
            cdBar[i] = toolBar.transform.GetChild(1).gameObject;
        }
    }
    //updates the toolbar position to where the current index is at
    //(should change how the position is handled in the future)
    void updateToolbarSlot()
    {
        Vector3 pos = Vector3.zero;
        pos.x = 30 + 55*toolbarIndex -222.5f;
        slotSelected.GetComponent<RectTransform>().localPosition = pos;
    }
    //moves the toolbar index by amount
    //amount should only be +- 1, but overflow
    //is handled
    public void updateToolbarIndex(int amount)
    {
        toolbarIndex += amount;
        if (toolbarIndex > toolBarSize-1)
        {
            toolbarIndex = toolbarIndex % toolBarSize;
        }
        else if (toolbarIndex < 0)
        {
            toolbarIndex = (toolBarSize + toolbarIndex) % toolBarSize;
        }
        updateToolbarSlot();
        updateWeaponStats();
    }
    //Adds or removes stats to the player and updates 
    //profile ui
    public void updateWeaponStats()
    {
        if(equippedWeapon != null)
        {
            profile.unequipWeapon(equippedWeapon);
            equippedWeapon = null;
        }
        Item itemSelected = itemList[toolbarIndex];
        if (itemSelected != null && (itemSelected.Type == "Sword" || itemSelected.Type == "Bow"))
        {
            profile.equipWeapon((Weapon)itemSelected);
            equippedWeapon = (Weapon)itemSelected;
        }

    }
    public Item getActiveItem()
    {
        return itemList[toolbarIndex];
    }
    public void putInToolbar(Item item, int index)
    {
        itemList[index] = item;
        if (item.Stackable)
        {
            itemDict.Add(item.Id, item);
        }
        toolbarSlots[index].GetComponent<Mask>().enabled = false;
        toolbarSlots[index].GetComponent<Image>().sprite = Resources.Load<Sprite>(item.SpritePath);
        updateQuantityText(findItemIndex(item));
    }
    //Increases the quantity of an item
    public void updateQuantity(int id, int amount)
    {
        Item item;
        itemDict.TryGetValue(id, out item);
        if (item.Stackable)
        {
            item.Quantity += amount;
            updateQuantityText(findItemIndex(item));
            if (item.Quantity <= 0)
            {
                removeItem(item);
            }
        }
    }

    //checks if the item id is in the toolbar
    public bool itemInToolbar(int id)
    {
        Item item;
        itemDict.TryGetValue(id, out item);
        return item != null;
    }
    //updates the text quantity ui for stackable items
    public void updateQuantityText(int index)
    {
        if (index < 0 || index >= itemList.Length)
        {
            return;
        }
        Item item = itemList[index];
        if (item != null && item.Stackable)
        {
            toolbarSlots[index].GetComponentInChildren<Text>().text = "x" + item.Quantity;
        }
        else
        {
            toolbarSlots[index].GetComponentInChildren<Text>().text = "";
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
        return 0;
    }
    //removes item and turn off image and text for the toolbar slot
    public void removeItem(Item item)
    {
        itemDict.Remove(item.Id);
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] == item)
            {
                itemList[i] = null;
                toolbarSlots[i].GetComponent<Mask>().enabled = true;
                toolbarSlots[i].GetComponentInChildren<Text>().text = "";
            }
        }
    }

    //checks what the dragged item is hovering over on ending drag
    public void endDrag(GameObject dragObject)
    {
        if (dragObject.GetComponent<Mask>().enabled)
        {
            GetComponent<ItemDrag>().returnToOriginal();
            return;
        }
        int slotIndex = findObjectIndex(dragObject);
        //finds the list of gameobject that the mouse is hovering over
        PointerEventData pData = new PointerEventData(EventSystem.current);
        pData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, results);
        Item draggedItem = itemList[slotIndex];
        for (int i = 0; i < results.Count; i++)
        {
            //moves the item to the inventory if the mouse is hovering over the inventory
            if (results[i].gameObject == GameObject.FindGameObjectWithTag("Inventory"))
            {
                removeItem(draggedItem);
                playerItem.addItem(draggedItem);
            }
            //Swaps positions within the toolbar if this condition is satisfied
            if(results[i].gameObject.tag == "ItemSlot" && findObjectIndex(results[i].gameObject) != slotIndex)
            {
                int newIndex = findObjectIndex(results[i].gameObject);
                //GetComponent<ItemDrag>().returnToOriginal();
                
                //swaps items in the indexes of item list
                Item temp = itemList[newIndex];
                itemList[newIndex] = itemList[slotIndex];
                itemList[slotIndex] = temp;
                if (itemList[newIndex] != null)
                {
                    //draggedObject.GetComponent<ItemDrag>().destroyObject();
                    toolbarSlots[newIndex].GetComponent<Mask>().enabled = false;
                    toolbarSlots[newIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>(itemList[newIndex].SpritePath);
                }
                //swaps the items in the slots if the targeted slot has an item
                if (itemList[slotIndex] != null)
                {
                    toolbarSlots[slotIndex].GetComponent<Image>().sprite = Resources.Load<Sprite>(itemList[slotIndex].SpritePath);
                    //update Text ui for toolbar if the item is stackable
                }
                else
                {
                    //hides the image in the slot
                    toolbarSlots[slotIndex].GetComponent<Mask>().enabled = true;
                }
                updateQuantityText(newIndex);
                updateQuantityText(slotIndex);
            }
            //Drag item from toolbar to equipment
            if (results[i].gameObject.name == draggedItem.Type && draggedItem.Type != "Item")
            {
                profile.equip((Armor) draggedItem, results[i].gameObject);
                removeItem(draggedItem);
            }
            
        }
        GetComponent<ItemDrag>().returnToOriginal();
        updateWeaponStats();
    }
    public int findObjectIndex(GameObject slotObject)
    {
        for (int i = 0; i < toolbarSlots.Length; i++)
        {
            if (toolbarSlots[i] == slotObject)
            {
                return i;
            }
        }
        return -1;
    }
    public int findItemIndex(Item item)
    {
        for (int i = 0; i < toolbarSlots.Length; i++)
        {
            if (itemList[i] == item)
            {
                return i;
            }
        }
        return -1;
    }

    //rotates the cooldown bar based on how much time has passed
    public IEnumerator setCooldown(float cd, string type)
    {
        float totalTime = 0;
        while(totalTime < cd)
        {
            totalTime += Time.deltaTime;
            updateCdBar(type, (1/cd)*(cd-totalTime), true);
            yield return null;
        }
        updateCdBar(type, 1, false);
    }
    //updates all the cd for items with the same type
    public void updateCdBar(string type, float amount, bool enabled)
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] != null && itemList[i].Type == type)
            {
                cdBar[i].transform.SetAsLastSibling();
                cdBar[i].SetActive(enabled);
                cdBar[i].GetComponent<Image>().fillAmount = amount;
            }
        }
    }

    public void loadToolbar(Item[] toolbarItems, int _toolbarIndex, Weapon _equippedWeapon)
    {
        toolbarIndex = _toolbarIndex;   
        equippedWeapon = _equippedWeapon;
        for (int i = 0; i < toolbarItems.Length; i++)
        { 
            Item item = toolbarItems[i];
            if(item != null)
            {
                putInToolbar(item, i);
            }
        }
        updateToolbarSlot();
    }

}
