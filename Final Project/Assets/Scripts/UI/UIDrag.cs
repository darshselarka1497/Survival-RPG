using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    public bool IsDragging
    {
        get{return isDragging;}
        set{isDragging = value;}
    }
    // Update is called once per frame
    void Update()
    {
        //moves ui to mouse positon+offset
        if (isDragging)
        {
            Transform ui = GameObject.FindGameObjectWithTag("UI").transform;
            RectTransform imageRect = GetComponent<RectTransform>();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            mousePosition.z = ui.position.z;
            imageRect.position = mousePosition;
        }
        
    }
    
    //stores offset between mouseposition and ui positon
    //and sets isDragging to true
    public void OnDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = GetComponent<RectTransform>().position - mousePosition;
        isDragging = true;

    }
    public void stopDrag()
    {
        isDragging = false;
    }
    
}
