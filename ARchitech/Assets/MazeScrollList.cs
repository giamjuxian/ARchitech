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

    //Other Scripts
    private BlockSystem bSys;
    private BlockData blockData;


    private GameObject blockPrefab;


    void Start()
    {
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        RemoveButtons();
        AddButtons();

    }

    private void AddButtons()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://architech-f2f76.firebaseio.com/");
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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
                foreach (DataSnapshot data in snapshot.Children)
                {
                    Debug.Log("ERROR");
                    GameObject newButton = buttonObjectPool.GetObject();
                    newButton.transform.SetParent(contentPanel);

                    SampleButton sampleButton = newButton.GetComponent<SampleButton>();
                    MazeData mazeData = JsonUtility.FromJson<MazeData>(data.Children.ToString());
                    String text = data.ToString();
                    sampleButton.Setup(mazeData, text, this);
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

    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            buttonObjectPool.ReturnObject(toRemove);
        }
    }

    public void LoadBlocks(List<CreatedBlock> createdBlockData)
    {

        SceneManager.LoadScene(2);

        blockData.gameData = createdBlockData;

        foreach (var item in blockData.gameData)
        {
            Vector3 blockBuildPos = new Vector3(item.buildPosX, item.buildPosY, item.buildPosZ);

            GameObject newBlock = Instantiate(blockPrefab, blockBuildPos, Quaternion.identity) as GameObject; // Create new block object
            Block tempBlock = bSys.allBlocks[item.blockSelectCounter]; // Spawn new block from dictionary
            newBlock.name = tempBlock.blockName + "-Block";
            newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;
        }
    }


}
