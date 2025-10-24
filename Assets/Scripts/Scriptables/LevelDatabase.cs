using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Game/Level Database", order = 0)]
public class LevelDatabase : ScriptableObject
{
    [Serializable]
    public class LevelInfo
    {
        [Header("Level Details")]
        public int levelNumber = 1;

        [Header("Grid Settings")]
        public int rows = 2;
        public int cols = 3;

        [Header("Gameplay Settings")]
        public float showCardBackDelay = 1.5f;
    }

    [Header("All Level Configurations")]
    public List<LevelInfo> levels = new List<LevelInfo>();
}
