using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Game/Level Database", order = 0)]
public class LevelDatabase : ScriptableObject
{
    [Header("All Level Configurations")]
    public List<LevelData> levels = new List<LevelData>();
}

[Serializable]
public class LevelData
{
    public int levelNumber;
    [Space(10)]
    public int rows;
    public int cols;
    [Space(10)]
    public float showCardBackDelay;
}
