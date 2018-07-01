using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockData : MonoBehaviour
{
    public Dictionary<int, CreatedBlock> gameData;
    // Use this for initialization
    void Start()
    {
        gameData = new Dictionary<int, CreatedBlock>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

[Serializable]
public class CreatedBlock {
    public float buildPosX;
    public float buildPosY;
    public float buildPosZ;
    public int blockSelectCounter;

    public CreatedBlock(float posX, float posY, float posZ, int counter) {
        this.buildPosX = posX;
        this.buildPosY = posY;
        this.buildPosZ = posZ;
        this.blockSelectCounter = counter;
    }
}
