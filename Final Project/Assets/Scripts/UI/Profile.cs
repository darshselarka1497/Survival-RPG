using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Profile : MonoBehaviour
{
    [SerializeField]
    private PlayerStats player;
    [SerializeField]
    private PlayerItem playerItem;
    [SerializeField]
    private Toolbar toolbar;
    private Sprite[] defaultSprites;
    private Armor[] equipments;
    [SerializeField]
    private GameObject[] equipSlots;
    private static Profile _instance;
    [SerializeField]
    private Text statName;
    [SerializeField]
    private Text statAmount;
    void Awake()
    {
        //_instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        setStats();    
        equipments = new Armor[equipSlots.Length];  
        defaultSprites = new Sprite[equipSlots.Length];
        for (int i = 0;  i < defaultSprites.Length; i++)
        {
            defaultSprites[i] = equipSlots[i].GetComponent<Image>().sprite;
        }
    }
    public static Profile Instance
    {
        get
        {
            return _instance;
        }
    }
    public Armor[] Equipment
    {
        get{return equipments;}
    }

    public Armor getArmorAtSlot(GameObject slot)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (slot == equipSlots[i])
            {
                return equipments[i];

            }
        }
        return null;
    }

    //puts armor in list of armor where the slot is and adds stats to player
    public void equip(Armor armor, GameObject slot)
    {
        int index = findSlotIndex(slot);
        if (equipments[index] != null)
        {
            unequip(armor);
        }
        player.addArmorStats(armor);
        equipments[index] = armor;
        imageDisplay(armor, slot);
        playerItem.removeItem(armor);
        setRingBonus();
        setStats();
    }
    void setRingBonus()
    {
        Modifiers modifiers = Modifiers.Instance;
        if(equipments[5] == null)
        {
            modifiers.ExpRate = 1;
            modifiers.DropRate = 1;
            return;

        }
        switch(equipments[5].Id)
        {
            case 90: 
                modifiers.ExpRate = 2;
                break;
            case 91:
                modifiers.DropRate = 2;
                break;
        }

    }
    public void equipWeapon(Weapon weapon)
    {
        player.addWeaponStats(weapon);
        setStats();
    }
    public void unequipWeapon(Weapon weapon)
    {
        player.removeWeaponStats(weapon);
        setStats();
    }
    public void unequip(Armor armor)
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            if (armor == equipments[i])
            {
                equipments[i] = null;
                player.removeArmorStats(armor);
                imageHide(i);
            }
        }
        setStats();
        setRingBonus();
    }
    public void unequipSlot(GameObject slot)
    {

    }
    void imageDisplay(Armor armor, GameObject slot)
    {
        //slot.GetComponent<Mask>().enabled = false;
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>(armor.SpritePath);

    }
    void imageHide(int index)
    {
        equipSlots[index].GetComponent<Image>().sprite = defaultSprites[index];
        //slot.GetComponent<Mask>().enabled = true;
    }
    int findSlotIndex(GameObject obj)
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (obj == equipSlots[i])
                return i;
        }
        return -1;
    }

    public void endDrag(GameObject draggedObject)
    {
        int index = findSlotIndex(draggedObject);
        if (draggedObject.GetComponent<Image>().sprite == defaultSprites[index])
        {   
            draggedObject.GetComponent<ItemDrag>().returnToOriginal();
            return;
        }
        //finds the list of gameobject that the mouse is hovering over
        PointerEventData pData = new PointerEventData(EventSystem.current);
        pData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, results);
        for (int i = 0; i < results.Count; i++)
        {
            //moves the item to the inventory if the mouse is hovering over the inventory
            if (results[i].gameObject == GameObject.FindGameObjectWithTag("Inventory"))
            {
                Armor armor = equipments[index];
                unequip(armor);
                playerItem.addItem(armor);
            }            
            if(results[i].gameObject.tag == "ItemSlot")
            {
                //toolbar index and slots
                int slotIndex = toolbar.findObjectIndex(results[i].gameObject);
                GameObject[] slots = toolbar.ToolBarSlots;

                //checks if toolbar slot already contains an item
                //returns to original position if it does
                if (toolbar.ItemList[slotIndex] != null)
                {
                    draggedObject.GetComponent<ItemDrag>().returnToOriginal();
                    return;
                }
                //removes equipment
                Armor armor = equipments[index];
                unequip(armor);

                //puts the item into the toolbar item list
                toolbar.putInToolbar(armor,slotIndex);
            }
        }
        draggedObject.GetComponent<ItemDrag>().returnToOriginal();
    }
    public void setStats()
    {
        string[] sNames = player.getStatName();
        float[] sAmount = player.getStatAmount();
        string nameText = "";
        string amountText = "";
        for (int i = 0; i < sNames.Length; i++)
        {
            if (sNames[i] == "MinWeaponAttack")
            {
                nameText += "Weapon Attack\n";
                amountText += sAmount[i] + "~";
            }
            else if(sNames[i] == "MaxWeaponAttack")
            {
                amountText += sAmount[i] + "\n";
            }
            else if (sNames[i] == "Crit Chance" || sNames[i] == "Crit Damage")
            {
                nameText += sNames[i] + "\n";
                amountText += sAmount[i] + "%\n";
            }
            else
            {
                nameText += sNames[i] + "\n";
                amountText += sAmount[i] + "\n";
            }
        }
        statName.text = nameText;
        statAmount.text = amountText;
    }

    public void loadEquips(Armor[] equips)
    {
        for (int i = 0; i < equips.Length; i++)
        {
            if(equips[i] != null)
            {
                equipments[i] = equips[i];
                imageDisplay(equips[i], equipSlots[i]);
            }
        }
        setRingBonus();
        setStats();
    }
}
