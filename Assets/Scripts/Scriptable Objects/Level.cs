using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    public int levelNo;
    public int size;
    public int machineClicked;
    public int maxMoves;
    public int nodeCycle;


    // Default constructor with parameters
    public void Init(int levelNo, int machineClicked, int size, int maxMoves, int nodeCycle)
    {
        this.levelNo = levelNo;
        this.size = size;
        this.machineClicked = machineClicked;
        this.maxMoves = maxMoves;
        this.nodeCycle = nodeCycle;
    }


    // Level Instance creator function
    public static Level CreateInstance(int levelNo, int machineClicked, int size, int maxMoves, int nodeCycle)
    {
        var lvl = ScriptableObject.CreateInstance<Level>();
        lvl.Init(levelNo, machineClicked, size, maxMoves, nodeCycle);
        return lvl;
    }
}
