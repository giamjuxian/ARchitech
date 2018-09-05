using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase;
using Firebase.Unity.Editor;

public class MazeScrollList : MonoBehaviour
{
    public Transform contentPanel;
    public SimpleObjectPool buttonObjectPool;
    public bool buildMode;
    public bool playMode;

    //Other Scripts
    private BlockSystem bSys;
    private BlockData blockData;

    [SerializeField]
    private GameObject blockPrefab;

    private DatabaseReference databaseReference;


    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        blockData = GetComponent<BlockData>();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        RemoveButtons();
        if (buildMode)
        {
            AddBuildButtons();
        }
        if (playMode)
        {
            AddPlayButtons();
        }

    }

    private void AddBuildButtons()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("======= ADDING BUILD BUTTONS ========");
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(username_static.userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                //Handle Error
                Debug.Log("ERROR ==" + task.IsFaulted);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<string> mazeTitles = new List<string>();
                foreach (DataSnapshot data in snapshot.Children)
                {
                    mazeTitles.Add(data.Key);
                    //GameObject newButton = buttonObjectPool.GetObject();
                    //newButton.transform.SetParent(contentPanel);
                }

                foreach (string title in mazeTitles)
                {
                    Debug.Log(title);
                    GameObject newButton = buttonObjectPool.GetObject();
                    newButton.transform.SetParent(contentPanel);
                    SampleButton sampleButton = newButton.GetComponent<SampleButton>();
                    sampleButton.Setup(title, this, buildMode, playMode, username_static.userId);
                }
            }
        });


        //if (File.Exists(Application.persistentDataPath + "/userMazeData.data"))
        //{
        //    Debug.Log("== SAVE FILE EXISTS FOR SCROLL ==");
        //    // Load the information of userMazeData from file
        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Open(Application.persistentDataPath + "/userMazeData.data", FileMode.Open);
        //    Debug.Log(Application.persistentDataPath);
        //    UserMazeData userMazeData = (UserMazeData)bf.Deserialize(file);
        //    file.Close();
        //    MazeCollection mazeCollection = userMazeData.getMazeByUsername(username_static.username);
        //    Dictionary<String, MazeData> collection = mazeCollection.getCollection();
        //    foreach (KeyValuePair<String, MazeData> mazeData in collection)
        //    {
        //        Debug.Log("== ==");
        //        GameObject newButton = buttonObjectPool.GetObject();
        //        newButton.transform.SetParent(contentPanel);
        //        SampleButton sampleButton = newButton.GetComponent<SampleButton>();
        //        sampleButton.Setup(mazeData.Value, mazeData.Key, this);
        //    }
        //}
    }

    private void AddPlayButtons()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("====== ADDING PLAY BUTTONS =======");
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                //Handle Error
                Debug.Log("ERROR ==" + task.IsFaulted);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                List<string[]> maps = new List<string[]>();
                foreach (DataSnapshot user in snapshot.Children)
                {
                    foreach (DataSnapshot map in user.Children)
                    {

                        maps.Add(new string[2] { user.Key, map.Key });
                    }
                }

                foreach (string[] map in maps)
                {
                    Debug.Log("USER : " + map[0] + " MAP : " + map[1]);
                    GameObject newButton = buttonObjectPool.GetObject();
                    newButton.transform.SetParent(contentPanel);
                    SampleButton sampleButton = newButton.GetComponent<SampleButton>();
                    sampleButton.Setup(map[1], this, buildMode, playMode, map[0]);
                }
            }
        });
    }

    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            if (contentPanel.childCount > 0)
            {
                GameObject toRemove = transform.GetChild(0).gameObject;
                Debug.Log("REMOVING BUTTON " + toRemove);
                buttonObjectPool.ReturnObject(toRemove);
            }
        }
    }

    public void loadMazeBlocks(string mazeTitle, bool buildMode, bool playMode, string mapAuthor)
    {
        // Getting the Maze Data
        username_static.newMaze = false;
        username_static.mazeTitle = mazeTitle;
        if (buildMode && !playMode)
        {
            FirebaseDatabase.DefaultInstance.GetReference("users").Child(username_static.userId).Child(mazeTitle).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Handle Error
                    Debug.Log("ERROR ==" + task.IsFaulted);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    DataSnapshot mazeData = snapshot.Child("createdBlockData");
                    List<CreatedBlock> createdBlocks = new List<CreatedBlock>();
                    foreach (DataSnapshot createdBlockData in mazeData.Children)
                    {
                        int blockId = 0;
                        float buildPosX = 0;
                        float buildPosY = 0;
                        float buildPosZ = 0;
                        int blockSelectCounter = 0;
                        foreach (DataSnapshot a in createdBlockData.Children)
                        {
                            switch (a.Key)
                            {
                                case "blockId":
                                    blockId = int.Parse(a.Value.ToString());
                                    break;
                                case "buildPosX":
                                    buildPosX = float.Parse(a.Value.ToString());
                                    break;
                                case "buildPosY":
                                    buildPosY = float.Parse(a.Value.ToString());
                                    break;
                                case "buildPosZ":
                                    buildPosZ = float.Parse(a.Value.ToString());
                                    break;
                                case "blockSelectCounter":
                                    blockSelectCounter = int.Parse(a.Value.ToString());
                                    break;
                            }
                        }
                        CreatedBlock block = new CreatedBlock(blockId, buildPosX, buildPosY, buildPosZ, blockSelectCounter);
                        createdBlocks.Add(block);
                    }
                    loadBlocks(createdBlocks, buildMode, playMode);
                }
            });
        }
        else if (!buildMode && playMode)
        {
            FirebaseDatabase.DefaultInstance.GetReference("users").Child(mapAuthor).Child(mazeTitle).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    //Handle Error
                    Debug.Log("ERROR ==" + task.IsFaulted);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    DataSnapshot mazeData = snapshot.Child("createdBlockData");
                    List<CreatedBlock> createdBlocks = new List<CreatedBlock>();
                    foreach (DataSnapshot createdBlockData in mazeData.Children)
                    {
                        int blockId = 0;
                        float buildPosX = 0;
                        float buildPosY = 0;
                        float buildPosZ = 0;
                        int blockSelectCounter = 0;
                        foreach (DataSnapshot a in createdBlockData.Children)
                        {
                            switch (a.Key)
                            {
                                case "blockId":
                                    blockId = int.Parse(a.Value.ToString());
                                    break;
                                case "buildPosX":
                                    buildPosX = float.Parse(a.Value.ToString());
                                    break;
                                case "buildPosY":
                                    buildPosY = float.Parse(a.Value.ToString());
                                    break;
                                case "buildPosZ":
                                    buildPosZ = float.Parse(a.Value.ToString());
                                    break;
                                case "blockSelectCounter":
                                    blockSelectCounter = int.Parse(a.Value.ToString());
                                    break;
                            }
                        }
                        CreatedBlock block = new CreatedBlock(blockId, buildPosX, buildPosY, buildPosZ, blockSelectCounter);
                        createdBlocks.Add(block);
                    }
                    loadBlocks(createdBlocks, buildMode, playMode);
                }
            });
        }
    }

    public void loadBlocks(List<CreatedBlock> createdBlockData, bool buildMode, bool playMode)
    {
        if (buildMode)
        {
            Debug.Log("SAVING BLOCKS");

            // Change the new data to be saved
            MazeData mazeData = new MazeData();
            mazeData.createdBlockData = createdBlockData;

            // Save the data into the file
            saveFile(mazeData);

            Debug.Log("SAVING BLOCKS DONE");
            SceneManager.LoadScene(1);
        }

        if (playMode)
        {
            Debug.Log("SAVING BLOCKS");

            // Change the new data to be saved
            MazeData mazeData = new MazeData();
            mazeData.createdBlockData = createdBlockData;

            // Save the data into the file
            saveFile(mazeData);

            Debug.Log("SAVING BLOCKS DONE");
            SceneManager.LoadScene(2);
        }

    }

    // This method creates a new file or updates and overwrite the current saved file
    private void saveFile(MazeData mazeData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/mazeData.data");
        bf.Serialize(file, mazeData);
        file.Close();
    }
}
