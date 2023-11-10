using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
/*
 * Is used to ensure game is not playable in the check process, 
 * namely, swapping one after another without detection process.
 * 
 * Also used for stop the game after movement is finished.
 */
{
    play,
    pause,
    finish,
}


public class Board : MonoBehaviour
{

    [Header("Level Stuff")]
    public World world;
    public int level;

    [Header("Board Dimensions")]
    public int size; // size of the board

    [Header("Level Information")]
    private int maxMoves;
    private float machineClicked;
    private int cycles;

    [Header("Prefabs")]
    public GameObject[] cells;
    private Cell[,] allCells;
    public GameObject tilePrefab; // TilePrefab will be used to fill the background of the tiles of the board.
    public GameObject[,] allTiles;

    // Max number that a cell can take.
    private int maxF = 4;
    // Int representation of the board.
    public int[,] boardInt;

    public List<List<int>> solveStack = new List<List<int>>();

    public GameState currentState = GameState.play;

    private Color initialTileColor;



private void Awake()
    {
        initialTileColor = tilePrefab.GetComponent<SpriteRenderer>().color;

        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            //PlayerPrefs.SetInt("CurrentLevel", 0);
            level = PlayerPrefs.GetInt("CurrentLevel");
        }
         
        Debug.Log(level);

        // If there is a world object for board, then perform required initializations
        if (world != null)
        {
            if (world.levels[level] != null)
            {
                size = world.levels[level].size;
                maxMoves = (int)world.levels[level].maxMoves;
                machineClicked = world.levels[level].machineClicked;
                cycles = world.levels[level].nodeCycle;
            }
        }
        else
        {
            Debug.Log("World could not be found!!!!");
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[size, size];
        allCells = new Cell[size, size];
        boardInt = new int[size, size];
        //Setup(); // Sets up the board
    }

