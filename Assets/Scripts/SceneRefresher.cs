using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class SceneRefresher : MonoBehaviour
{
    private Button button;


    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(RefreshScene);

    }

    private void RefreshScene()
    {
        //DeductLife();
        SceneManager.LoadScene("Main");
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
}
