using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

[System.Serializable]
public class Player : MonoBehaviour
{
    
    private static Player _instance; //returns this script instance
    private Rigidbody2D Rigidbody; //rigidbody component
    private Vector2 move; //movement velocity
    [SerializeField]
    private float speed = 5.0f; //movement speed
    private const float arrowSpeed = 5.0f; //speed of arrow
    private const float arrowImageOffset = 135.0f; //rotation offset of arrow 

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject uimanager;
    [SerializeField]
    private ArrowFactory arrow;
    [SerializeField]
    private SceneLoad sceneLoader;

    //Survival variables
    [SerializeField]
    private int hunger = 100; //hunger counter(100=full)
    [SerializeField]
    private int thirst = 100; //thirst counter(100=full)
    [SerializeField]
    private int fatigue = 0; //fatigue counter(100=fatigued)
    [SerializeField]
    private int health = 200; //health bar(0=death)
    [SerializeField]
    private int maxHunger = 100;
    [SerializeField]
    private int maxThirst = 100;
    [SerializeField]
    private int maxFatigue = 100;
    [SerializeField]
    private int maxHealth = 200;
    Vector2 direction;
    private List<int> itemCollideID; //list of collision game object ids
    private GameObject resource;
    [SerializeField]
    private Toolbar toolbar;
    private bool inAction = false; //checks whether the player is in an animation
    private bool consumeCD = false;
    public int MaxHealth
    {
        get{return maxHealth;}
        set{maxHealth = value;}
    }
    void Awake()
    {
        _instance = this;
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.freezeRotation = true;
        itemCollideID = new List<int>();
        
    }
    // Start is called before the first frame update

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (speed == 5)
                speed = 10;
            else
                speed = 5;
        }

        //scrolls toolbar slot with mouse wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            toolbar.updateToolbarIndex((int)Mathf.Round(-Input.mouseScrollDelta.y));
        }
        //moves the slot selected in the tool bar
        if (Input.GetKeyDown(KeyCode.X))
        {
            toolbar.updateToolbarIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            toolbar.updateToolbarIndex(-1);
        }
        //checks collision and whether the player is facing the collision object
        //if so, performs an action upon left click or pressing z   
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && !inAction && !isMouseOverUI())
        {
            //checks for what actions to perform based on the first object hit by the raycast
            GameObject objectCollide = facingCollision();
            if (objectCollide != null)
            {
                if( objectCollide.tag == "Crafting")
                {
                    craftItem(objectCollide);
                }
                if (objectCollide.tag == "Water")
                {
                    updateThirst(100);
                }

                //checks which toolbar item is actively selected and performs the action
                //associated with the active item
                if (toolbar.getActiveItem() != null)
                {
                    switch (toolbar.getActiveItem().Type)
                    {   
                        case "Pickaxe":  
                            if(objectCollide.tag == "Ore")
                            {
                                StartCoroutine(mineOre(objectCollide));
                            }
                            if (objectCollide.tag == "HealingCrystal")
                            {
                                StartCoroutine(destroyCrystal(objectCollide));
                            }
                            break;
                        case "Axe":  
                            if(objectCollide.tag == "Tree")
                            {
                                StartCoroutine(chopTree(objectCollide));
                            }
                            break;
                        case "Bomb":
                            if(objectCollide.tag == "Boulder")
                            {
                                Destroy(objectCollide.gameObject);
                            }
                            break;
                        case "Water":
                            if(objectCollide.tag == "Lava")
                            {
                                Destroy(objectCollide.gameObject);
                            }
                            break;
                    }
                }
            }

            //checks for active toolbar items and performs their action
            //regardless of whether there is an active collision or not
            if (toolbar.getActiveItem() != null)
            {
                switch(toolbar.getActiveItem().Type)
                {
                    case "Sword":
                        StartCoroutine(swordAttack());
                        break;
                    case "Bow":
                        StartCoroutine(bowAttack());
                        break;
                    case "Meat":
                        consumeItem((Consumable)toolbar.getActiveItem());
                        break;
                    case "Potion":
                        consumeItem((Consumable)toolbar.getActiveItem());
                        break;
                }
            }
        } 
    }
    void FixedUpdate()
    {
        //gets velocity and direction of movement based on arrow keyss
        if (!inAction)
        {
            Rigidbody.MovePosition(Rigidbody.position + move.normalized * speed * Time.deltaTime);
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if(move.normalized != Vector2.zero) {
                direction = move.normalized;
            }
        }
        else
        {
            Rigidbody.MovePosition(Rigidbody.position);
        }
        //animates 8-directional walking movement
        animator.SetFloat("Horizontal", move.x);
        animator.SetFloat("Vertical", move.y);
        animator.SetFloat("Magnitude", move.magnitude);
        
        //animates 8-directional standing sprite
        animator.SetFloat("FacingX", direction.x);
        animator.SetFloat("FacingY", direction.y);
        animator.SetBool("InAction", inAction);
        if (!inAction)
        {
            //Updates rigidbody position
        }
    }

    //checks if the mouse is over certain ui elements
    public static bool isMouseOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        for (int i = 0; i < results.Count; i++)
        {
            string tag = results[i].gameObject.tag;
            if (tag == "Inventory" || tag == "Profile" || tag == "Toolbar")
            {
                return true;
            }
        }
        return false;
    }
    void setClickDirection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mPos.z = 0;
            Vector3 offset = mPos - transform.position;
            direction = offset.normalized;
        }   
    }
    public Vector2 FacingDirection
    {
        get{return direction;}
    }
    public static Player Instance
    {
        get
        {
            return _instance;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        
        //checks for collision with a nearby resource
        itemCollideID.Add(collision.gameObject.GetInstanceID());
    }

    void OnCollisionExit2D(Collision2D collision)
    {   
        //checks for leaving collision with a nearby resource
        itemCollideID.Remove(collision.gameObject.GetInstanceID());
    }
    //returns object hit by raycast
    public GameObject facingCollision()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction);   
        if (hit.Length >= 2 && hit[1].collider != null && hit[1].collider.gameObject != null) 
        { 
            GameObject objectCollide = hit[1].collider.gameObject;
            if (itemCollideID.Contains(objectCollide.GetInstanceID()))
            {
                return objectCollide;
            }
        }
        return null;
    }
    public void updateFatigue(int amount)
    {
        fatigue += amount;
        if (fatigue < 0)
        {
            fatigue = 0;
        } 
        else if (fatigue > 100)
        {
            fatigue = 100;
        }
    }
    public int getFatigue()
    {
        return fatigue;
    }
    public void setFatigue(int value)
    {
        fatigue = value;
    }
    public void updateHunger(int amount)
    {
        hunger += amount;
        if(hunger < 0)
        {
            hunger = 0;
        }
        else if (hunger > 100)
        {
            hunger = 100;
        }
    }

    public int getHunger()
    {
        return hunger;
    }
    public void setHunger(int value)
    {
        hunger = value;
    }
    public void updateThirst(int amount)
    {
        thirst += amount;
        if (thirst < 0)
        {
            thirst = 0;
        }
        else if (thirst > 100)
        {
            thirst = 100;
        }
    }
    public int getThirst()
    {
        return thirst;
    }
    public void setThirst(int value)
    {
        thirst = value;
    }

    public void updateHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0 && !inAction)
        {
            health = 0;
            StopAllCoroutines();
            OffBoat();
            sceneLoader.GameOver();
        }
    }
    public int getHealth()
    {

        return health;
    }
    public void setHealth(int value)
    {
        health = value;
    }
    void consumeItem(Consumable consumable)
    {
        if (!consumeCD)
        {
            updateHealth((int)consumable.HpRecovery);
            updateHunger((int)consumable.HungerRecovery);
            toolbar.updateQuantity(consumable.Id, -1);
            StartCoroutine(toolbar.setCooldown(1.0f, consumable.Type));
            StartCoroutine(consumeCoolDown());
        }
    }

    IEnumerator consumeCoolDown()
    {
        consumeCD = true;
        yield return new WaitForSeconds(1.0f);
        consumeCD = false;
    }
    //plays the gathering animation for a random amount of time and
    //adds the gathered resource to the player's item afterwards
    IEnumerator mineOre(GameObject gatherNode)
    {
        inAction = true;
        animator.SetTrigger("IsMining");         
        yield return new WaitForSeconds(Random.Range(1.0f,3.0f));   
        Gathering gatherInstance = gatherNode.GetComponent<Gathering>();
        if (gatherInstance.FatigueCost + fatigue <= maxFatigue)
        {
            updateFatigue(gatherInstance.FatigueCost);
            gatherInstance.gather();
        }  
        inAction = false;
    }
    IEnumerator destroyCrystal(GameObject crystal)
    {
        inAction = true;
        animator.SetTrigger("IsMining");         
        yield return new WaitForSeconds(Random.Range(2.0f,3.0f));   
        Destroy(crystal);
        inAction = false;
    }
    //plays the gathering animation for a random amount of time and
    //adds the gathered resource to the player's item afterwards
    IEnumerator chopTree(GameObject gatherNode)
    {
        inAction = true;
        animator.SetTrigger("IsChopping");         
        yield return new WaitForSeconds(Random.Range(1.0f,3.0f));   
        Gathering gatherInstance = gatherNode.GetComponent<Gathering>();
        if (gatherInstance.FatigueCost + fatigue <= maxFatigue)
        {
            updateFatigue(gatherInstance.FatigueCost);
            gatherInstance.gather();
        }  
        inAction = false;
    }
    //starts sword attack animation
    IEnumerator swordAttack()
    {
        PlayerStats pStat = GetComponent<PlayerStats>();
        inAction = true;
        setClickDirection();
        animator.SetTrigger("SwordAttack");
        animator.speed = pStat.PStats.AttackSpeed;
        yield return new WaitForSeconds(1.0f/(2*animator.speed));
        swordDamage();
        yield return new WaitForSeconds(1.0f/(2*animator.speed));
        animator.speed = 1;
        inAction = false;
    }
    void swordDamage()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] collide = Physics2D.OverlapCircleAll(pos+direction, 1);
        for (int i = 0; i < collide.Length; i++)
        {
            if (collide[i].gameObject.tag == "Mob")
            {
                PlayerStats pStat = GetComponent<PlayerStats>();
                DamageCalc.Instance.playerToMob(pStat, collide[i].gameObject.GetComponent<MobStats>());
            }
        }

    }
    public void OnBoat()
    {
        GameObject boat = transform.GetChild(0).gameObject;
        boat.SetActive(true);
        Animator boatAnimator = boat.GetComponent<Animator>();
        boatAnimator.SetFloat("FacingX", direction.x);
        boatAnimator.SetFloat("FacingY", direction.y);
    }
    public void OffBoat()
    {
        GameObject boat = transform.GetChild(0).gameObject;
        boat.SetActive(false);
    }
    //starts bow attack animation
    IEnumerator bowAttack()
    {
        PlayerStats pStat = GetComponent<PlayerStats>();
        inAction = true;
        animator.SetTrigger("BowAttack");
        setClickDirection();
        animator.speed = pStat.PStats.AttackSpeed;
        yield return new WaitForSeconds(1.0f/(2.0f*animator.speed));
        StartCoroutine(shootArrow());
        yield return new WaitForSeconds(1.0f/(2.0f*animator.speed));
        animator.speed = 1;
        inAction = false;
    }
    //creates an arrow and shoots it in the direction the player is facing
    IEnumerator shootArrow()
    {
        GameObject newArrow = arrow.createPlayerArrow(transform.position, direction, arrowSpeed);
        yield return new WaitForSeconds(3.0f);
        Destroy(newArrow);

    }
    //opens the crafting item user interface when the player
    //is facing the furnace and left clicks
    public void craftItem(GameObject craftObject)
    {
        if (craftObject.name == "Workbench")
        {
            uimanager.GetComponent<UIManager>().openCraftingMenu();
        }
        else if (craftObject.name == "Furnace")
        {
            uimanager.GetComponent<UIManager>().openSmeltingMenu();
        }
    }
}