    // Setup process for setting up the board
    public void Setup()
    {
        // Setup int representation of the board.
        for (int k = 0; k < machineClicked; k++)
        {
            int m = Mathf.FloorToInt(size * Random.value);
            int n = Mathf.FloorToInt(size * Random.value);

            // implement cycle logic
            if (cycles == 0)
            {
                if (boardInt[m,n] != 4)
                {
                    incrementGroup(m, n);
                }
                else
                {
                    k--;
                }
            }
            //else if (cycles == 1)
            //{

            //}
            else
            {
                incrementGroup(m, n);
            }
            

            solveStack.Add(new List<int> { m, n });
            //Debug.Log("-------------");
            //Debug.Log(maxMoves);
            //Debug.Log("-------------");
            //Debug.Log(solveStack[k][0] + "..." + solveStack[k][1]);
        }

        //boardPrinter();

        // Setup the actual board object.
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector2 Position = new Vector2(i, j);

                GameObject backgroundTile = Instantiate(tilePrefab, Position, Quaternion.identity);
                
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";
                backgroundTile.GetComponent<SpriteRenderer>().sortingOrder = -1;
                allTiles[i, j] = backgroundTile;


                GameObject cellObject = Instantiate(cells[boardInt[i, j]], Position, Quaternion.identity);
                cellObject.transform.parent = backgroundTile.transform;
                cellObject.name = "(" + i + "," + j + ")";

                Cell cell = cellObject.GetComponent<Cell>();
                cell.boardPosition = new Vector2Int(i, j);
                //cell.transform.parent = cellObject.transform;
                allCells[i, j] = cell;

                
            }
        }

        //pulseRequired();
        pulsePlus(solveStack[solveStack.Count - 1][0], solveStack[solveStack.Count - 1][1]);
    }


    //private void colorTile(int i, int j, bool power)
    //{
    //    Color powColor = new Color(1f, 0.1f, 0.2f, 0.6f);
    //    Color lowColor = new Color(1f, 0.1f, 0.2f, 0.4f);
    //    if (power)
    //    {
    //        allTiles[i, j].GetComponent<SpriteRenderer>().color = powColor;
    //    }
    //    else
    //    {
    //        allTiles[i, j].GetComponent<SpriteRenderer>().color = lowColor;
    //    }
    //}

    //private void colorPlus(int i, int j)
    //{
    //    colorTile(i,j,true);
    //    if (i > 0) colorTile(i - 1, j, false);
    //    if (j > 0) colorTile(i, j - 1, false);
    //    if (i + 1 < size) colorTile(i + 1, j, false);
    //    if (j + 1 < size) colorTile(i, j + 1, false);
    //}


    private Color GetRandomColor()
    {
        // Define hue ranges that exclude green to blue spectrum
        float[] allowedHueRanges = new float[] { 0.0f, 0.16f, 0.67f, 1.0f }; // Red to Yellow to Red hues

        // Randomly pick a range
        int rangeIndex = UnityEngine.Random.Range(0, allowedHueRanges.Length / 2);
        float hue = UnityEngine.Random.Range(allowedHueRanges[rangeIndex * 2], allowedHueRanges[rangeIndex * 2 + 1]);

        float saturation = UnityEngine.Random.Range(0.7f, 1.0f); // High saturation for vivid color
        float value = UnityEngine.Random.Range(0.7f, 1.0f); // High value for brightness

        return Color.HSVToRGB(hue, saturation, value);
    }


    // Gradient frame
    private void frameTile(int i, int j, Color clr1, Color clr2)
    {
        GameObject bgTile = allTiles[i, j];

        // First, create a new GameObject for the frame
        GameObject frameObject = new GameObject("Frame");
        LineRenderer lineRenderer = frameObject.AddComponent<LineRenderer>();

        // Set the LineRenderer properties
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f; // Width of the frame
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 5; // A square frame needs 5 points (4 corners + closing the square)

        // Create a gradient for the line renderer
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(clr1, 0.0f),
            new GradientColorKey(clr2, 1.0f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 1.0f)
            }
        );
        lineRenderer.colorGradient = gradient;

        // Set the material to a solid color with the gradient affecting the color
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the corner positions of the frame relative to the parent tile
        float halfSize = bgTile.GetComponent<SpriteRenderer>().bounds.size.x / 2; // Assuming x and y size are equal
        Vector3[] points = new Vector3[5] {
        new Vector3(-halfSize, -halfSize, 0), // Bottom left
        new Vector3(-halfSize, halfSize, 0),  // Top left
        new Vector3(halfSize, halfSize, 0),   // Top right
        new Vector3(halfSize, -halfSize, 0),  // Bottom right
        new Vector3(-halfSize-(halfSize/9), -halfSize, 0)  // Back to bottom left to close the frame
    };

        lineRenderer.SetPositions(points);

        // Now attach the frame to the tile
        frameObject.transform.SetParent(bgTile.transform, false);
        frameObject.transform.localPosition = Vector3.zero; // Center the frame on the tile
    }


    private void framePlus(int i, int j)
    {
        Color clr1 = GetRandomColor();
        Color clr2 = GetRandomColor();
        frameTile(i, j, clr1, clr2);
        if (i > 0) frameTile(i - 1, j, clr1, clr2);
        if (j > 0) frameTile(i, j - 1, clr1, clr2);
        if (i + 1 < size) frameTile(i + 1, j, clr1, clr2);
        if (j + 1 < size) frameTile(i, j + 1, clr1, clr2);
    }


    public void DeleteFrame(int i, int j)
    {
        GameObject bgTile = allTiles[i,j];
        Transform frameTransform = bgTile.transform.Find("Frame");
        if (frameTransform != null)
        {
            Destroy(frameTransform.gameObject);
        }
    }


    private void increment(int i, int j)
    {
        boardInt[i,j]++; if (boardInt[i,j] > maxF) boardInt[i,j] = 0;
    }


    private void incrementGroup(int i, int j)
    {
        increment(i, j);
        if (i > 0) increment(i - 1, j);
        if (j > 0) increment(i, j - 1);
        if (i + 1 < size) increment(i + 1, j);
        if (j + 1 < size) increment(i, j + 1);
    }


    public void CellClicked(Vector2Int position)
    {
        ChangeSprite(position);

        // Change sprite for left adjacent cell
        if (position.x > 0) ChangeSprite(new Vector2Int(position.x - 1, position.y));

        // Change sprite for bottom adjacent cell
        if (position.y > 0) ChangeSprite(new Vector2Int(position.x, position.y - 1));

        // Change sprite for right adjacent cell
        if (position.x + 1 < size) ChangeSprite(new Vector2Int(position.x + 1, position.y));

        // Change sprite for top adjacent cell
        if (position.y + 1 < size) ChangeSprite(new Vector2Int(position.x, position.y + 1));
    }


    private void ChangeSprite(Vector2Int position)
    {
        Cell cell = allCells[position.x, position.y];
        cell.GetComponent<SpriteRenderer>().sprite = cell.sprites[boardInt[position.x,position.y]];

        StartCoroutine(cell.AnimateCell());
    }


    public void stopAllPulse()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                allCells[i, j].pulsing = false;
                allCells[i, j].transform.localScale = transform.localScale;
                allTiles[i, j].GetComponent<SpriteRenderer>().color = initialTileColor;
                DeleteFrame(i,j);
            }
        }
    }


    private int count = 1;


    public void pulsePlus(int i, int j)
    {
        stopAllPulse();
        if (level <= 10)
        {
            pulseRequired(i, j, true);
            if (i > 0) pulseRequired(i - 1, j, false);
            if (j > 0) pulseRequired(i, j - 1, false);
            if (i + 1 < size) pulseRequired(i + 1, j, false);
            if (j + 1 < size) pulseRequired(i, j + 1, false);
        }
        else
        {
            if (count <= 3)
            {
                pulseRequired(i, j, true);
                if (i > 0) pulseRequired(i - 1, j, false);
                if (j > 0) pulseRequired(i, j - 1, false);
                if (i + 1 < size) pulseRequired(i + 1, j, false);
                if (j + 1 < size) pulseRequired(i, j + 1, false);
            }
            count++;
        }
    }


    //solveStack[solveStack.Count - 1][0], solveStack[solveStack.Count - 1][1]

    public void pulseRequired(int i, int j, bool pow)
    {
        //stopAllPulse();
        if (allCells[i,j] != null)
        {
            //colorPlus(solveStack[solveStack.Count - 1][0], solveStack[solveStack.Count - 1][1]);
            //framePlus(solveStack[solveStack.Count - 1][0], solveStack[solveStack.Count - 1][1]);
            if (pow)
            {
                Color clr1 = GetRandomColor();
                Color clr2 = GetRandomColor();
                frameTile(i, j, clr1, clr2);
            }
            Cell cell = allCells[i,j];
            cell.pulseCell();
        }
    }


    public void Kill()
    {
        Destroy(this);
    }
}









