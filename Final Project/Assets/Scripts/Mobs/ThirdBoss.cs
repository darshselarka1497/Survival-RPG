using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBoss : MonoBehaviour
{
    [SerializeField]
    private GameObject clonePrefab;
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private GameObject portal;
    [SerializeField]
    private GameObject sceneTransition;
    private bool isAttacking = false;
    private MobStats bossStat;
    private int state = 0;
    private bool isClone = false;
    [SerializeField]
    private RectTransform healthBar;
    private Vector3[] spawnPoints;
    private List<GameObject> clones;

    // Start is called before the first frame update
    void Start()
    {
        bossStat = GetComponent<MobStats>();
        spawnPoints = new Vector3[5];
        spawnPoints[0] = new Vector3(0,13,0);
        spawnPoints[1] = new Vector3(0,45,0);
        spawnPoints[2] = new Vector3(55,13,0);
        spawnPoints[3] = new Vector3(25,30,0);
        spawnPoints[4] = new Vector3(50,45,0);
        clones = new List<GameObject>();
    }

    IEnumerator ProjectileAttack()
    {
        isAttacking = true;
        GameObject projectile = Instantiate(ProjectilePrefab,transform.position, Quaternion.identity);
        projectile.GetComponent<FireBall>().setMobStats(GetComponent<MobStats>());
        projectile.GetComponent<FireBall>().setTargetPosition(Player.Instance.transform.position);
        yield return new WaitForSeconds(3.0f);
        isAttacking = false;
        yield return new WaitForSeconds(10.0f);
        Destroy(projectile);

    }
    // Update is called once per frame
    void Update()
    {
        float percentHealth = bossStat.Health/bossStat.MaxHealth;
        if (!isClone)
        {
            healthBar.sizeDelta = new Vector2(300.0f*percentHealth,healthBar.sizeDelta.y);
            if (percentHealth < 0.75 && state == 0){
                int rand = Random.Range(0,5);
                transform.position = spawnPoints[rand];
                createClone(rand);
                state++;
            }
            if (percentHealth < 0.5 && state == 1){
                int rand = Random.Range(0,5);
                transform.position = spawnPoints[rand];
                createClone(rand);
                state++;
            }
            if (percentHealth < 0.25 && state == 2){
                int rand = Random.Range(0,5);
                transform.position = spawnPoints[rand];
                createClone(rand);
                state++;
            }
        }



        if(!isAttacking)
        {
            StartCoroutine(ProjectileAttack());
        }
        bossDeath();
    }
    void bossDeath()
    {
        if (bossStat.Health <= 0)
        {
            StopAllCoroutines();
            MobUI.Instance.removeMob(bossStat);
            if (!isClone)
            {
                Destroy(healthBar.parent.gameObject);
                sceneTransition.SetActive(true);
                portal.SetActive(true);
                destroyAllClones();
            }
            Destroy(this.gameObject);
        }

    }
    public bool IsClone
    {
        set{isClone = value;}
    }
    void createClone(int index)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i != index)
            {
                GameObject clone = Instantiate(clonePrefab, spawnPoints[i], Quaternion.identity);
                clone.GetComponent<ThirdBoss>().IsClone = true;
                clones.Add(clone);
            }
        }
        
    }
    void destroyAllClones()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            if(clones[i] != null)
            {
                MobUI.Instance.removeMob(clones[i].GetComponent<MobStats>());
                Destroy(clones[i]);
            }
        }
    }
}
