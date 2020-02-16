using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad;
    [SerializeField]
    private string sceneObjectPosition;
    private bool hasntEntered = true;
    public bool HasntEntered
    {
        set{hasntEntered = value;}
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasntEntered && collision.gameObject.name == "Player")
        {
            Vector3 collPos = collision.transform.position - transform.position;
            Vector3 size = transform.localScale;
            Vector3 offset;
            if (size.x < size.y)
            {
                offset = new Vector3(-collPos.x/size.x, collPos.y/size.y,0);
            }
            else
            {
                offset = new Vector3(collPos.x/size.x, -collPos.y/size.y,0);

            }
            Time.timeScale = 0;
            SceneLoad.Instance.load(sceneToLoad, sceneObjectPosition, offset);
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        hasntEntered = true;
    }
}