//private void frameTile(int i, int j, bool drawTop, bool drawBottom, bool drawLeft, bool drawRight)
//{
//    GameObject bgTile = allTiles[i, j];

//    // Create a new GameObject for the frame
//    GameObject frameObject = new GameObject("Frame");
//    LineRenderer lineRenderer = frameObject.AddComponent<LineRenderer>();

//    // Set the LineRenderer properties
//    lineRenderer.useWorldSpace = false;
//    lineRenderer.startWidth = 0.1f;
//    lineRenderer.endWidth = 0.1f;
//    lineRenderer.positionCount = 5;

//    // Set up a gradient with random color keys
//    Gradient gradient = new Gradient();
//    gradient.SetKeys(
//        new GradientColorKey[] {
//        new GradientColorKey(GetRandomColor(), 0.0f),
//        new GradientColorKey(GetRandomColor(), 0.5f),
//        new GradientColorKey(GetRandomColor(), 1.0f)
//        },
//        new GradientAlphaKey[] {
//        new GradientAlphaKey(1.0f, 0.0f),
//        new GradientAlphaKey(1.0f, 0.5f),
//        new GradientAlphaKey(1.0f, 1.0f)
//        }
//    );
//    lineRenderer.colorGradient = gradient;

//    // Assign the material with the Sprites/Default shader
//    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

