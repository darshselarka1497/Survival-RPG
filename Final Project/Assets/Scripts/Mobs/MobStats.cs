

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MobStats : MonoBehaviour
{
    [SerializeField]
    private GameObject itemDropPrefab;
    [SerializeField]
    private float health = 10000.0f;
    [SerializeField]
    private float minAttack = 5.0f;
    [SerializeField]
    private float maxAttack = 10.0f;
    [SerializeField]
    private float attack = 10.0f;
    [SerializeField]
    private float defense = 1.0f;
    [SerializeField]
    private float respawnTime = 5.0f;
    [SerializeField]
    private string mobType;
    [SerializeField]
    private float experience;
    [SerializeField]
    private List<float> dropRates;
    [SerializeField]
    private List<int> dropId;
    private bool recentDamageTaken = false;
    private float originalHealth;
    
    void Awake()
    {
        originalHealth = health;
    }
    void Start()
    {
        MobUI.Instance.addMob(this);
    }
    void OnEnable()
    {
        health = originalHealth;
    }
    public float Experience
    {
        get{return experience;}
    }
    public string MobType
    {
        get{return mobType;}
    }
    public float RespawnTime
    {
        get{return respawnTime;}
    }
    public float Health
    {
        get{return health;}
        set{health = value;}
    }
    public float MaxHealth
    {
        get{return originalHealth;}
    }
    public float MinAttack
    {
        get{return minAttack;}
    }
    public float MaxAttack
    {
        get{return maxAttack;}
    }
    public float Attack
    {
        get{return attack;}
    }    
    public float Defense
    {
        get{return defense;}
    }

    public void updateHealth(float amount)
    {
        health += amount;
        if (health > originalHealth)
        {
            health = originalHealth;
        }

        StopCoroutine(destroyHealthBar());
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(destroyHealthBar());
        }     
        MobUI.Instance.updateHealthBar(this);
        checkDeath();
    }
    IEnumerator destroyHealthBar()
    {
        yield return new WaitForSeconds(10.0f);
    
        //destroys healthbar ui if no damage has been taken recently
        MobUI.Instance.destroyHealthBar(this);
    }
    public void checkDeath()
    {
        if (health <= 0)
        {
            //destroys health and damage text ui for the mob
            MobUI.Instance.destroyUI(this);
            //adds experience to player
            addExp();

            //drops item and starts mob respawn if it's not a boss
            if (mobType != "Boss")
            {
                dropItemOnDeath();
                StartCoroutine(MobUI.Instance.respawn(this.gameObject, respawnTime));
            }
        }
    }

    void addExp()
    {
        PlayerStats playerStats = Player.Instance.GetComponent<PlayerStats>();
        playerStats.addExp(experience*Modifiers.Instance.ExpRate);
    }
    void dropItemOnDeath()
    {
        for (int i = 0; i < dropRates.Count; i++)
        {
            float randVal = Random.Range(0.0f,1.0f);
            float dropRate = dropRates[i] * Modifiers.Instance.DropRate;
            if (randVal <= dropRate)
            {
                dropItem(dropId[i]);
            }
        }
    }
    void dropItem(int id)
    {
        Item item = ItemDatabase.Instance.createItem(id);
        GameObject itemDrop = Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        itemDrop.GetComponent<ItemDrop>().drop(item);

    }

   
}
