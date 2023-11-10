using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[CreateAssetMenu(fileName = "World", menuName = "World")]
public class World : ScriptableObject
{

    // World has levels to feed the scene...
    public Level[] levels;


    // Fill the available levels to world
    public void fillLevelsToWorld()
    {
        Debug.Log("levels are filling to world...");


        DirectoryInfo d = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] Files = d.GetFiles();

        this.levels = new Level[100];
        this.levels[0] = Level.CreateInstance(levelNo: 0, maxMoves: 5, size: 3, machineClicked: 1, nodeCycle: 0); // Tutorial level inside.

        int cycles = 0;

        for (int i = 0; i < Files.Length; i++)
        {
            if (i <= 20) cycles = 1;
            
            if (Files[i].Name != "player.dat" && Files[i].Name != ".DS_Store")
            {
                using (StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + Files[i].Name))
                {
                    //Debug.Log(Application.persistentDataPath + Files[i].Name);
                    string _level_no = "";
                    string _size = "";
                    string _machine = "";
                    string _maxMoves = "";
                    string line;
                    int line_ct = 0;
                    //Debug.Log("File Read Phase started!!!");
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line_ct == 0)
                        {
                            _level_no = line.Substring(7);
                        }
                        if (line_ct == 1)
                        {
                            _size = line.Substring(6);
                            Debug.Log("zzzzzz: "+_size);
                        }
                        if (line_ct == 2)
                        {
                            _machine = line.Substring(9);
                        }
                        if (line_ct == 3)
                        {
                            _maxMoves = line.Substring(10);
                            Debug.Log(_level_no + ">>>" + _size + ">" + _machine + ">" + _maxMoves);
                            this.levels[int.Parse(_level_no)] = (Level.CreateInstance(levelNo: int.Parse(_level_no), maxMoves: int.Parse(_maxMoves), size: int.Parse(_size), machineClicked: int.Parse(_machine), nodeCycle: cycles));
                        }
                        line_ct++;
                    }
                }
            }
        }
    }



    /*
    public void generateLevels()
    {
        
        const int baseSize = 4;
        const float baseComplexity = 1.0f;
        const float linearFactor = 0.3f;  // Reduced for slower complexity increase
        const float exponentialFactor = 1.03f;  // Reduced for slower complexity increase

        // Size progress 
        int GetBoardSize(int level)
        {
            return baseSize + (int)(2 * Mathf.Log(level + 1));  // Logarithmic increase for size
        }

        int GetComplexityVariable(int level)
        {
            float result = baseComplexity
                           + level * linearFactor
                           + baseComplexity * ((float)Mathf.Pow(exponentialFactor, level) - 1);

            return (int)result;
        }

        int CalculateClicksRequired(int boardSize, int complexity, int level)
        {
            const float boardSizeMultiplier = 0.2f;
            const float complexityMultiplier = 0.4f;
            const float levelMultiplier = 1.05f;  // Slightly reduced for a gentler curve

            float baseClicks = boardSize * boardSizeMultiplier + complexity * complexityMultiplier;
            int totalClicks = (int)Mathf.Round(baseClicks * Mathf.Pow(levelMultiplier, level));

            return totalClicks;
        }*/

    /*
    const int baseSize = 4;
    const float baseComplexity = 1.0f;
    const float linearFactor = 0.5f;  // Adjust as needed
    const float exponentialFactor = 1.05f;  // Adjust as needed

    // Size progress => 5
    int GetBoardSize(int level)
    {
        return baseSize + (int)Mathf.Sqrt(level);
    }

    int GetComplexityVariable(int level)
    {
        float result = baseComplexity
                       + level * linearFactor
                       + baseComplexity * ((float)Mathf.Pow(exponentialFactor, level) - 1);

        return (int)result; // Convert the final result to int
    }

    int CalculateClicksRequired(int boardSize, int complexity, int level)
    {
        const float boardSizeMultiplier = 0.2f;    // Adjust as needed
        const float complexityMultiplier = 0.4f;  // Adjust as needed
        const float levelMultiplier = 1.1f;       // Adjust as needed

        float baseClicks = boardSize * boardSizeMultiplier + complexity * complexityMultiplier;
        int totalClicks = (int)Mathf.Round(baseClicks * Mathf.Pow(levelMultiplier, level));

        return totalClicks;
    }

    this.levels = new Level[100];

    // Tutorial level
    this.levels[0] = Level.CreateInstance(maxMoves: 2, size: 4, machineClicked: 2);

    int size;
    int complexity;
    int requiredClicks;

    for (int i = 1; i < this.levels.Length; i++)
    {
        size = GetBoardSize(i);
        complexity = GetComplexityVariable(i);
        requiredClicks = CalculateClicksRequired(boardSize: size, complexity: complexity, level: i); //size * size * complexity / 20f;

        this.levels[i] = Level.CreateInstance(maxMoves: requiredClicks + requiredClicks/4 , size: size, machineClicked: requiredClicks);
    } 
}
*/
}



/*
    // Static first ten levels
    public void firstTenLevelsStatic()
    {
        // Tutorial 0.
        this.levels[0] = Level.CreateInstance(levelNo: 0, maxMoves: 5, size: 3, machineClicked: 1);
        this.levels[1] = Level.CreateInstance(levelNo: 1, maxMoves: 6, size: 4, machineClicked: 2);
        this.levels[2] = Level.CreateInstance(levelNo: 2, maxMoves: 7, size: 4, machineClicked: 3);
        this.levels[3] = Level.CreateInstance(maxMoves: 9, size: 4, machineClicked: 4);
        this.levels[4] = Level.CreateInstance(maxMoves: 9, size: 5, machineClicked: 4);
        this.levels[5] = Level.CreateInstance(maxMoves: 11, size: 5, machineClicked: 5);
        this.levels[6] = Level.CreateInstance(maxMoves: 12, size: 5, machineClicked: 6);
        this.levels[7] = Level.CreateInstance(maxMoves: 14, size: 5, machineClicked: 7);
    }
    */