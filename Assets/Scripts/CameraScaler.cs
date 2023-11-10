using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{

    private Board board;

    // Considerably good repositioning measures for both portrait and landscape mode.
    private float CameraOffset = -10;
    //private float aspectRatio = .5625f;
    private float padding = 2;
    private float yOffset = 3;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            Reposition(board.size - 1, board.size - 1);
        }
    }


    // Reposition the camera according to the board.
    void Reposition(float x, float y)
    {
        Vector3 tempPosition = new Vector3((x/2), (y/2), CameraOffset);
        transform.position = tempPosition;

        //if (board.size >= board.size)
        //{
        //    Camera.main.orthographicSize = (board.size / 2 + padding) / aspectRatio; 
        //}
        //else
        //{
        Camera.main.orthographicSize = (board.size - yOffset / 2 + padding);
        //}
    }
}
