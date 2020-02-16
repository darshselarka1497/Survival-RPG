using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobUI : MonoBehaviour
{
    [SerializeField]
    private GameObject healthBarPrefab; //prefab of health bar
    [SerializeField]
    private GameObject damageTextPrefab; //prefab for damage text
    [SerializeField]
    private RectTransform canvas; //canvas to create damage text in
    private Dictionary<MobStats, GameObject[]> mobDict; //stores the healthbar and damage text layout for each mob 
    private static MobUI _instance; //this script instance
    void Awake()
    {
        mobDict = new Dictionary<MobStats, GameObject[]>();
        _instance = this;
    }
    // Start is called before the first frame update
    public static MobUI Instance
    {
        get{return _instance;}
    }
    void Update()
    {   
        IDictionaryEnumerator dictEnum = mobDict.GetEnumerator();
        //iterates through all the mobs with a healthbar and damage text and
        //updates them accordingly
        while(dictEnum.MoveNext())
        {    
            GameObject[] val;
            MobStats mob = (MobStats)dictEnum.Key;
            Vector3 pos = mob.transform.position;
            mobDict.TryGetValue(mob, out val);
            GameObject healthBar = val[0];
            GameObject damageTextLayout = val[1];  
            if (healthBar != null)
            {
                updateHealthBarPos(healthBar,pos);
            }
            if(damageTextLayout != null)
            {
                updateDamageLayoutPos(damageTextLayout,pos);
                movingDamage(damageTextLayout);
            }
        }
        
    }
    //updates health bar's position to the mob's position
    public void updateHealthBar(MobStats mob)
    {
        GameObject[] mobInfo = getMobInfo(mob);
        //ignores healthbar if there is no prefab for it(boss has different health bar)
        if (mob.MobType == "Boss")
        {
            return;
        }

        //creates a new health bar if the mob doesn't have one
        if (mobInfo[0] == null)
        {
            mobInfo[0] = Instantiate(healthBarPrefab, canvas);
        }
        updateHealthBarPos(mobInfo[0], mob.transform.position);
        RectTransform healthGauge = mobInfo[0].transform.GetChild(1).GetComponent<RectTransform>();
        
        //calculates health bar size based on percentage of hp remaining
        float healthPercent = mob.Health/mob.MaxHealth;
        if (healthPercent <= 0)
            healthPercent = 0;
        healthGauge.sizeDelta = new Vector2(healthPercent*100,healthGauge.sizeDelta.y);
        
    }

    //updates the health bar position to mob's position
    //since canvas and game's position are in different planes
     void updateHealthBarPos(GameObject healthBar, Vector3 pos)
    {
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        healthBar.GetComponent<RectTransform>().position = cam.WorldToScreenPoint(pos+new Vector3(0,1,0));
    }

    //updates damage layout position to mob's position
    void updateDamageLayoutPos(GameObject damageTextLayout, Vector3 pos)
    {

        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        damageTextLayout.GetComponent<RectTransform>().position = cam.WorldToScreenPoint(pos);
    }

    //damage moves up over time and fades away
    void movingDamage(GameObject damageTextLayout)
    {
        //
        Transform damage = damageTextLayout.transform;
        damageTextLayout.GetComponent<VerticalLayoutGroup>().enabled = false;

        //index starts as one since the first child is used as a prefab
        for (int i = 1; i < damage.childCount; i++)
        {
            Transform damageText = damage.GetChild(i);

            //moves damage in a quadratic curve
            Vector3 damagePos = damageText.GetComponent<RectTransform>().position;
            GameObject damagePrefab = damageTextLayout.transform.GetChild(0).gameObject;
            Vector3 originalPos = damagePrefab.GetComponent<RectTransform>().position;
            float newX = (damagePos-originalPos).x + Time.deltaTime*5;
            Vector3 newPos = new Vector3(newX+originalPos.x, -((newX-5)*(newX-5))+originalPos.y,0);
            damageText.GetComponent<RectTransform>().position = newPos;
            //damagePos += new Vector3(0,1,0);

            //fades the damage text
            damageText.GetComponent<Text>().CrossFadeAlpha(0f,3.0f,true);
        }
    }
    public GameObject[] getMobInfo(MobStats mob)
    {
        GameObject[] val;
        mobDict.TryGetValue(mob, out val);
        return val;
    }
    public void addMob(MobStats mob)
    {
        mobDict.Add(mob, new GameObject[2]);
    }
    
    //takes the damage to the mob and creates a damage text gameobject 
    //as child of damagetextlayout and changes the text color to yellow
    //if the damage is a critical hit
    public IEnumerator displayDamage(MobStats mob, float damage, bool criticalHit)
    {
        //creates the damage object
        GameObject damageTextLayout = getMobInfo(mob)[1];
        GameObject damageText = damageTextLayout.transform.GetChild(0).gameObject;
        Vector3 pos = damageText.GetComponent<RectTransform>().position;
        damageText = Instantiate(damageText, pos, Quaternion.identity, damageTextLayout.transform);
        Text dmgText = damageText.GetComponent<Text>();
        dmgText.text = ((int)damage).ToString();

        //checks if the damage is a critical hit and adjusts the color
        if (criticalHit)
        {
            dmgText.fontSize = 30;
            dmgText.color = new Color(255,177,0,1);
        }
        damageText.GetComponent<Text>().enabled = true;

        //wait time before damage disappears
        yield return new WaitForSeconds(2.5f);
        Destroy(damageText);
    }
    
    public void setDamageLayout(MobStats mob)
    {
        GameObject[] mobInfo = getMobInfo(mob);
        if (mobInfo[1] == null)
        {
            mobInfo[1] = Instantiate(damageTextPrefab,canvas);
        }

    }

    public void destroyHealthBar(MobStats mob)
    {
        
        if (mob.MobType == "Boss")
        {
            return;
        }
        

        GameObject[] mobInfo = getMobInfo(mob);
        Destroy(mobInfo[0]);
        mobInfo[0] = null;
    }

    //destroys any ui objects associated with the mob and starts mob respawn time
    public void destroyUI(MobStats mob)
    {
        GameObject[] mobInfo = getMobInfo(mob);
        Destroy(mobInfo[0]);
        Destroy(mobInfo[1]);
    }

    public void removeMob(MobStats mob)
    {
        mobDict.Remove(mob);
    }

    
    //disables mob gameobject and enables it after a certain duration
    public IEnumerator respawn(GameObject mob, float respawnTime)
    {  
        mob.SetActive(false);
        if (respawnTime >= 0)
        {
            yield return new WaitForSeconds(respawnTime);
            mob.SetActive(true);
        }
        else
        {
            yield return null;
        }
    }
}
