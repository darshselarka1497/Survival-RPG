using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    //constants
    [SerializeField]
    private float chasingSpeed = 3.0f; //speed when chasing player
    [SerializeField]
    private Vector3 searchRadius = new Vector3(10,10,0); //search radius for path to player
    [SerializeField]
    private float roamWaitTimeMax = 2.0f; //maximum time to wait in between roaming
    [SerializeField]
    private float roamWaitTimeMin = 0.0f; //minimum time to wait in between roaming
    [SerializeField]
    private int leashDistance = 20; //farthest a mob can move away from its starting position
    [SerializeField]
    private int aggroDistance = 10; //distance between player and mob before mob becomes aggroed
    [SerializeField]
    private float deaggroTime = 5.0f; //time until the mob deaggros
    [SerializeField]
    private float roamingSpeed = 1.0f; //speed when roaming
    [SerializeField]
    private int roamRange = 5; //picks random int between +- range and adds to originalpos x and y values
    [SerializeField]
    protected float attackSpeed = 5.0f; //wait time between attacks
    [SerializeField]
    private bool aggressive = false; //checks if the mob will chase the player

    protected GameObject player;
    [SerializeField]
    protected Animator animator;
    protected Vector3 facingDirection;
    protected bool attackCD; //checks if attack is on cooldown

    //pathfinding
    [SerializeField]
    private Pathfinding pathfinder;
    private List<Vector3> path;
    private int pathIndex = -1;
    private float speed; //movement speed
    private Vector3 lastPlayerPathPos;
    private Vector3 originalPos; //starting position

    //roaming
    private Vector3 roamPos; //pos to move to when roaming
    private bool roaming; //checks whether the mob is roaming or not
    private bool roamWait;

    //aggro
    private bool aggroed; //whether the mob is aggroed
    private bool walkingBack; //checks if mob is walking back to original pos

    //Path visualization
    public bool Visualize = false;
    private GameObject[] pathVisTile = new GameObject[0];
    [SerializeField]
    private GameObject pathTile; //only assign this if you want to visualize the path
    private Rigidbody2D rigidbody;
    void Awake()
    {
        GetComponent<Rigidbody2D>().freezeRotation = true;
        originalPos = transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        attackCD = false;
        walkingBack = false;
        aggroed = false;
        roaming = false;
        roamWait = false;
        transform.position = originalPos;
        player = Player.Instance.gameObject;
        lastPlayerPathPos = new Vector3(0,0,100);

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 mobPos = transform.position;

        //checks if player is within mob's aggro distance
        checkAggro(playerPos, mobPos);
        normalMobBehavior();
    }
    protected void normalMobBehavior()
    {        
        Vector3 playerPos = player.transform.position;
        Vector3 mobPos = transform.position;
        //starts pathfinding if mob is not next to player and within aggro distance
        if (aggressive && aggroed && Vector3.Distance(playerPos,originalPos) <= leashDistance)
        {
            facingDirection = playerPos - mobPos;
            //starts pathfinding if mob is not next to player
            if (Vector3.Distance(mobPos, playerPos) > 1)
            {
                //finds a new path if the player has moved to a new position
                if (Vector3.Distance(playerPos,lastPlayerPathPos) >= 1)
                {
                    //finds a path to the player
                    findPlayerPath();
                }
                moveToPos(mobPos,playerPos);
            }
            else if (!attackCD)
            {
                StartCoroutine(attack());
            }
        }
        //deaggros if player isn't nearby
        else
        {
            //regenerates health over time if not at max health
            MobStats mobstat = GetComponent<MobStats>();
            if(mobstat.Health < mobstat.MaxHealth && aggressive)
            {
                mobstat.updateHealth(Time.deltaTime*mobstat.MaxHealth/2);
            }
            //find path to original position if the mob isn't near it and
            //isn't walking back yet and isn't roaming
            if (Vector3.Distance(mobPos, originalPos) > .5 && !walkingBack && !roaming)
            {
                //finds a path to original position
                findReturnPath();
            }
            //follows the path to the original position
            else if (Vector3.Distance(mobPos,originalPos) > .5 && walkingBack && !roaming)
            {
                moveToPos(mobPos,originalPos);
            }
            //randomly roams around the mob's spawning point
            else
            {
                randomRoam();
            }

        }

    }
    void findNewPath(Vector3 searchRange, Vector3 target)
    {
        Vector3 mobPos = transform.position;
        DestroyPath();// for path visualization

        //saves the next tile to move to in old path
        //to avoid back and forth movement between new path and old path
        Vector3 oldPath = transform.position;
        if (path != null && pathIndex >=0 && pathIndex < path.Count)
        {
            oldPath = path[pathIndex];
        }

        path = pathfinder.findPath(mobPos,target,mobPos - searchRange,mobPos + searchRange);
        VisualizePath();//for path visualization
        pathIndex = path.Count-1;
        
        updateNewIndex(oldPath); 
    }
    IEnumerator attack()
    {
        attackCD = true;
        animator.speed = attackSpeed;
        yield return new WaitForSeconds(1/(2*animator.speed));
        DamageCalc.Instance.mobToPlayer(player.GetComponent<PlayerStats>(), GetComponent<MobStats>());
        yield return new WaitForSeconds(1/(2*animator.speed));
        attackCD = false;
        animator.speed = 1;
    }
    protected void checkAggro(Vector3 playerPos, Vector3 mobPos)
    {
        if (Vector3.Distance(playerPos, mobPos) <= aggroDistance && Vector3.Distance(playerPos, originalPos) <= leashDistance)
        {
            aggroed = true;
            StopCoroutine(stoppingAggro());
        }
        else if (aggroed)
        {
            StartCoroutine(stoppingAggro());
        }
    }
    IEnumerator stoppingAggro()
    {
        yield return new WaitForSeconds(deaggroTime);
        aggroed = false;
    }
    private void randomRoam() 
    {
        speed = roamingSpeed;
        roaming = true;
        if (pathIndex <= 0 && !roamWait)
        {
            roamWait = true;
            StartCoroutine(roamWaitingTime());
        }
        moveToPos(transform.position,roamPos);
    }
    //random pauses between roaming
    IEnumerator roamWaitingTime()
    {
        float randomWait = Random.Range(roamWaitTimeMin, roamWaitTimeMax);
        yield return new WaitForSeconds(randomWait);
        Vector3 randomRoamRange = new Vector3(Random.Range(-roamRange,roamRange), Random.Range(-roamRange,roamRange), 0);
        roamPos = originalPos + randomRoamRange;
        findNewPath(new Vector3(roamRange,roamRange,0), roamPos);
        roamWait = false;
    }
    //if next tile in old path is in new path, update the index
    //to where the tile is in the new path
    void updateNewIndex(Vector3 oldPath)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (oldPath == path[i])
            {
                pathIndex = i;
            }
        }
    }

    private void moveToPos(Vector3 mobPos, Vector3 targetPos)
    {
        Vector3 moveDirection;
        if (pathIndex>= 0)
        {            
            moveDirection = path[pathIndex]-mobPos;
            if (Vector3.Distance(mobPos,path[pathIndex]) < .5)
            {
                pathIndex--;
            }
        } 
        else if (!roaming)
        {
            moveDirection = player.transform.position-mobPos;     
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        //transform.position += moveDirection.normalized * speed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + (Vector2)moveDirection.normalized * speed * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            facingDirection = moveDirection.normalized;
        }
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Magnitude", moveDirection.magnitude);
        
        animator.SetFloat("FacingX", facingDirection.x);
        animator.SetFloat("FacingY", facingDirection.y);

    }
    public void dealDamage()
    {

    }
    //visualize the path in red
    void VisualizePath()
    {
        if (!Visualize ||pathTile == null)
            return;
        pathVisTile = new GameObject[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            pathVisTile[i] = Instantiate(pathTile, path[i], Quaternion.identity);
        }
    }
    //destroys visualization tiles
    void DestroyPath()
    {
        for (int i = 0; i < pathVisTile.Length; i++)
        {
            Destroy(pathVisTile[i]);
        }
    }
    private void findPlayerPath()
    {
        roaming = false;
        walkingBack = false;
        speed = chasingSpeed;
        findNewPath(searchRadius, player.transform.position);
        //records last player position to check if player has moved or not
        lastPlayerPathPos = player.transform.position;
    }
    private void findReturnPath()
    {
        speed = roamingSpeed;
        walkingBack = true;
        findNewPath(new Vector3(leashDistance,leashDistance,0), originalPos);
    }
}