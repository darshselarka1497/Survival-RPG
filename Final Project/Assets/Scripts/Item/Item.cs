using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField]
    protected int id;
    [SerializeField]
    protected string name;
    [SerializeField]
    protected string type;
    [SerializeField]
    protected bool stackable;
    [SerializeField]
    protected int quantity;
    [SerializeField]
    protected string description;
    protected string spritePath;

    public Item(int id, string name, string type, bool stackable, int quantity, string description)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.stackable = stackable;
        this.quantity = quantity;
        this.description = description;
        this.spritePath = "Items/item_" + id.ToString();
    }
    public int Id
    {
        get{return id;}
    }
    public string Name
    {
        get{return name;}
    }
    public string Type
    {
        get{return type;}
    }
    public bool Stackable
    {
        get{return stackable;}
    }
    public int Quantity
    {
        get{return quantity;}
        set{quantity = value;}
    }
    public string Description
    {
        get{return description;}
    }
    public string SpritePath
    {
        get{return spritePath;}
    }

    public virtual Item newItem()
    {
        return new Item(id,name,type,stackable,quantity,description);
    }
    //displays the stat of the item as a user iterface
    public virtual string display()
    {
        string itemInfo = 
        "Name: " + name +
        "\nType: " + type +
        "\nQuantity: " + quantity +
        "\nDescription: " + description;
        return itemInfo;
    }
}
