using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour
{
    private float expRate = 1.0f;
    private float dropRate = 1.0f;
    private float mobHP = 1.0f;
    private static Modifiers _instance;
    void Start()
    {
        _instance = this;
    }
    public static Modifiers Instance
    {
        get{return _instance;}
    }
    public float ExpRate
    {
        get{return expRate;}
        set{expRate = value;}
    }
    public float DropRate
    {
        get{return dropRate;}
        set{dropRate = value;}
    }
    public float MobHP
    {
        get{return mobHP;}
        set{mobHP = value;}
    }
}
