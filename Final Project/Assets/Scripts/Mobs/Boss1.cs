using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [SerializeField]
    private GameObject wallOfFire;
    [SerializeField]
    private GameObject ringOfFire;
    [SerializeField]
    private GameObject fireBallPrefab;
    [SerializeField]
    private GameObject healCrystalPrefab;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private GameObject portal;
    [SerializeField]
    private GameObject sceneTransition;
    private GameObject fireBall;
    private float attackSpeed = 5.0f;
    private Vector3 staticPlayerPosition;
    private Vector3 leftRightMovement = new Vector3(1,0,0);
    private bool wallActive = false; //wait time between fire wall attacks
    private bool mainAttackActive = false; //wait time between fire ball attacks
    private bool waitCrystalSpawn = false;
    private MobStats bossStat;
    private List<GameObject> crystals;
    // Start is called before the first frame update
    void Start()
    {
        crystals = new List<GameObject>();
        bossStat = GetComponent<MobStats>();
        ringOfFire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        float currentHealth = bossStat.Health;
        float maxHealth = bossStat.MaxHealth;
        transform.position += leftRightMovement * 2 * Time.deltaTime;
        updateCrystalCount();
        if (!mainAttackActive && currentHealth > maxHealth * .5)
        {
            StartCoroutine(castFireBall());
        }
        else if (!mainAttackActive)
        {
            StartCoroutine(castFireBall2());
        }

        //boss phases for every 25% decrease in health
        healthBar.sizeDelta = new Vector2(300.0f/maxHealth*currentHealth,healthBar.sizeDelta.y);
        bossStat.updateHealth(bossStat.MaxHealth*.01f*Time.deltaTime*crystals.Count);
        if (currentHealth > maxHealth*.75)
        {
            ringOfFire.SetActive(false);
            if (!wallActive)
            {
                StartCoroutine(FireWall());   
            }

        }
        else if (currentHealth > maxHealth *.5 && currentHealth <= maxHealth*.75)
        {
            ringOfFire.SetActive(false);
            if (!wallActive)
            {
                StartCoroutine(FireWall());   
            }
            if (!waitCrystalSpawn)
            {
                StartCoroutine(SpawnHealCrystal());
            }
        }
        else if (currentHealth > maxHealth *.25 && currentHealth <= maxHealth*.5)
        {
            ringOfFire.SetActive(true);
            if (!waitCrystalSpawn)
            {
                StartCoroutine(SpawnHealCrystal());
            }
        }
        else
        {
            ringOfFire.SetActive(true);
            if (!wallActive)
            {
                StartCoroutine(FireWall());   
            }
            if (!waitCrystalSpawn)
            {
                StartCoroutine(SpawnHealCrystal());
            }
        }

        //moves left and right
        if (transform.position.x < 0)
        {
            leftRightMovement = new Vector3(1,0,0);
            transform.rotation = Quaternion.Euler(0,0,0);
        }
        else if (transform.position.x > 22)
        {
            leftRightMovement = new Vector3(-1,0,0);
            transform.rotation = Quaternion.Euler(0,180,0);
        }
        
        bossDeath();
        
    }
    void bossDeath()
    {
        if (bossStat.Health <= 0)
        {
            StopAllCoroutines();
            MobUI.Instance.removeMob(GetComponent<MobStats>());
            Destroy(healthBar.parent.gameObject);
            destroyCrystals();
            Destroy(ringOfFire);
            sceneTransition.SetActive(true);
            portal.SetActive(true);
            Destroy(this.gameObject);
        }

    }
    //wall of fire that moves down
    IEnumerator FireWall()
    {
        wallActive = true;
        Instantiate(wallOfFire);

        yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        wallActive = false;
    }
    //fire ball attack that's aimed at the player's position when the attack is cast
    IEnumerator castFireBall()
    {
        mainAttackActive = true;
        fireBall = Instantiate(fireBallPrefab,transform.position, Quaternion.identity);
        fireBall.GetComponent<FireBall>().setTargetPosition(Player.Instance.transform.position);
        fireBall.GetComponent<FireBall>().setMobStats(GetComponent<MobStats>());
        yield return new WaitForSeconds(7.0f);
        mainAttackActive = false;
    }
    //same fire ball but grows in size over time
    IEnumerator castFireBall2()
    {
        mainAttackActive = true;
        fireBall = Instantiate(fireBallPrefab,transform.position, Quaternion.identity);
        fireBall.GetComponent<FireBall>().setTargetPosition(Player.Instance.transform.position);
        fireBall.GetComponent<FireBall>().setMobStats(GetComponent<MobStats>());
        fireBall.GetComponent<FireBall>().setFireBallPhase2();
        yield return new WaitForSeconds(7.0f);
        mainAttackActive = false;
    }
    IEnumerator SpawnHealCrystal()
    {
        waitCrystalSpawn = true;
        Vector3 randomPos = new Vector3(Random.Range(1,20), Random.Range(-7,7),0);
        crystals.Add(Instantiate(healCrystalPrefab,randomPos,Quaternion.identity));
        yield return new WaitForSeconds(30.0f);
        waitCrystalSpawn = false;
    }

    void updateCrystalCount()
    {
        for (int i = 0; i < crystals.Count; i++)
        {
            if(crystals[i] == null)
            crystals.RemoveAt(i);
        }
    }
    void destroyCrystals()
    {    
        for (int i = 0; i < crystals.Count; i++)
        {
            Destroy(crystals[i]);
        }

    }
}
