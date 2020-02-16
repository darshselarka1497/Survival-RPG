using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Recipe
{
    public int id; //id of crafted item
    public string name; //name of crafted item
    public List<MaterialInfo> materials; //list of materials and their information

    [System.Serializable]
    public class MaterialInfo
    {
        public int materialId; //id of material
        public string materialName; //name of material
        public int materialQuantity; //number of material needed
    }
}
