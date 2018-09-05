using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class BuildingSystem : MonoBehaviour
{
    #region Variables
    // GAME CAMERA
    [SerializeField]
    private Camera playerCamera;

    // ATTRIBUTES
    private bool buildModeOn = false;
    private bool destroyModeOn = false;
    private bool canBuild = false;
    private bool canDestroy = false;
    private Vector3 buildPos;
    private GameObject blockToDestroy;
    private GameObject currentTemplateBlock; // variable to store current template block

    // OTHER SCRIPTS
    private BlockSystem bSys;
    private BlockData blockData;

    // GAME LAYERS
    [SerializeField]
    private LayerMask buildableSurfacesLayer;
    [SerializeField]
    private LayerMask destroyableSurfacesLayer;

    // GAME BUTTONS
    [SerializeField]
    private GameObject modeToggleButton;
    [SerializeField]
    private GameObject placeButton;
    [SerializeField]
    private GameObject saveButton;
    [SerializeField]
    private GameObject loadButton;
    [SerializeField]
    private GameObject changeButton;

    // PREFABS
    [SerializeField]
    private GameObject blockTemplatePrefab;
    [SerializeField]
    private GameObject blockPrefab;

    // MATERIALS
    [SerializeField]
    private Material templateMaterial;

    // MATERIAL SELECTOR
    private int blockSelectCounter;

    // SAVE LOAD VARIABLES
    private string email;
    private string mazeTitle;
    private string userId;

    // DATABASE
    DatabaseScript database;
    #endregion

    #region Initialization
    private void Start()
    {
        bSys = GetComponent<BlockSystem>();
        blockData = GetComponent<BlockData>();
        blockSelectCounter = 0;
        buildModeOn = true;
        destroyModeOn = false;
        email = username_static.email;
        mazeTitle = username_static.mazeTitle;
        userId = username_static.userId;
        database = GetComponent<DatabaseScript>();
        if (!username_static.newMaze)
        {
            loadBlocks();
        }

    }
    #endregion

    #region Button Toggle Update
    private void Update()
    {
        // Toggle Build and Destroy Mode
        if (CrossPlatformInputManager.GetButtonDown("ModeToggle"))
        {
            buildModeOn = !buildModeOn;
            destroyModeOn = !destroyModeOn;
        }

        if (CrossPlatformInputManager.GetButtonDown("Back"))
        {
            SceneManager.LoadScene(0);
        }

        // Print Current Mode
        if (buildModeOn && !destroyModeOn)
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Build Mode";
            placeButton.GetComponentInChildren<Text>().text = "Place";
            buildMode();
        }
        else if (!buildModeOn && destroyModeOn)
        {
            modeToggleButton.GetComponentInChildren<Text>().text = "Destroy Mode";
            placeButton.GetComponentInChildren<Text>().text = "Destroy";
            destroyMode();
        }

        if (CrossPlatformInputManager.GetButtonDown("Save"))
        {
            saveBlocks();
        }

        //if (CrossPlatformInputManager.GetButtonDown("Load"))
        //{
        //    loadBlocks();
        //}
    }
    #endregion

    #region Main Building Mechanics
    // This method enables the build mode for users to build the blocks 
    private void buildMode()
    {
        // Ray Cast if build mode is true
        if (buildModeOn)
        {
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 100, buildableSurfacesLayer)) // Raycast on the buildable surfaces
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y) + 0.8f, Mathf.Round(point.z));
                canBuild = true;
            }
            else
            {
                Destroy(currentTemplateBlock.gameObject);
                canBuild = false;
            }
        }

        // Destroy block when build mode is off and template is still present. Make sure that the block cannot be build without build mode 
        if (!buildModeOn && currentTemplateBlock != null)
        {
            Destroy(currentTemplateBlock.gameObject);
            canBuild = false;
        }

        // Create currentTemplateBlock when canBuild is on 
        if (canBuild && currentTemplateBlock == null)
        {
            currentTemplateBlock = Instantiate(blockTemplatePrefab, buildPos, Quaternion.identity); // Creates template block
            currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial; // Create template block and update the material
        }

        // Building part of system
        if (canBuild && currentTemplateBlock != null)
        {
            currentTemplateBlock.transform.position = buildPos;
            if (CrossPlatformInputManager.GetButtonDown("Place"))
            {
                placeBlock();
            }
        }

        // Change Block when "Change Button is Clicked"
        if (CrossPlatformInputManager.GetButtonDown("Change"))
        {
            changeBlock();
        }
    }
    #endregion

    #region Building Mechanics Helper Methods
    // This method changes the material of the block to be built
    private void changeBlock()
    {
        blockSelectCounter++;
        if (blockSelectCounter >= bSys.allBlocks.Count)
        {
            blockSelectCounter = 0;
        }
    }

    // This method creates the block at the targetted location
    private void placeBlock()
    {
        GameObject newBlock = Instantiate(blockPrefab, buildPos, Quaternion.identity) as GameObject; // Create new block object

        Block tempBlock = bSys.allBlocks[blockSelectCounter]; // Spawn new block from dictionary
        newBlock.name = tempBlock.blockName + "-Block";
        newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;

        // Adding to Dictionary
        CreatedBlock createdBlock = new CreatedBlock(newBlock.GetInstanceID(), buildPos.x, buildPos.y, buildPos.z, blockSelectCounter);
        blockData.gameData.Add(createdBlock);
    }

    // This method enables the detroy mode for users to destroy the blocks 
    private void destroyMode()
    {
        // ray cast if destroy mode is true
        if (destroyModeOn)
        {
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 100, destroyableSurfacesLayer)) // raycast on the buildable surfaces
            {
                Vector3 point = buildPosHit.point;
                blockToDestroy = buildPosHit.transform.gameObject;
                buildPos = blockToDestroy.transform.position;
                canDestroy = true;
            }
            else
            {
                Destroy(currentTemplateBlock.gameObject);
                canDestroy = false;
            }
        }

        // destroy block when destroy mode is off and template is still present. make sure that the block cannot be build without build mode 
        if (!destroyModeOn && currentTemplateBlock != null)
        {
            Destroy(currentTemplateBlock.gameObject);
            canDestroy = false;
        }

        // create currenttemplateblock when canDestroy is on 
        if (canDestroy && currentTemplateBlock == null)
        {
            currentTemplateBlock = Instantiate(blockTemplatePrefab, buildPos, Quaternion.identity); // creates template block
            currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial; // create template block and update the material
        }

        // building part of system
        if (canDestroy)
        {
            currentTemplateBlock.transform.position = buildPos;
            if (CrossPlatformInputManager.GetButtonDown("Place"))
            {
                destroyblock();
            }
        }
    }

    // This method destroys the targeted block
    private void destroyblock()
    {
        if (blockToDestroy.name != "Ground")
        {
            CreatedBlock blockToDelete = blockData.gameData.Find(x => x.blockId == blockToDestroy.GetInstanceID());
            blockData.gameData.Remove(blockToDelete);
            Destroy(blockToDestroy);
        }
    }
    #endregion

    #region Load/Save
    // This method saves the current created blocks
    private void saveBlocks()
    {
        //if (File.Exists(Application.persistentDataPath + "/userMazeData.data"))
        //{
        // Load the save file for details
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Open(Application.persistentDataPath + "/userMazeData.data", FileMode.Open);
        //UserMazeData userMazeData = (UserMazeData)bf.Deserialize(file);
        //MazeCollection mazeCollection = userMazeData.getMazeByUsername(username);
        //file.Close();

        // Change the new data to be saved
        //MazeData mazeData = new MazeData();
        //mazeData.createdBlockData = blockData.gameData;

        //mazeCollection.saveMazeDataByTitle(mazeData, mazeTitle);
        //userMazeData.saveMazeCollectionByUsername(mazeCollection, username);

        // Save the data into the file
        //saveFile(userMazeData);
        //}

        MazeData mazeData = new MazeData();
        mazeData.createdBlockData = blockData.gameData;
        database.writeNewUserData(mazeData, userId, mazeTitle);
    }

    // This method loads the previously saved blocks
    private void loadBlocks()
    {
        Debug.Log("LOAD TRIGGED");
        if (File.Exists(Application.persistentDataPath + "/mazeData.data"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/mazeData.data", FileMode.Open);
            MazeData userMazeData = (MazeData)bf.Deserialize(file);
            file.Close();
            blockData.gameData.Clear(); // Clear the current blocks
            blockData.gameData = userMazeData.createdBlockData;
        }
        placeLoadedBlocks();
    }

    #region Load/Save Helper Methods
    // This method iterates through the data of the blocks and instantiate it into the plane
    private void placeLoadedBlocks()
    {
        foreach (var item in blockData.gameData)
        {
            //Debug.Log("LOADED - " + item.Key + " | " + item.Value.buildPosX + "," + item.Value.buildPosY + "," + item.Value.buildPosZ + " | " + item.Value.blockSelectCounter);
            Vector3 blockBuildPos = new Vector3(item.buildPosX, item.buildPosY, item.buildPosZ);
            GameObject newBlock = Instantiate(blockPrefab, blockBuildPos, Quaternion.identity) as GameObject; // Create new block object
            Block tempBlock = bSys.allBlocks[item.blockSelectCounter]; // Spawn new block from dictionary
            newBlock.name = tempBlock.blockName + "-Block";
            newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
        }
    }

    // This method creates a new file or updates and overwrite the current saved file 
    //private void saveFile(UserMazeData userMazeData)
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(Application.persistentDataPath + "/userMazeData.data");
    //    bf.Serialize(file, userMazeData);
    //    file.Close();
    //}
    #endregion

    #endregion
}



