using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class createNew : MonoBehaviour {

    public GameObject mazeTitle;
    [SerializeField]
    private GameObject currentPanel;

    [SerializeField]
    private GameObject failedPanel;

	public void createMazeData()
    {

        Debug.Log(username_static.username);

        string title;
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

            SceneManager.LoadScene(1);


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
        if(userMazeData.ContainsKey(username)) {
            Maze[] mazeArray = userMazeData[username];
            for (int i = 0; i < mazeArray.Length; i++)
            {
                if (mazeArray[i].mazeTitle == "")
                {
                    mazeArray[i] = mazeToBeAdded;
                }
            }
        } else {
            Maze[] mazeArray = new Maze[10];
            mazeArray[0] = mazeToBeAdded;

            userMazeData.Add(username, mazeArray);
        }



    }


}

[Serializable]
class Maze
{
    public string mazeTitle;
    public CreatedBlock blocks;

  

    public Maze(string title)
    {
        this.blocks = new CreatedBlock(0, 0, 0, 0);
        this.mazeTitle = title;

    }




}
