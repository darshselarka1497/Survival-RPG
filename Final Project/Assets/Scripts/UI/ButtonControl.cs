using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate; //template for creating button
    private List<Recipe> recipes; //list of recipes
    [SerializeField]
    private Text materialText; //materials text
    [SerializeField]
    private Text quantityText; //quantity for material
    private int currentItem; //index of recipe list
    private int craftQuantity = 1; //quantity of item being crafted

    //sets the list of recipes when the menu opens
    public void setRecipes(List<Recipe> recipeList)
    {
        recipes = recipeList;
        //creates button names from each recipe 
        for (int i = 0; i < recipes.Count; i++)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            ButtonList bList = button.GetComponent<ButtonList>();
            bList.SetButton(recipes[i].name, i);
            
            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }

    //takes the button's id(index of recipe in recipe list)
    //and goes through all the materials needed for the recipe
    //and displays it as text ingame
    public void buttonClick(int id)
    {
        currentItem = id;   
        string matName = "";
        string quantity = "";
        for (int i = 0; i < recipes[id].materials.Count; i++)
        {
            matName += recipes[id].materials[i].materialName + "\n";
            quantity += recipes[id].materials[i].materialQuantity + "\n";
        }
        materialText.text = matName;
        quantityText.text = quantity;
    }

    //checks if all the material requirements are met
    //and then crafts the item if the conditions
    //are satisfied
    public void craftItem()
    {
        PlayerItem pItem = PlayerItem.Instance;
        Recipe recipe = recipes[currentItem];
        List<int> removeId = new List<int>();
        List<int> removeQuantity = new List<int>();

        //checks if player has the materials
        for (int i = 0; i < recipe.materials.Count; i++)
        {
            int matId = recipe.materials[i].materialId;
            int quantityOwned = pItem.getQuantity(matId);
            removeId.Add(matId);
            removeQuantity.Add(recipe.materials[i].materialQuantity);
            if (quantityOwned < recipe.materials[i].materialQuantity)
            {
                return;
            }
        }

        //add crafted material to player inventory
    
        Item newItem = ItemDatabase.Instance.createItem(recipe.id);
        PlayerItem.Instance.addAndRemove(newItem, removeId, removeQuantity);
    }
}
