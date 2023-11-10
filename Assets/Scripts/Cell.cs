using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class Cell : MonoBehaviour
{

    private Vector2 firstTouch;
    private Vector2 lastTouch;
    private float swipeResist = 0.4f; // To check its a tap instead of swipe.

    private int row;
    private int column;

    private Board board;
    private int maxF = 4;

    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public Vector2Int boardPosition;

    private EndGameManager endGameManager;

    private Vector3 initialScale; // Initial scale of the object.


    //private SpriteRenderer bgTileSpriteRenderer;





    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        endGameManager = FindObjectOfType<EndGameManager>();
        board = FindObjectOfType<Board>();
    }

    // Start is called before the first frame update
    void Start()
    {
        row = (int)transform.position.x;
        column = (int)transform.position.y;
        boardPosition = new Vector2Int(row, column);
        initialScale = transform.localScale;
    }


    // Obtains first touch position
    private void OnMouseDown()
    {
        firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    // Obtains last touch position
    private void OnMouseUp()
    {
        lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (boardPosition.x == board.solveStack[board.solveStack.Count - 1][0] && boardPosition.y == board.solveStack[board.solveStack.Count - 1][1])
        {
            pulsing = false;
            StopCoroutine(Pulse());
            transform.localScale = initialScale;
            transform.parent.localScale = initialScale;
        }
        CalculateTap();
    }


    private void CalculateTap()
    {
        // Its a tap.
        if (Mathf.Abs(lastTouch.y - firstTouch.y) < swipeResist && Mathf.Abs(lastTouch.x - firstTouch.x) < swipeResist)
        {
            if(board.currentState == GameState.play)
            {
                Click();
            }
        }
    }


    private void Click()
    {
        transform.localScale = initialScale;
        pulsing = false;

        decrementGroup(row,column);
        board.CellClicked(boardPosition);
        endGameManager.DecrementMove();

        if (checkBoard()) {
            if (endGameManager != null)
            {
                endGameManager.winGame();
            }
            Debug.Log(checkBoard());
        }

        refreshStack();
        board.pulsePlus(board.solveStack[board.solveStack.Count - 1][0], board.solveStack[board.solveStack.Count - 1][1]);

        print(board.solveStack[board.solveStack.Count - 1][0] + ":" + board.solveStack[board.solveStack.Count - 1][1] + ",,,");
    }


    private void refreshStack()
    {
        // True click
        //if (boardPosition.x == board.solveStack[FindSublist(board.solveStack, new List<int> { boardPosition.x, boardPosition.y })][0] && boardPosition.y == board.solveStack[FindSublist(board.solveStack, new List<int> { boardPosition.x, boardPosition.y })][1])
        if(FindSublist(board.solveStack, new List<int> { boardPosition.x, boardPosition.y }) != -1)
        {
            board.solveStack.RemoveAt(FindSublist(board.solveStack, new List<int> { boardPosition.x, boardPosition.y })); // POP
        }
        // False click
        else
        {
            board.solveStack.Add(new List<int> { boardPosition.x, boardPosition.y }); // PUSH
            board.solveStack.Add(new List<int> { boardPosition.x, boardPosition.y }); // PUSH
            board.solveStack.Add(new List<int> { boardPosition.x, boardPosition.y }); // PUSH
            board.solveStack.Add(new List<int> { boardPosition.x, boardPosition.y }); // PUSH

        }
    }


    public static int FindSublist(List<List<int>> nestedList, List<int> targetSublist)
    {
        for (int i = 0; i < nestedList.Count; i++)
        {
            if (nestedList[i].SequenceEqual(targetSublist))
            {
                return i;  // Return the index of the matching sub-list.
            }
        }
        return -1;
    }


    public float pulseSpeed = 0.7f; // Speed of the pulse animation.
    public float pulseAmount = 0.2f; // How much the object will scale during the pulse.
    public bool pulsing = false;


    public void pulseCell()
    {
        initialScale = transform.localScale; // Store the original scale.
        pulsing = true;
        StartCoroutine(Pulse());
    }


    IEnumerator Pulse()
    {
        while (pulsing)
        {
            // Scale up.
            float time = 0;
            while (time <= pulseSpeed)
            {
                time += Time.deltaTime * pulseSpeed;
                transform.localScale = initialScale + (Vector3.one * pulseAmount * Mathf.Sin(time * Mathf.PI));
                transform.parent.localScale = initialScale + (Vector3.one * pulseAmount * Mathf.Sin(time * Mathf.PI)); 
                yield return null; // Wait for the next frame.
            }

            if (!pulsing)
            {
                transform.localScale = initialScale;
                break;
            }

            // Scale down.
            time = 0;
            while (time <= pulseSpeed)
            {
                time += Time.deltaTime * pulseSpeed;
                transform.localScale = initialScale + (Vector3.one * pulseAmount * Mathf.Sin(time * Mathf.PI));
                transform.parent.localScale = initialScale + (Vector3.one * pulseAmount * Mathf.Sin(time * Mathf.PI));
                yield return null; // Wait for the next frame.
            }
        }
    }


    private void decrement(int i, int j)
    {
        board.boardInt[i, j]--; if (board.boardInt[i, j] < 0) board.boardInt[i, j] = maxF;
    }


    private void decrementGroup(int i, int j)
    {
        decrement(i, j);
        if (i > 0) decrement(i - 1, j);
        if (j > 0) decrement(i, j - 1);
        if (i + 1 < board.size) decrement(i + 1, j);
        if (j + 1 < board.size) decrement(i, j + 1);
    }


    private bool checkBoard()
    {
        for (int i = 0; i < board.size; i++)
        {
            for (int j = 0; j < board.size; j++)
            {
                if (board.boardInt[i,j] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }


    public IEnumerator AnimateCell()
    {
        float duration = 0.2f; // total animation time
        float halfDuration = duration / 2f;

        // Phase 1: Scale up and fade out
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration; // converts the time between 0 and 1 for lerping
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, normalizedTime);
            spriteRenderer.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0.5f), normalizedTime);
            yield return null;
        }

        // Phase 2: Scale down and fade in
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / halfDuration;
            transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, normalizedTime);
            spriteRenderer.color = Color.Lerp(new Color(1, 1, 1, 0.5f), Color.white, normalizedTime);
            yield return null;
        }

        transform.localScale = Vector3.one; // reset scale to original
        spriteRenderer.color = Color.white;  // reset color to original
    }
}
