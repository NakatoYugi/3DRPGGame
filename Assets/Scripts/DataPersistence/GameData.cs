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

    //�ù��캯���ڵı����ᱻ����Ĭ��ֵ
    //��û�����ݼ��ػ�ʼ����Ϸʱʹ��Ĭ��ֵ
    public GameData()
    {
        xPosition = 0; 
        yPosition = 0; 
        zPosition = 0;
        secondsPlayed = 0f;
    }
}
