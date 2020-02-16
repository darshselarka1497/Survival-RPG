using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Puzzle : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private GameObject gate;
    private GameObject[,] originalGrid;
    private GameObject[,] puzzleGrid;
    private BoundsInt bounds;
    private Dictionary<GameObject,int> pieces;
    private int blankPosition;
    [SerializeField]
    private GameObject puzzlePiece;
    private bool isMoving = false;
    private Vector3 slidePosition;
    private bool scrambling = true;

    // Start is called before the first frame update
    void Start()
    {
        bounds = tilemap.cellBounds;
        puzzleGrid = new GameObject[bounds.size.x/3,bounds.size.x/3];
        originalGrid = new GameObject[bounds.size.x/3,bounds.size.x/3];
        pieces = new Dictionary<GameObject,int>();
        createPuzzleGrid();
        scramblePieces();
    }

    // Update is called once per frame
    void Update()
    {
        Item equipped = Toolbar.Instance.getActiveItem();
        //checks for left click and the puzzle controller item
        if (Input.GetMouseButtonDown(0) && equipped != null && equipped.Id == 98)
        {
            Vector3 pos = Player.Instance.transform.position;
            if (pos.x >= bounds.xMin && pos.x < bounds.xMax && pos.y >= bounds.yMin && pos.y < bounds.yMax)
            {
                int xIndex = (int)(pos.x-bounds.xMin)/3;
                int yIndex = (int)(bounds.yMax-1-pos.y)/3;
                switchPieces(puzzleGrid[xIndex,yIndex],true);
            }
        }
    }
    //creates the grid of puzzle gameobjects and their sprites
    void createPuzzleGrid()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("puzzle");
        int pieceNum = 0;

        for (int i = 0; i < puzzleGrid.GetLength(0); i++)
        {
            for (int j = 0; j < puzzleGrid.GetLength(1); j++)
            {
                if (i == puzzleGrid.GetLength(0) - 1 && j == puzzleGrid.GetLength(1) - 1)
                {
                    blankPosition = pieceNum;
                    return;
                }
                //instantiates a piece at the position and attaches a sprite
                Vector3 pos = new Vector3(bounds.xMin + 1 + 3*j, bounds.yMax - 2 - 3*i, 0);
                GameObject piece = Instantiate(puzzlePiece, pos, Quaternion.identity, transform);
                piece.GetComponent<SpriteRenderer>().sprite = sprites[pieceNum];

                //set grid and dictionary values
                puzzleGrid[j,i] = piece;
                originalGrid[j,i] = piece;
                pieces.Add(piece,pieceNum);
                pieceNum++;

            }
        }

    }

    //checks the puzzle grid against the original grid to see if they're the same
    bool isComplete()
    {
        for (int i = 0; i < puzzleGrid.GetLength(0); i++)
        {
            for (int j = 0; j < puzzleGrid.GetLength(1); j++)
            {
                if (puzzleGrid[j,i] != originalGrid[j,i])
                {
                    return false;
                }
            }
        }
        return true;
    }

    //switches the piece position with the blank piece
    void switchPieces(GameObject piece, bool showAnim)
    {
        if (piece == null || isMoving)
        {
            return;
        }
        int position;
        pieces.TryGetValue(piece, out position);

        //gets the index of the pieces
        int xIndex = position % 5;
        int yIndex = position / 5;
        int xBlank = blankPosition % 5;
        int yBlank = blankPosition / 5;

        if (Mathf.Abs(xBlank-xIndex) + Mathf.Abs(yBlank-yIndex) == 1)
        {
            pieces.Remove(piece);
            pieces.Add(piece, blankPosition);
            blankPosition = position;
            Vector3 pos = new Vector3(bounds.xMin + 1 + 3*xBlank, bounds.yMax - 2 - 3*yBlank, 0);

            //toggles animation 
            if (showAnim)
            {
                StartCoroutine(slidePiece(piece, pos));
            }
            else
            {
                piece.transform.position = pos;
            }
            
            //updates the grid positions
            puzzleGrid[xBlank, yBlank] = piece;
            puzzleGrid[xIndex, yIndex] = null;
        }
        //opens the gate and turns off this script if the puzzle is completed
        if (!scrambling && isComplete())
        {
            gate.SetActive(false);
            this.enabled = false;
        }
    }
    //sliding piece animation
    IEnumerator slidePiece(GameObject piece, Vector3 newPos)
    {
        
        isMoving = true;
        while(piece.transform.position != newPos)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position,newPos,.1f);
            yield return null;
        }
        isMoving = false;
    }
    //scrambles the pieces by repeatedly calling switch piece
    void scramblePieces()
    {
        int scrambleCount = 0;
        int[] moves = new int[]{-5,-1,1,5};
        while (scrambleCount < 200)
        {
            int moveIndex = (int)Random.Range(0,4);
            int piecePos = blankPosition + moves[moveIndex];
            if (piecePos >= 0 && piecePos <= pieces.Count)
            {
                switchPieces(puzzleGrid[piecePos%5, piecePos/5], false);
            } 
            scrambleCount++;
        }
        scrambling = false;
    }


}
