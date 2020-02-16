using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject profile;
    [SerializeField]
    private GameObject menu;    
    [SerializeField]
    private GameObject inventory;
    [SerializeField]
    private GameObject craftingMenu;
    [SerializeField]
    private GameObject smeltingMenu;
    [SerializeField]
    private GameObject itemDisplay;
    private LinkedList<GameObject> openedUI;
    // Start is called before the first frame update
    void Start()
    {
        openedUI = new LinkedList<GameObject>();
        string json = Resources.Load<TextAsset>("Json/SmeltingRecipes").text;
        List<Recipe> smeltingRecipes = JsonConvert.DeserializeObject<List<Recipe>>(json);
        smeltingMenu.GetComponentInChildren<ButtonControl>().setRecipes(smeltingRecipes);

        json = Resources.Load<TextAsset>("Json/CraftingRecipes").text;
        List<Recipe> craftingRecipes = JsonConvert.DeserializeObject<List<Recipe>>(json);
        craftingMenu.GetComponentInChildren<ButtonControl>().setRecipes(craftingRecipes);

        smeltingMenu.SetActive(false);
        craftingMenu.SetActive(false);
        profile.SetActive(false);
        inventory.SetActive(false);
        itemDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !menu.activeSelf && openedUI.Count == 0)
        {
            menu.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menu.activeSelf)
        {
            menu.SetActive(false);
            Time.timeScale = 1;
        }
        if (!menu.activeSelf)
        {
            //creates an inventory game object and displays it
            //when the I key is pressed
            if (Input.GetKeyDown(KeyCode.P) && !openedUI.Contains(profile))
            {
                profile.SetActive(true);
                openedUI.AddLast(profile);
            } 
            //destroys the profileentory gameobject when I or esc is pressed
            else if (Input.GetKeyDown(KeyCode.P) && openedUI.Contains(profile))
            {
                profile.SetActive(false);
                openedUI.Remove(profile);
            }
            //creates an inventory game object and displays it
            //when the I key is pressed
            if (Input.GetKeyDown(KeyCode.I) && !openedUI.Contains(inventory))
            {
                inventory.SetActive(true);
                openedUI.AddLast(inventory);
            } 
            //destroys the inventory gameobject when I or esc is pressed
            else if (Input.GetKeyDown(KeyCode.I) && openedUI.Contains(inventory))
            {
                inventory.SetActive(false);
                itemDisplay.SetActive(false);
                openedUI.Remove(inventory);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && openedUI.Count > 0)
            {
                if (openedUI.First.Value == inventory)
                {
                    itemDisplay.SetActive(false);
                }
                if(openedUI.First.Value == smeltingMenu || openedUI.First.Value == craftingMenu)
                {
                    Time.timeScale = 1;
                }
                openedUI.First.Value.SetActive(false);
                openedUI.RemoveFirst();
            }
        }
    }
    public void openSmeltingMenu()
    {
        if (!openedUI.Contains(smeltingMenu))
        {
            smeltingMenu.SetActive(true);
            openedUI.AddLast(smeltingMenu);
            Time.timeScale = 0;
        }
    }
    public void openCraftingMenu()
    {
        if (!openedUI.Contains(craftingMenu))
        {
            craftingMenu.SetActive(true);
            openedUI.AddLast(craftingMenu);
            Time.timeScale = 0;
        }
    }
    public void exitGame()
    {
        Application.Quit();
    }
    public void returnToGame()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
    }
}
