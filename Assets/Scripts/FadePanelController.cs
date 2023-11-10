using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameStartAnim;
    public Animator pauseAnimator;

    public GameObject pausePanel;
    public GameObject pauseButton;


    private Board board;

    private int clickCount = 0;
    private int startClickCount = 0;

    private EndGameManager endGameManager;


    private void Awake()
    {
        board = FindObjectOfType<Board>();
        endGameManager = FindObjectOfType<EndGameManager>();
    }


    public void OK()
    {
        if (panelAnim != null && gameStartAnim != null && startClickCount == 0)
        {
            panelAnim.SetBool("Out", true);
            gameStartAnim.SetBool("Out", true);
            pauseButton.SetActive(true);
            board.Setup();
            startClickCount++;
        }
    }


    public void ContinuePlay()
    {
        if (panelAnim != null && gameStartAnim != null)
        {
            board.currentState = GameState.play;
            panelAnim.SetBool("Out", true);
            gameStartAnim.SetBool("Out", true);
            panelAnim.SetBool("Stop Game", false);
            pauseButton.SetActive(true);
            endGameManager.get3Moves();
        }

    }


    public void GameOver()
    {
        Debug.Log("Game Over Called");
        board.currentState = GameState.finish;
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Stop Game", true);
        pauseButton.SetActive(false);
        //board.Kill();
    }


    public void Pause()
    {
        clickCount++; 
        board.currentState = GameState.pause;
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Stop Game", true);
        pauseAnimator.SetBool("pause out", false);
        pausePanel.SetActive(true);
        
    }


    public void Play()
    {
        if (clickCount != 0)
        {
            board.currentState = GameState.play;
            panelAnim.SetBool("Out", true);
            panelAnim.SetBool("Stop Game", false);
            pauseAnimator.SetBool("pause out", true);
            clickCount = 0;
        }
    }



    //public void PauseHandler()
    //{
    //    clickCount++;
    //    Debug.Log("CLİCK: " + clickCount);

    //    if (clickCount == 1)
    //    {
    //        board.currentState = GameState.pause;
    //        panelAnim.SetBool("Out", true);
    //        pausePanel.SetActive(true);
    //    }
    //    else
    //    {
    //        board.currentState = GameState.play;
    //        panelAnim.SetBool("Out", false);
    //        pauseAnimator.SetBool("pause out", true);
    //    }

    //    clickCount = 0;
    //}
}
