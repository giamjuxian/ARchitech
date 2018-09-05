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
            //if (File.Exists(Application.persistentDataPath + "/userMazeData.data"))
            //{
            //Debug.Log("== SAVE FILE EXISTS ==");

            //// Load the information of userMazeData from file
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(Application.persistentDataPath + "/userMazeData.data", FileMode.Open);
            //Debug.Log(Application.persistentDataPath);
            //UserMazeData userMazeData = (UserMazeData)bf.Deserialize(file);
            //file.Close();
            //userMazeData.createNewMazeData(username_static.email, title);
            //// Save the information back into the loaded file
            //saveFile(userMazeData);
            username_static.mazeTitle = title;
            username_static.newMaze = true;
            SceneManager.LoadScene(1);
            //}
            //else
            //{
            //Debug.Log("== SAVE FILE DOESN'T EXIST, CREATING NEW SAVE FILE ==");
            //UserMazeData userMazeData = new UserMazeData();
            //userMazeData.createNewMazeData(username_static.email, title);
            //saveFile(userMazeData);
            //username_static.mazeTitle = title;
            //SceneManager.LoadScene(1);
            //}
        }
        else
        {
            currentPanel.SetActive(false);
            failedPanel.SetActive(true);
        }

    }

}
