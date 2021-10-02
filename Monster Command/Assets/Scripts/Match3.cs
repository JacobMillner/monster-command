using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;

    [Header("Prefabs")]
    public GameObject nodePiece;

    int width = 9;
    int height = 14;
    Node[,] board;

    System.Random random;

    void StartGame()
    {
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());

        Debug.Log("Initialize Board!");
        InitializeBoard();
        Debug.Log("Verify Board!");
        VerifyBoard();
        Debug.Log("Instantiate Board");
        InstantiateBoard();
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int val = board[x,y].value;
                Debug.Log("Board val: " + val);
                if (val <= 0) continue;

                Debug.Log("here we go!");
                GameObject p = Instantiate(nodePiece, gameBoard);
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
            }
        }
    }

    void InitializeBoard()
    {
         board = new Node[width, height];
         for(int y = 0; y < height; y++)
         {
             for(int x = 0; x < width; x++)
             {
                 // generate our piece
                 // check the boardlayout for holes
                 // else generate a new random piece
                 int newPiece;
                 if (boardLayout.rows[y].row[x]) {
                     newPiece = -1;
                 } else {
                     newPiece = fillPiece();
                 }
                 board[x, y] = new Node(-1, new Point(x,y));
             }
         }
    }

    void VerifyBoard()
    {
        List<int> remove;

        // check that there are no matches on start
         for(int x = 0; x < width; x++)
         {
             for(int y = 0; y < height; y++)
             {
                Point p = new Point(x, y);
                int val = getValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();
                while(isConnected(p, true).Count > 0)
                {
                    val = getValueAtPoint(p);
                    if (!remove.Contains(val))
                    {
                        remove.Add(val);
                    }
                    setValueAtPoint(p, newValue(ref remove));
                }
             }
         }
    }

    List<Point> isConnected(Point point, bool main)
    {
        List<Point> connected = new List<Point>();
        // original points value
        int val = getValueAtPoint(point);
        Point[] directions =
        {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };

        // look for matches of 2 or more in each direction
        foreach(Point dir in directions)
        {
            List<Point> line = new List<Point>();

            // check points in a straight line (3 max)
            int same = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.Add(point, Point.Multiply(dir, i));
                if(getValueAtPoint(check) == val)
                {
                    // add to our list of matching points
                    line.Add(check);
                    same++;
                }
            }

            // if we get to 2 we know we have 3 matching in a line
            if (same > 1)
            {
                // add points to main list of matches
                addPoints(ref connected, line);
            }
        }

        // check if we are in the middle piece in a match of 3
        for (int i= 0; i < 2; i++)
        {  
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.Add(point, directions[i]), Point.Add(point, directions[i+2]) };
            // check both sides of piece if matching
            foreach (Point next in check)
            {
                if(getValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            // if we get to 2 we know we have 3 matching in a line
            if (same > 1)
            {
                // add points to main list of matches
                addPoints(ref connected, line);
            }
        }

        // check for 2x2 match
        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4)
            {
                next -= 4;
            }

            Point[] check = { Point.Add(point, directions[i]), Point.Add(point, directions[next]), Point.Add(point, Point.Add(directions[i], directions[next])) };
            foreach (Point checkedPoint in check)
            {
                if(getValueAtPoint(checkedPoint) == val)
                {
                    square.Add(checkedPoint);
                    same++;
                }
            }

            if (same > 2)
            {
                addPoints(ref connected, square);
            }
        }

        // checks for other matches along current match
        // (compond matches)
        if (main) {
            for(int i = 0; i < connected.Count; i++) {
                addPoints(ref connected, isConnected(connected[i], false));
            }
        }

        if (connected.Count > 0)
        {
            connected.Add(point);
        }

        return connected;
    }

    void addPoints(ref List<Point> points, List<Point> pToAdd)
    {
        // check each point to make sure they are not already added
        foreach (Point p in pToAdd)
        {
            bool isAdded = true;
            for (int i = 0; i < pToAdd.Count; i ++)
            {
                if(pToAdd[i].Equals(p))
                {
                    // point is already in list, skip adding it
                    isAdded = false;
                    break;
                }
            }

            if (isAdded)
            {
                points.Add(p);
            }
        }
    }

    int getValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height)
        {
            return -1;
        }
        return board[p.x, p.y].value;
    }

    int newValue(ref List<int> remove)
    {
        // generate a new random value out of the list of available pieces
        List<int> available = new List<int>();
        for (int i = 0 ; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (int i in remove)
        {
            available.Remove(i);
        }

        if (available.Count <= 0) return 0;

        return available[random.Next(0, available.Count)];
    }

    void setValueAtPoint(Point p, int v) {
        board[p.x, p.y].value = v;
    }

    int fillPiece()
    {
        int val = 1;
        // grab a random piece, bump by 1 so it's not a blank
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }
    void Start()
    {
        Debug.Log("start game!");
        StartGame();
    }

    void Update()
    {
        
    }

    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz12345670!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }
}

[System.Serializable]
public class Node
{
    // TODO: convert to enum
    // 0 = blank
    // 1 = cube
    // 2 = sphere
    // 3 = cylinder
    // 4 = pyramid
    // 5 = diamond
    // -1 = hole
    public int value; 
    public Point index;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }
}
