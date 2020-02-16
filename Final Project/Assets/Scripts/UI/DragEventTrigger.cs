using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEventTrigger : EventTrigger
{
    public override void OnEndDrag(PointerEventData eventData)
    {
        Inventory inventory = GameObject.FindGameObjectWithTag("UIManager").GetComponent<Inventory>();

        inventory.endDrag(this.gameObject);
    }
}
