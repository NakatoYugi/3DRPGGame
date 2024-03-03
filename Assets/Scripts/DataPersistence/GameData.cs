using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string characterName;

    public float secondsPlayed;

    public long lastUpdated;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    //该构造函数内的变量会被赋予默认值
    //当没有数据加载或开始新游戏时使用默认值
    public GameData()
    {
        xPosition = 0; 
        yPosition = 0; 
        zPosition = 0;
        secondsPlayed = 0f;
    }
}
