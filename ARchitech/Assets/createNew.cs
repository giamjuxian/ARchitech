using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class createNew : MonoBehaviour
{
    public GameObject mazeTitle;
    public GameObject currentPanel;
    public GameObject failedPanel;

    public void createMazeData()
    {
        string title;
        title = mazeTitle.GetComponent<InputField>().text;
        if (title != "")
        {
            if (File.Exists(Application.persistentDataPath + "/userMazeData.data"))
            {
                Debug.Log("== SAVE FILE EXISTS ==");

                // Load the information of userMazeData from file
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/userMazeData.data", FileMode.Open);
                Debug.Log(Application.persistentDataPath);
                UserMazeData userMazeData = (UserMazeData)bf.Deserialize(file);
                file.Close();
                userMazeData.createNewMazeData(username_static.email, title);
                // Save the information back into the loaded file
                saveFile(userMazeData);
                username_static.mazeTitle = title;
                SceneManager.LoadScene(1);
            }
            else
            {
                Debug.Log("== SAVE FILE DOESN'T EXIST, CREATING NEW SAVE FILE ==");
                UserMazeData userMazeData = new UserMazeData();
                userMazeData.createNewMazeData(username_static.email, title);
                saveFile(userMazeData);
                username_static.mazeTitle = title;
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            currentPanel.SetActive(false);
            failedPanel.SetActive(true);
        }

    }

    // This method creates a new file or updates and overwrite the current saved file 
    private void saveFile(UserMazeData userMazeData)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/userMazeData.data");
        bf.Serialize(file, userMazeData);
        file.Close();
    }
}



[Serializable]
class UserMazeData
{
    // Variables
    private Dictionary<string, MazeCollection> userMazeData;

    // Constructor
    public UserMazeData()
    {
        this.userMazeData = new Dictionary<string, MazeCollection>();
    }

    // This method creates a new maze data with maze title
    public void createNewMazeData(string username, string mazeTitle)
    {
        if (userMazeData.ContainsKey(username))
        {
            MazeCollection collection = userMazeData[username]; // Retrieve collection based on username
            collection.addNewMaze(mazeTitle); // Add new Maze into collection
        }
        else
        {
            MazeCollection collection = new MazeCollection();
            collection.addNewMaze(mazeTitle); // Add new Maze into collection
            userMazeData.Add(username, collection); // Add username with mazeCollection into the Dictionary
        }
    }

    // This method returns the maze using the username as key
    public MazeCollection getMazeByUsername(string mazeTitle)
    {
        return userMazeData[mazeTitle];
    }

    // This method saves the new maze collection by username
    public void saveMazeCollectionByUsername(MazeCollection newCollection, string username) {
        userMazeData[username] = newCollection;
    }
}

[Serializable]
class MazeCollection
{
    // Variables
    private Dictionary<string, MazeData> collection;

    // Constructor
    public MazeCollection()
    {
        collection = new Dictionary<string, MazeData>();
    }

    // This method adds a new maze into the collection with an empty maze data
    public void addNewMaze(string mazeTitle)
    {
        MazeData mazeData = new MazeData();
        collection.Add(mazeTitle, mazeData);
    }

    // This method retrieves the maze data by title of the maze
    public MazeData getMazeDataByTitle(string mazeTitle)
    {
        return collection[mazeTitle];
    }

    // This method saves the maze data by title of the maze
    public void saveMazeDataByTitle(MazeData mazeData, string mazeTitle)
    {
        collection[mazeTitle] = mazeData;
    }
}
