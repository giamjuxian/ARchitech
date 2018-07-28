using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Login : MonoBehaviour
{
    [SerializeField]
    public GameObject username;
    public GameObject email;
    public GameObject password;
    public GameObject currentPanel;
    public GameObject nextPanel;
    public GameObject failedPanel;
    public Button enter;
    public string Username;
    private string Email;
    private string Password;
    private string form;
    private bool EmailValid = false;


    public void ToggleOnClick()
    {

        Button toUpdate = enter.GetComponent<Button>();

        Username = username.GetComponent<InputField>().text;



        Password = password.GetComponent<InputField>().text;


        Debug.Log("LOAD TRIGGED");
        if (File.Exists(Application.persistentDataPath + "/userInfo.data"))
        {
            Debug.Log("FILE EXISTS");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/userInfo.data", FileMode.Open);
            Debug.Log("FILE READ");
            UserInfo userInfo = (UserInfo)bf.Deserialize(file);
            file.Close();



            if (userInfo.userData.ContainsKey(Username) && userInfo.userData[Username] == Password)
            {
                Debug.Log("YEY");
                currentPanel.SetActive(false);
                nextPanel.SetActive(true);
                username_static.username = Username;
                Debug.Log(username_static.username);

            }
            else if(!userInfo.userData.ContainsKey(Username) || userInfo.userData[Username] != Password)
            {
                Debug.Log("YEY1");
                currentPanel.SetActive(false);
                failedPanel.SetActive(true);
            }




        }
    }

}



