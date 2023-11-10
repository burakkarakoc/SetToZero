using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;


public class GemManager : MonoBehaviour
{

    public LevelHandler levelHandler;
    public TMP_Text livesText;

    public int maxLives = 3;
    private int currentLives;
    private float timeForRefresh = 30f * 60f; // 30 minutes in seconds
    private DateTime lastLifeLostTime;


    private void Awake()
    {
        //PlayerPrefs.SetInt("currentLives", 0);

        levelHandler = FindObjectOfType<LevelHandler>();

        currentLives = PlayerPrefs.GetInt("currentLives", maxLives);

        string timestampStr = PlayerPrefs.GetString("lastLifeLostTime", null);
        if (string.IsNullOrEmpty(timestampStr))
        {
            lastLifeLostTime = DateTime.UtcNow.AddMinutes(-timeForRefresh);
        }
        else
        {
            long timestamp = long.Parse(timestampStr);
            lastLifeLostTime = DateTimeFromUnixTimestamp(timestamp);
        }

        Debug.Log($"Loaded time: {lastLifeLostTime} | Current UtcNow: {DateTime.UtcNow}");

        CheckAndRefreshLives();
    }


    private DateTime DateTimeFromUnixTimestamp(long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
    }


    void CheckAndRefreshLives()
    {
        double elapsedMinutes = (DateTime.UtcNow - lastLifeLostTime).TotalMinutes;

        if (currentLives < maxLives)
        {

            // Check for a negative time which could arise from tampering or errors
            if (elapsedMinutes < 0)
            {
                Debug.LogWarning("Detected negative elapsed time. Possible time tampering by user.");
                return; // Do not refresh lives in this case
            }

            if (elapsedMinutes >= 30)  // 30 minutes
            {
                currentLives = maxLives;
                PlayerPrefs.SetInt("currentLives", currentLives);
            }
        }

        Debug.Log(elapsedMinutes + "elapsed mins...");
        livesText.text = currentLives.ToString();
        levelHandler.ChangeSprite();
    }


    public void RewardGem()
    {
        currentLives = PlayerPrefs.GetInt("currentLives", maxLives);
        currentLives++;
        PlayerPrefs.SetInt("currentLives", currentLives);
        CheckAndRefreshLives();
    }


    public void DeductLife()
    {
        if (currentLives > 0)
        {
            currentLives--;

            PlayerPrefs.SetInt("currentLives", currentLives);

            lastLifeLostTime = DateTime.UtcNow;
            PlayerPrefs.SetString("lastLifeLostTime", lastLifeLostTime.ToString("o"));  // Using the "o" format string for a round-trip format.
        }
        livesText.text = currentLives.ToString();
    }

}