//    // Calculate half the size of the tile
//    float halfSize = bgTile.GetComponent<SpriteRenderer>().bounds.size.x / 2;

//    // Define the corner positions of the frame
//    //    Vector3[] points = new Vector3[5] {
//    //    new Vector3(-halfSize, -halfSize, 0),
//    //    new Vector3(-halfSize, halfSize, 0),
//    //    new Vector3(halfSize, halfSize, 0),
//    //    new Vector3(halfSize, -halfSize, 0),
//    //    new Vector3(-halfSize-(halfSize/9), -halfSize, 0)
//    //};

//    List<Vector3> points = new List<Vector3>();

//    // Calculate the positions for the frame based on the desired sides
//    Vector3 bottomLeft = new Vector3(-halfSize, -halfSize, 0);
//    Vector3 topLeft = new Vector3(-halfSize, halfSize, 0);
//    Vector3 topRight = new Vector3(halfSize, halfSize, 0);
//    Vector3 bottomRight = new Vector3(halfSize, -halfSize, 0);

//    // Add the points based on the flags provided, without closing the loop
//    if (drawLeft)
//    {
//        points.Add(bottomLeft);
//        points.Add(topLeft);
//    }
//    if (drawTop)
//    {
//        if (!drawLeft) points.Add(topLeft); // Add the top left corner only if the left line hasn't been drawn
//        points.Add(topRight);
//    }
//    if (drawRight)
//    {
//        if (!drawTop) points.Add(topRight); // Add the top right corner only if the top line hasn't been drawn
//        points.Add(bottomRight);
//    }
//    if (drawBottom)
//    {
//        if (!drawRight) points.Add(bottomRight); // Add the bottom right corner only if the right line hasn't been drawn
//        points.Add(bottomLeft); // Complete the bottom line to the starting point of left line if it was drawn
//    }

//    // Assign points to the LineRenderer if there are any lines to draw
//    if (points.Count > 1) // Ensure there are at least two points to draw a line
//    {
//        lineRenderer.positionCount = points.Count;
//        lineRenderer.SetPositions(points.ToArray());
//    }
//    else // If no sides are drawn, there's no need for a LineRenderer component
//    {
//        Destroy(frameObject); // Remove the frame object as it's not needed
//    }


//    //points.Add(new Vector3(-halfSize, -halfSize, 0));

//    lineRenderer.positionCount = points.Count;
//    lineRenderer.SetPositions(points.ToArray());


//    // Attach the frame to the tile
//    frameObject.transform.SetParent(bgTile.transform, false);
//    frameObject.transform.localPosition = Vector3.zero;
//}


//private Color GetRandomColor()
//{
//    // Randomize hue, but keep high saturation and value to ensure the color is vibrant and eye-catching
//    float hue = UnityEngine.Random.Range(0.0f, 1.0f);
//    float saturation = UnityEngine.Random.Range(0.7f, 1.0f); // Keep saturation above 70%
//    float value = UnityEngine.Random.Range(0.7f, 1.0f); // Keep value above 70%
//    return Color.HSVToRGB(hue, saturation, value);
//}


// Rounded cornered frames.
//private void frameTile(int i, int j)
//{
//    GameObject bgTile = allTiles[i, j];

//    // First, create a new GameObject for the frame
//    GameObject frameObject = new GameObject("Frame");
//    LineRenderer lineRenderer = frameObject.AddComponent<LineRenderer>();

//    // Set the LineRenderer properties
//    lineRenderer.useWorldSpace = false;
//    lineRenderer.startWidth = 0.08f; // Width of the frame
//    lineRenderer.endWidth = 0.1f;

