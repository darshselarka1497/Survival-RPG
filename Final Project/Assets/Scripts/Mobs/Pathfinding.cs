using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Pathfinding : MonoBehaviour
{
    //path visualization
    [SerializeField]
    private GameObject pathTile; //only assign this if you want to visualize search path
    private List<GameObject> pathList; 
    private Vector3 offset;

    void Start()
    {
        pathList = new List<GameObject>();
    }

    //call this function to find a path
    //builds a 2d grid from the grid positions and finds a path
    //from start to end
    public List<Vector3> findPath(Vector3 start, Vector3 end, Vector3 gridMinPos, Vector3 gridMaxPos)
    {
        destroyPath();
        gridMaxPos = roundVector(gridMaxPos);
        gridMinPos = roundVector(gridMinPos);
        start = roundVector(start);
        end = roundVector(end);
        //builds a 2d array for walkable tiles
        int[,] grid = buildGrid(gridMinPos,gridMaxPos);
        
        //offset from position in game to grid position and finds the path in grid by
        //adjusting starting and ending value by the offset
        offset = gridMaxPos - new Vector3(grid.GetLength(0)-1, grid.GetLength(1)-1, 0);
        List<Vector3> path = findGridPath(grid, start - offset, end - offset);

        //adds the offset back to the path to get the path positions in game
        for (int i = 0; i < path.Count; i++)
        {
            path[i] += offset;
        }
        return path;
    }
    //finds walkable tiles by raycasting down on each tile
    //and checking if they're collidable
    public int[,] buildGrid(Vector3 from, Vector3 to)
    {        
        //have to add +-1 to the size because it's to-from inclusive
        //ex: 6-4 should have a size of 3, because we have to include 4,5,6
        int xsize = (int)Mathf.Abs((to.x-from.x)+1);
        int ysize = (int)Mathf.Abs((to.y-from.y)+1);
        int[,] grid = new int[xsize,ysize];

        //puts a 0 in the grid if there is a collision, or 1 if there isnt
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < ysize; j++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(from.x+i,from.y+j), Vector2.zero); 
                if (hit && hit.collider.gameObject.tag != "Player" && hit.collider.gameObject != this.gameObject)
                {
                    grid[i,j] = 0;
                }
                else
                {
                    grid[i,j] = 1;
                }
            }
        }
        return grid;
    }
    public Vector3 roundVector(Vector3 vect)
    {
        return new Vector3(Mathf.Round(vect.x), Mathf.Round(vect.y), Mathf.Round(vect.z));
    }
    //node for storing x,y position in grid and the cost of the node
    //x,y from 0 to grid size
    public class Node
    {
        public int x;
        public int y;
        public int cost; //cost for heuristic function
        public int distance;
        public Node(int x, int y, int cost, int distance)
        {
            this.x = x;
            this.y = y;
            this.cost = cost;
            this.distance = distance;
        }
    }
    /* Uses a walking tile layer to build the grid
    //builds a grid of walkable tiles (1 = walkable, 0 = not walkable)
    public int[,] buildGrid(Vector3 from, Vector3 to)
    {        
        //have to add +-1 to the size because it's to-from inclusive
        //ex: 6-4 should have a size of 3, because we have to include 4,5,6
        int xsize = (int)Mathf.Abs((to.x-from.x)+1);
        int ysize = (int)Mathf.Abs((to.y-from.y)+1);
        int[,] grid = new int[xsize,ysize];
        int minx = (int)from.x;
        int miny = (int)from.y;

        //pulls the tiles based on the grid bounds
        BoundsInt bounds = new BoundsInt(minx, miny, 0, xsize, ysize, 1);
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);

        //puts a 0 in the grid if there isn't a tile, or 1 if there is a tile
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < ysize; j++)
            {
                if (tiles[i+j*xsize] == null)
                {
                    grid[i,j] = 0;
                }
                else
                {
                    grid[i,j] = 1;
                }
            }
        }
        return grid;
    }
    */
    bool comparator(Node n1, Node n2)
    {
        if (n1.cost <= n2.cost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //returns a list of paths to walk
    //uses A* pathfinding
    public List<Vector3> findGridPath(int[,] grid, Vector3 start, Vector3 end)
    {
        //grid length can be 0, so we just return an empty list
        if (grid.Length == 0)
        {
            return new List<Vector3>();
        }
        //nodes to vist next, should be a priority queue instead...
        //List<Node> nodes = new List<Node>(); 

        PriorityQueue<Node> nodes = new PriorityQueue<Node>(comparator);
        //stores the node used to backtrack the path
        Dictionary<Node, Node> backtrack = new Dictionary<Node, Node>();

        //initializes the start node based on start position and adds it to the list of nodes to explore
        Node startNode = new Node((int)Mathf.Abs(start.x),(int)Mathf.Abs(start.y),0, 0);
        nodes.Add(startNode);
        Node closestNode = startNode;
        while (nodes.Count != 0)
        {
            //gets the node with the lowest cost from the list
            //Node currentNode = popSmallestNode(nodes);
            Node currentNode = nodes.pop();
            visualizePath(new Vector3(currentNode.x+offset.x,currentNode.y+offset.y,offset.z));
            if (grid[currentNode.x,currentNode.y]== 1)
            {
                grid[currentNode.x,currentNode.y] = 2;
                //returns the backtracking if the current node as at the targeted node
                if (currentNode.x == (int)Mathf.Abs(end.x) && currentNode.y == (int)Mathf.Abs(end.y))
                {
                    return backtrackPath(backtrack, currentNode, startNode);
                }
                addAdjacentNodes(currentNode, nodes, grid, end, backtrack);
                if(distanceToNode(currentNode, end) < distanceToNode(closestNode, end))
                {
                    closestNode = currentNode;
                }
            }
        }
        return backtrackPath(backtrack,closestNode,startNode);
    }
    float distanceToNode(Node current, Vector3 target)
    {
    
        return Mathf.Sqrt(Mathf.Pow((current.x-target.x),2.0f) + Mathf.Pow((current.y-target.y),2.0f));
    }
    void visualizePath(Vector3 pos)
    {
        if (this.gameObject.GetComponent<MobController>().Visualize && pathTile != null)
        {
            pathList.Add(Instantiate(pathTile, pos, Quaternion.identity));
        }
    }
    void destroyPath()
    {
        if(pathList == null)
        {
            return;
        }
        for (int i = 0; i < pathList.Count; i++)
        {
            Destroy(pathList[i]);
        }
    }
    /*
    //returns the node with the lowest cost
    public Node popSmallestNode(List<Node> nodes)
    {
        Node smallestNode = nodes[0];
        int index = 0;
        for (int i = 1; i < nodes.Count; i++)
        {   
            if (nodes[i].cost < smallestNode.cost)
            {
                smallestNode = nodes[i];
                index = i;
            }
        }
        nodes.RemoveAt(index);
        return smallestNode;
    }
    */
    //adds all 8 adjacent positions to current node that haven't been
    //visited and within the grid to the list of nodes
    public void addAdjacentNodes(Node currentNode, PriorityQueue<Node> nodes, int[,] grid, Vector3 target, Dictionary<Node, Node> backtrack)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int newX = currentNode.x + i;
                int newY = currentNode.y + j;
                if (newX >= 0 && newX < grid.GetLength(0) && newY >= 0 && newY < grid.GetLength(1) && grid[newX,newY] == 1)
                {
                    //can only move diagonally if moving horizontally and vertically are possible
                    if((i*j != 0 && grid[currentNode.x,newY] == 1 && grid[newX, currentNode.y] == 1) || i*j == 0)
                    {
                        //cost of heuristic of new node and its distance to current node
                        int cost = heuristic(newX,newY,(int)target.x,(int)target.y) + currentNode.distance;
                        Node newNode = new Node(newX,newY,cost,currentNode.distance + 1);
                        nodes.Add(newNode);
                        backtrack.Add(newNode, currentNode);
                    }   
                }
            }
        }
    }
    //finds the path from start to end by backtracking
    public List<Vector3> backtrackPath(Dictionary<Node, Node> backtrack, Node finish, Node start)
    {
        List<Vector3> pathList = new List<Vector3>();
        Node current = finish;
        pathList.Add(new Vector3(current.x, current.y, 0));
        while(current != start)
        {
            Node newNode;
            backtrack.TryGetValue(current, out newNode);
            pathList.Add(new Vector3(newNode.x,newNode.y, 0));
            current = newNode;
        }
        return pathList;
        
    }
    //manhattan distance for A* heuristic
    public int heuristic(int x, int y, int xmax, int ymax)
    {
        return Mathf.Abs((ymax-y)) + Mathf.Abs((xmax-x));
    }
}
