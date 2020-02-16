using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour {

    public GameObject player;        
    private Vector3 offset;  
    private Camera camera;    
    private float maxZoomDistance = 12.0f;  
    private float minZoomDistance = 5.0f;

    // Use this for initialization
    void Start () 
    {
        camera = GetComponent<Camera>();
        //offset between camera and player position
        offset = transform.position - player.transform.position;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            camera.orthographicSize += .5f;
            if (camera.orthographicSize > maxZoomDistance)
                camera.orthographicSize = maxZoomDistance;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            camera.orthographicSize -= .5f;
            if (camera.orthographicSize < minZoomDistance)
                camera.orthographicSize = minZoomDistance;
        }
    }
    void LateUpdate () 
    {
        //adjust camera to the player's position
        transform.position = player.transform.position + offset;
    }
}
