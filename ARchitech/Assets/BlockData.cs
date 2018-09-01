using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockData : MonoBehaviour
{
    public int id;
    public List<CreatedBlock> gameData;
    // Use this for initialization
    void Start()
    {
        gameData = new List<CreatedBlock>();
    }

}

[Serializable]
public class CreatedBlock {
    public int blockId;
    public float buildPosX;
    public float buildPosY;
    public float buildPosZ;
    public int blockSelectCounter;

    public CreatedBlock(int id, float posX, float posY, float posZ, int counter) {
        this.blockId = id;
        this.buildPosX = posX;
        this.buildPosY = posY;
        this.buildPosZ = posZ;
        this.blockSelectCounter = counter;
    }
}
