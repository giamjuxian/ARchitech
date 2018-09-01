using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using System;
using System.IO;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Firebase.Database;
using Firebase.Auth;

public class DatabaseScript : MonoBehaviour
{
    #region Variables
    DatabaseReference databaseReference;
    FirebaseAuth auth;
    string userName;
    string userEmail;
    string userId;
    #endregion

    #region Firebase Initialization
    // Use this for initialization
    void Start()
    {
        // Check the dependecies for Firebase Database
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Dependencies Resolved");
            }
            else
            {
                // Firebase Unity SDK is not safe to use here.
                Debug.LogError(string.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                return;
            }
        });
        // Set the Base URL for the database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://architech-f2f76.firebaseio.com/");
        // Get the root reference location of the database.
        databaseReference = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }
    #endregion

    #region Database Helper Function
    public void writeNewUserData(MazeData data, string userId, string mazeTitle) {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        string json = JsonUtility.ToJson(data);
        databaseReference.Child("users").Child(userId).Child(mazeTitle).SetRawJsonValueAsync(json);
    }
    #endregion
}

[Serializable]
public class MazeData
{
    public List<CreatedBlock> createdBlockData;

    public MazeData()
    {
        createdBlockData = new List<CreatedBlock>();
    }
}

[Serializable]
public class UserMazeData
{
    #region Variables
    // Variables
    private Dictionary<string, MazeCollection> userMazeData;
    #endregion

    #region Constructor
    // Constructor
    public UserMazeData()
    {
        this.userMazeData = new Dictionary<string, MazeCollection>();
    }
    #endregion

    #region Helper Methods
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
    public void saveMazeCollectionByUsername(MazeCollection newCollection, string username)
    {
        userMazeData[username] = newCollection;
    }
    #endregion
}

[Serializable]
public class MazeCollection
{
    #region Variables
    // Variables
    private Dictionary<string, MazeData> collection;
    #endregion

    #region Constructor
    // Constructor
    public MazeCollection()
    {
        collection = new Dictionary<string, MazeData>();
    }
    #endregion

    #region Helper Methods
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
    #endregion
}
