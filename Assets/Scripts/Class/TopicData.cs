﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TopicData {
    public int id;
    public string name;
    public string topic;
    //オイラー角キャリブレーションデータ
    public float head;
    public float roll;
    public float pitch;
    public Sprite image;
}