//    // Increase the number of points for a smoother rounded corner
//    int segmentsPerCorner = 20; // Try increasing this for a smoother curve
//    lineRenderer.positionCount = segmentsPerCorner * 4 + 1;

//    // Set the material to a solid color
//    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//    lineRenderer.material.color = new Color(0.7f, 0.1f, 0.2f, 0.95f); // Frame color

//    // Set the corner positions of the frame relative to the parent tile
//    float halfSize = bgTile.GetComponent<SpriteRenderer>().size.x / 2;
//    float cornerRadius = 0.1f; // Radius of the rounded corner

//    // Buffer to hold points
//    Vector3[] points = new Vector3[lineRenderer.positionCount];

//    // Define a helper method to create rounded corners
//    void CreateRoundedCorner(int startIndex, Vector2 center, float startAngle, float radius)
//    {
//        for (int i = 0; i < segmentsPerCorner; i++)
//        {
//            float angle = startAngle + (i / (float)(segmentsPerCorner - 1)) * 90f;
//            points[startIndex + i] = new Vector3(
//                center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
//                center.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
//                0f
//            );
//        }
//    }

//    // Create each rounded corner
//    CreateRoundedCorner(0 * segmentsPerCorner, new Vector2(-halfSize + cornerRadius, -halfSize + cornerRadius), 180, cornerRadius);
//    CreateRoundedCorner(1 * segmentsPerCorner, new Vector2(-halfSize + cornerRadius, halfSize - cornerRadius), 270, cornerRadius);
//    CreateRoundedCorner(2 * segmentsPerCorner, new Vector2(halfSize - cornerRadius, halfSize - cornerRadius), 0, cornerRadius);
//    CreateRoundedCorner(3 * segmentsPerCorner, new Vector2(halfSize - cornerRadius, -halfSize + cornerRadius), 90, cornerRadius);

//    // Close the loop
//    points[points.Length - 1] = points[0];

//    // Apply the points to the line renderer
//    lineRenderer.SetPositions(points);

//    // Now attach the frame to the tile
//    frameObject.transform.parent = bgTile.transform;
//    frameObject.transform.localPosition = Vector3.zero; // Center the frame on the tile
//}


//private void framePlus(int i, int j)
//{
//    // Center cell
//    frameTile(i, j, true, true, true, true);

//    // Top cell
//    if (i > 0) frameTile(i - 1, j, true, false, true, true); // Draw only the top edge

//    // Bottom cell
//    if (i < size - 1) frameTile(i + 1, j, false, true, true, true); // Draw only the bottom edge

//    // Left cell
//    if (j > 0) frameTile(i, j - 1, true, true, true, false); // Draw only the left edge

//    // Right cell
//    if (j < size - 1) frameTile(i, j + 1, true, true, false, true); // Draw only the right edge
//}


//private void framePlus(int i, int j)
//{
//    frameTile(i, j, true, true, true, true);
//    if (i > 0) frameTile(i - 1, j, false, true, true, true);
//    if (j > 0) frameTile(i, j - 1, true, true, true, false);
//    if (i + 1 < size) frameTile(i + 1, j, true, false, true, true);
//    if (j + 1 < size) frameTile(i, j + 1, true, true, false, true);
//}



//Outer sides
//private void framePlus(int i, int j)
//{
//    // Draw all sides for the center tile
//    frameTile(i, j, false, false, false, false);

//    // Draw only adjacent sides for each direction. Note that since 'i' is horizontal, 'i-1' and 'i+1' adjust the left and right positions, respectively.
//    // Draw top side for the tile above
//    if (j + 1 < size) frameTile(i, j + 1, true, false, true, true); // Only top side if it's the upper part of the plus

//    // Draw bottom side for the tile below
//    if (j > 0) frameTile(i, j - 1, false, true, true, true); // Only bottom side if it's the lower part of the plus

