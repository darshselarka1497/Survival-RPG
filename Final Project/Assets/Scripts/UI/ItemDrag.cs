using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Transform originalParent;
    private GameObject dragObject;
    private bool isToolbarItem = false;
    // Start is called before the first frame update

    void Start()
    {
        dragObject = this.gameObject;
    }
    public bool IsDragging
    {
        get{return isDragging;}
        set{isDragging = value;}
    }
    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            RectTransform imageRect = dragObject.GetComponent<RectTransform>();
            //sets the item display to the mouse cursor's position
            Transform ui = GameObject.FindGameObjectWithTag("ForegroundUI").transform;
            dragObject.transform.SetParent(ui);
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = ui.position.z;
            imageRect.position = mousePosition;
        }
        
    }
    
    //copies original recttransform properties and set
    //isDragging to true
    public void OnDrag()
    {
        RectTransform imageRect = dragObject.GetComponent<RectTransform>();
        originalPosition = imageRect.localPosition;
        originalParent = imageRect.parent;
        isDragging = true;

      }
    public void stopDrag()
    {
        isDragging = false;
    }
    public void destroyObject()
    {

        Destroy(dragObject.gameObject);
        Destroy(originalParent.gameObject);
    }
    //restore original recttransform properties
    public void returnToOriginal()
    {
        stopDrag();
        dragObject.GetComponent<RectTransform>().SetParent(originalParent);
        dragObject.GetComponent<RectTransform>().localPosition= originalPosition;
    }
    //handles when the item dragged is from the toolbar
    public void OnToolbarDrag(GameObject drag)
    {
        isToolbarItem = true;
        dragObject = drag;
        OnDrag();
    }
    
}
