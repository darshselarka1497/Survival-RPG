using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBoss : MonoBehaviour
{
    [SerializeField]
    private GameObject swordPrefab;
    [SerializeField]
    private GameObject portal;
    [SerializeField]
    private GameObject sceneTransition;
    [SerializeField]
    private RectTransform healthBar;
    private MobStats bossStat;
    private bool isAttacking = false; //checks if the boss is attacking
    // Start is called before the first frame update
    void Start()
    {
        bossStat = GetComponent<MobStats>();
    }

    // Update is called once per frame
    void Update()
    {
        bossDeath();
        phaseUpdate();

        //healthbar update
        healthBar.sizeDelta = new Vector2(300.0f/bossStat.MaxHealth*bossStat.Health,healthBar.sizeDelta.y);
    }
    void phaseUpdate()
    {
        if (!isAttacking)
        {
            MobStats mobStat = GetComponent<MobStats>();
            float hpPercent = mobStat.Health/mobStat.MaxHealth;
            float randomRecallTime = Random.Range(-1f, 0);
            float recallChance = Random.Range(0f,1f);
            if (hpPercent <= 1)
            {
                createSwords(3, 5, 5+randomRecallTime, 1f, 45, recallChance);
            }
            if (hpPercent <= .75)
            {
                createSwords(4, 7.5f, 6+randomRecallTime, 1.5f, 90, recallChance);
            }
            if (hpPercent <= .5)
            {
                createSwords(5, 10, 7+randomRecallTime, 2f, 135, recallChance);
            }
            if (hpPercent <= .25)
            {
                createSwords(6, 12.5f, 8+randomRecallTime, 2.5f, 180, recallChance);
            }
            StartCoroutine(swordAttack());
        }

    }
    //cooldown between attacks
    IEnumerator swordAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(8.5f);
        while(GameObject.FindGameObjectWithTag("BossSword") != null)
        {
            yield return null;
        }
        isAttacking = false;
    }

    //instantiates the swords
    void createSwords(int amount, float radius, float recallTime, float recallSpeed, float rotationSpeed, float recallChance)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject sword = Instantiate(swordPrefab,transform.position,Quaternion.identity);
            BossSword bossSword = sword.GetComponent<BossSword>();
            bossSword.release(this.gameObject, recallTime, recallSpeed, i * 360/amount, radius, rotationSpeed, recallChance);
        }
    }
    void bossDeath()
    {
        if (GetComponent<MobStats>().Health <= 0)
        {
            StopAllCoroutines();
            MobUI.Instance.removeMob(bossStat);
            Destroy(healthBar.parent.gameObject);
            sceneTransition.SetActive(true);
            portal.SetActive(true);
            Destroy(this.gameObject);

        }
    }

}
