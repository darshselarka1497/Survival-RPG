using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Vector3 from; //from position boundary
    [SerializeField]
    private Vector3 to; //to position boundary
    [SerializeField]
    private int numSpawns; //number of mobs to spawn
    [SerializeField]
    private List<GameObject> prefabs; //list of mob prefabs to spawn
    private List<Vector3> spawnSpots;
    // Start is called before the first frame update
    void Start()
    {
        spawnSpots = new List<Vector3>();
        buildSpawnLocation();
        spawnMobs();
    }

    //builds a list of spawnable spots(no collisions)
    void buildSpawnLocation()
    {        
        int xsize = (int)Mathf.Abs((to.x-from.x)+1);
        int ysize = (int)Mathf.Abs((to.y-from.y)+1);

        //adds vector to spawn spots if there is no collisions on tile
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < ysize; j++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(from.x+i,from.y+j), Vector2.zero); 
                bool neighborHit = checkNeighborHit((int)from.x+i,(int)from.y+j);
                if (!hit && neighborHit)
                {
                    spawnSpots.Add(new Vector3(from.x+i, from.y+j, 0));
                }
            }
        }
    }

    //checks if there's at least 4 neighboring spots to move to
    bool checkNeighborHit(int xPos, int yPos)
    {
        int[] neighbors = new int[]{-1,0,1};
        int numSpotsAvail = 0;
        for (int i = 0; i < neighbors.Length; i++)
        {
            for (int j = 0; j < neighbors.Length; j++)
            {
                Vector2 neighbor = new Vector2(xPos+neighbors[i], yPos+neighbors[j]);
                RaycastHit2D hit = Physics2D.Raycast(neighbor, Vector2.zero); 
                if (!hit)
                {
                    numSpotsAvail++;
                }
            }
        }
        if (numSpotsAvail > 4)
        {
            return true;
        }
        return false;
    }

    //randomly spawns a mob from the mob list in a random position
    void spawnMobs()
    {
        for (int i = 0; i < numSpawns; i++)
        {
            int randPrefab = Random.Range(0, prefabs.Count);
            int randSpot = Random.Range(0, spawnSpots.Count);
            Instantiate(prefabs[randPrefab], spawnSpots[randSpot], Quaternion.identity);
        }
    }
}
