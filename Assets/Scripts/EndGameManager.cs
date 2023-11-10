using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;



public class EndGameManager : MonoBehaviour
{
    public TMP_Text movesText;
    public TMP_Text levelText;

    private Board board;
    private int moves;
    private bool lastMove = false;

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject internetPanel;
    public GameObject allLevelsDonePanel;

    private bool check = false;
    private bool check2 = false;


    public Button continueButton;
    public TMP_Text continueText;


    private InterstitialAdsClass interstitialAdsClass;

    private int tempLose = 0;

    private bool continuePressed = false;



    private void Start()
    {
        interstitialAdsClass = FindObjectOfType<InterstitialAdsClass>();
        board = FindObjectOfType<Board>();
        setMaxMove();
    }

    void Update()
    {
        movesText.text = moves.ToString();

        if (moves == 0)
        {
            lastMove = true;
        }

        if (!check)
        {
            finishLevel();
        }

        if (continuePressed && check && !check2 && lastMove)
        {
            realLoseGame();
            check2 = true;
        }
    }


    public void finishLevel()
    {
        if (lastMove && board.world.levels[board.level] != null)
        {
            // Game losed.
            temploseGame();
            check = true;
        }
       
        else if (lastMove)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                //You have finished all levels.
                allLevelsDone();
                check = true;

            }
            else
            {
                // No internet
                noInternet();
                check = true;

            }

        }
    }


    public void temploseGame()
    {
        if (tempLose == 0 && !continuePressed)
        {
            FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
            fadePanel.GameOver();
            losePanel.SetActive(true);
            tempLose++;
        }
    }


    public void get3Moves()
    {
        continuePressed = true;
        lastMove = false;
        interstitialAdsClass.Instance.ShowAd();
        moves = 3;
        movesText.text = moves.ToString();
        losePanel.SetActive(false);
    }


    public void realLoseGame()
    {
        if (lastMove && board.world.levels[board.level] != null)
        {
            //DeductLife();
            losePanel.SetActive(true);
            Debug.Log("***** lose ******");
            board.currentState = GameState.finish;
            StartCoroutine(delayLose(0.15f));
            board.stopAllPulse();
            continueButton.interactable = false;
            continueText.text = "You already got moves";
            interstitialAdsClass.Instance.ShowAd();
        }
    }


    IEnumerator delayLose(float delay)
    {
        yield return new WaitForSeconds(delay);
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
        losePanel.SetActive(true);
    }



    public void allLevelsDone()
    {
        board.currentState = GameState.finish;
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
        allLevelsDonePanel.SetActive(true);
        interstitialAdsClass.Instance.ShowAd();
    }


    public void noInternet()
    {
        board.currentState = GameState.finish;
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
        internetPanel.SetActive(true);
    }


    public void winGame()
    {
        board.currentState = GameState.finish;
        registerFinishedLevel(board.level);
        StartCoroutine(delayWin(0.15f));
        interstitialAdsClass.Instance.ShowAd();
    }


    IEnumerator delayWin(float delay)
    {
        yield return new WaitForSeconds(delay);
        FadePanelController fadePanel = FindObjectOfType<FadePanelController>();
        fadePanel.GameOver();
        winPanel.SetActive(true);
    }



    private long UnixTimestampFromDateTime(DateTime date)
    {
        return (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }


    public void DeductLife()
    {
        int currentLives = currentLives = PlayerPrefs.GetInt("currentLives", 3);
        DateTime lastLifeLostTime;

        if (currentLives > 0)
        {
            currentLives--;

            PlayerPrefs.SetInt("currentLives", currentLives);

            lastLifeLostTime = DateTime.UtcNow;
            PlayerPrefs.SetString("lastLifeLostTime", UnixTimestampFromDateTime(lastLifeLostTime).ToString());
            Debug.Log($"Saving time: {lastLifeLostTime}");
        }
    }


    // Should be reached from dot class and decremented by 1 if a move happens
    public void DecrementMove()
    {
        --moves;
        if (moves < 1)
        {
            moves = 0; // Reset the seen remaining moves to 0 instead of negative
        }
    }

    private void setMaxMove()
    {
        if (board.world.levels[board.level] != null)
        {
            levelText.text = board.level.ToString();
            moves = Mathf.FloorToInt(board.world.levels[board.level].maxMoves);
            //moves = moves + Mathf.FloorToInt(moves / 10) + 4;
        }
    }

    public void registerFinishedLevel(int level_number)
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", level_number + 1);
            Debug.Log(level_number + 1 + " registered...");
        }
    }
}