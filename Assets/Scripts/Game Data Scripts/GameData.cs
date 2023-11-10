using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[Serializable]
public class SaveData
{
    public int[] levelNos;
    public int[] boardSizes;
    public int[] machineClicks;
    public int[] cycleTimes;
    public int[] maxTileNos;
}


// Keep in mind that the gamedata is only saved in RUNTIME. Any other changes will not be store.
public class GameData : MonoBehaviour
{
    private int levelsLength = 100;

    // You can click the GameData object in the editor and uncheck ON RUNTIME any level to test wheter levels are correct.
    public static GameData gameData;

    // needs to be public to be serialized by formatter
    public SaveData saveData;


    // Start is called before the first frame update
    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Load data if possible
        Load();

        if (saveData.levelNos.Length != levelsLength)
        {
            saveData.levelNos = new int[levelsLength];
            saveData.boardSizes = new int[levelsLength];
            saveData.machineClicks = new int[levelsLength];
            saveData.cycleTimes = new int[levelsLength];
            saveData.maxTileNos = new int[levelsLength];
        }
    }


    public void Save()
    {
        // Binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a route from the program to file
        FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Create);

        // Create a copy save data
        SaveData data = new SaveData();
        data = saveData;

        // Save the data
        formatter.Serialize(file, data);

        // Close the Filestream
        file.Close();

        Debug.Log("Saved");
    }

    private void OnDisable()
    {
        Save();
    }

    // Loads player data if possible
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/levels.dat"))
        {
            // Create a binary formatter
            BinaryFormatter formatter = new BinaryFormatter();

            // Open data file
            FileStream file = File.Open(Application.persistentDataPath + "/levels.dat", FileMode.Open);

            // Add it to saveData object
            saveData = formatter.Deserialize(file) as SaveData;

            // Close the Filestream
            file.Close();

            Debug.Log("Loaded");
        }
    }
}




// Save method example below:
/*
if (gameData != null)
{
    // Unlock the upcoming level
    gameData.saveData.isActives[board.level + 1] = true;

    // Update the highscore if necessary
    if (smanager.score > gameData.saveData.highScores[board.level])
    {
        gameData.saveData.highScores[board.level] = smanager.score;
    }

    // Save that level game data
    gameData.Save();
}
*/