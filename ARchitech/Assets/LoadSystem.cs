using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class LoadSystem : MonoBehaviour {

    // GAME CAMERA
    [SerializeField]
    private Camera playerCamera;

    // OTHER SCRIPTS
    private BlockSystem bSys;
    private BlockData blockData;

    // GAME BUTTONS
    [SerializeField]
    private GameObject loadButton;
    [SerializeField]
    private GameObject spawnButton;

    // PREFABS
    [SerializeField]
    private GameObject blockPrefab;

    // MATERIAL SELECTOR
    private int blockSelectCounter;

    // AVATAR PREFAB
    [SerializeField]
    private GameObject avatarPrefab;

    // Use this for initialization
    void Start () {
        bSys = GetComponent<BlockSystem>();
        blockData = GetComponent<BlockData>();
        blockSelectCounter = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (CrossPlatformInputManager.GetButtonDown("Spawn"))
        {
            spawnAvatar();
        }
        if (CrossPlatformInputManager.GetButtonDown("Load"))
        {
            loadBlocks();
        }
    }

    // This method loads the previously saved blocks
    private void loadBlocks()
    {
        Debug.Log("LOAD TRIGGED");
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData playerData = (PlayerData)bf.Deserialize(file);
            file.Close();

            blockData.gameData.Clear();
            blockData.gameData = playerData.createdBlockData;
        }

        placeLoadedBlocks();
    }

    private void spawnAvatar() {
        Vector3 spawnPos = new Vector3(-11f, 5f, -11f);
        GameObject avatar = Instantiate(avatarPrefab, spawnPos, Quaternion.identity) as GameObject;
    }

    private void placeLoadedBlocks()
    {
        foreach (var item in blockData.gameData)
        {
            Debug.Log("LOADED - " + item.Key + " | " + item.Value.buildPosX + "," + item.Value.buildPosY + "," + item.Value.buildPosZ + " | " + item.Value.blockSelectCounter);
            Vector3 blockBuildPos = new Vector3(item.Value.buildPosX, item.Value.buildPosY, item.Value.buildPosZ);

            GameObject newBlock = Instantiate(blockPrefab, blockBuildPos, Quaternion.identity) as GameObject; // Create new block object
            Block tempBlock = bSys.allBlocks[item.Value.blockSelectCounter]; // Spawn new block from dictionary
            newBlock.name = tempBlock.blockName + "-Block";
            newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
        }
    }
}
