using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform expBar;
    [SerializeField]
    private Text expText;
    private static ExperienceUI _instance;
    private float xSize;
    private float targetSize = 0;
    private bool updating = false;
    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        xSize = expBar.sizeDelta.x;
    }
    void Update()
    {
        //animates exp bar 
        if (expBar.sizeDelta.x < targetSize && expBar.sizeDelta.x < xSize)
        {
            expBar.sizeDelta += new Vector2(targetSize/150,0);
            if(expBar.sizeDelta.x >= xSize)         
            {
                expBar.sizeDelta = new Vector2(xSize, expBar.sizeDelta.y);
            }   

        }
        else if (expBar.sizeDelta.x >= xSize)
        {
            expBar.sizeDelta = new Vector2(0,expBar.sizeDelta.y);
            updating = false;
        }
        else
        {
            updating = false;
        }
    }
    public static ExperienceUI Instance
    {
        get{return _instance;}
    }
    public bool Updating
    {
        get{return updating;}
    }
    public void updateExpBar(float currentExp, float expNxtLv, int level)
    {
        float expPercent = currentExp/expNxtLv;
        float roundedPercent = Mathf.Round(expPercent*10000f)/100f;
        expText.text = "Lv " + level + "  " + currentExp + " (" + roundedPercent + "%)";
        targetSize = expPercent*xSize;
        updating = true;
    }
}
