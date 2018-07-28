using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;

public class createNew : MonoBehaviour {

    public GameObject mazeTitle;

    public Button enter;

    public GameObject currentPanel;
    public GameObject nextPanel;
    public GameObject failedPanel;


    public void createMazeData()
    {

        Debug.Log(username_static.username);

        string title;

        Button toUpdate = enter.GetComponent<Button>();
        title = mazeTitle.GetComponent<InputField>().text;
        if (title != "")
        {
            Maze mazeCreated = new Maze(title);

            UserMazeData userMazeData = new UserMazeData();
            userMazeData.createNewMazeData(username_static.username, mazeCreated);

            Debug.Log("SAVE TRIGGED TO " + Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/userMazeData.data");

            bf.Serialize(file, userMazeData);
            file.Close();

            currentPanel.SetActive(false);
            nextPanel.SetActive(true);


        }
        else
        {
            currentPanel.SetActive(false);
            failedPanel.SetActive(true);
        }

    }
}

[Serializable]
class UserMazeData
{

    public Dictionary<string, Maze[]> userMazeData;

    public UserMazeData()
    {
        this.userMazeData = new Dictionary<string, Maze[]>();
    }

    public void createNewMazeData(string username, Maze mazeToBeAdded) {
        Maze[] mazeArray = userMazeData[username];
        for (int i = 0; i < mazeArray.Length; i++) {
            if(mazeArray[i].mazeTitle == "") {
                mazeArray[i] = mazeToBeAdded;
            }    
        }
    }


}

[Serializable]
class Maze
{
    public string mazeTitle;
    public CreatedBlock blocks;
    public Vector3 buildPos;

    public Maze(string title)
    {
        this.blocks = new CreatedBlock(buildPos.x, buildPos.y, buildPos.z, 0);
        this.mazeTitle = title;

    }




}