//    // Draw right side for the tile to the right
//    if (i + 1 < size) frameTile(i + 1, j, true, true, false, true); // Only right side if it's the right part of the plus

//    // Draw left side for the tile to the left
//    if (i > 0) frameTile(i - 1, j, true, true, true, false); // Only left side if it's the left part of the plus
//}



//// Sharp cornered frames
//private void frameTile(int i, int j)
//{
//    GameObject bgTile = allTiles[i, j];

//    // First, create a new GameObject for the frame
//    GameObject frameObject = new GameObject("Frame");
//    LineRenderer lineRenderer = frameObject.AddComponent<LineRenderer>();

//    // Set the LineRenderer properties
//    lineRenderer.useWorldSpace = false;
//    lineRenderer.startWidth = 0.08f; // Width of the frame
//    lineRenderer.endWidth = 0.1f;
//    lineRenderer.positionCount = 5; // A square frame needs 5 points (4 corners + closing the square)

//    // Set the material to a solid color
//    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
//    lineRenderer.material.color = new Color(0.7f, 0.1f, 0.2f, 0.95f); // Frame color

//    // Set the corner positions of the frame relative to the parent tile
//    float halfSize = (bgTile.GetComponent<SpriteRenderer>().size[1] / 2); // Assuming you know the size of your tiles
//    Vector3[] points = new Vector3[5]
//    {
//                new Vector3(-halfSize, -halfSize, 0), // Bottom left
//                new Vector3(-halfSize, halfSize, 0),  // Top left
//                new Vector3(halfSize, halfSize, 0),   // Top right
//                new Vector3(halfSize, -halfSize, 0),  // Bottom right
//                new Vector3(-halfSize, -halfSize, 0)  // Back to bottom left to close the frame
//    };

//    lineRenderer.SetPositions(points);

//    // Now attach the frame to the tile
//    frameObject.transform.parent = bgTile.transform;
//    frameObject.transform.localPosition = Vector3.zero; // Center the frame on the tile
//}

// soft cornered frames with cool effects
//private void frameTile(int i, int j)
//{
//    GameObject bgTile = allTiles[i, j];

//    // First, create a new GameObject for the frame
//    GameObject frameObject = new GameObject("Frame");
//    LineRenderer lineRenderer = frameObject.AddComponent<LineRenderer>();

//    // Set the LineRenderer properties
//    lineRenderer.useWorldSpace = false;
//    lineRenderer.startWidth = 0.1f; // Width of the frame
//    lineRenderer.endWidth = 0.1f;
//    lineRenderer.positionCount = 5; // A square frame needs 5 points (4 corners + closing the square)

//    // Create a gradient for the line renderer
//    Gradient gradient = new Gradient();
//    gradient.SetKeys(
//        new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
//        new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
//    );
//    lineRenderer.colorGradient = gradient;

//    // Use a texture for the line renderer
//    lineRenderer.material = new Material(Shader.Find("Unlit/Texture"));
//    lineRenderer.material.mainTexture = Resources.Load<Texture2D>("Textures/LineTexture"); // Replace with your texture path

//    // Set the corner positions of the frame relative to the parent tile
//    float halfSize = (bgTile.GetComponent<SpriteRenderer>().size.x / 2); // Assuming x and y size are equal
//    Vector3[] points = new Vector3[5]
//    {
//            new Vector3(-halfSize, -halfSize, 0), // Bottom left
//            new Vector3(-halfSize, halfSize, 0),  // Top left
//            new Vector3(halfSize, halfSize, 0),   // Top right
//            new Vector3(halfSize, -halfSize, 0),  // Bottom right
//            new Vector3(-halfSize, -halfSize, 0)  // Back to bottom left to close the frame
//    };

//    lineRenderer.SetPositions(points);

//    // Now attach the frame to the tile
//    frameObject.transform.parent = bgTile.transform;
//    frameObject.transform.localPosition = Vector3.zero; // Center the frame on the tile
//}