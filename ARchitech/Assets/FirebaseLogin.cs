using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Firebase;
using Firebase.Auth;

public class FirebaseLogin : MonoBehaviour
{
    #region Variables
    public GameObject email;
    public GameObject password;
    public GameObject currentPanel;
    public GameObject nextPanel;
    public GameObject failedPanel;
    public Button enter;
    private string Email;
    private string Password;
    private string form;
    private bool validLogin = false;
    #endregion

    #region Login Handler
    // This method triggers the login upon click of enter
    public void loginTrigger()
    {
        Button toUpdate = enter.GetComponent<Button>();
        Email = email.GetComponent<InputField>().text;
        Password = password.GetComponent<InputField>().text;
        if (Email != "" && Password != "")
        {
            firebaseSignIn(Email, Password);
        }

    }
    #endregion

    #region Helper Methods
    // This method takes in email and password of user and check with the firebase authentication. If sign in successful, change scene
    private void firebaseSignIn(string email, string password)
    {
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("== SignInWithEmailAndPasswordAsync was canceled. ==");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("== SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception + " ==");
                currentPanel.SetActive(false);
                failedPanel.SetActive(true);
                return;
            }

            FirebaseUser newUser = task.Result;
            currentPanel.SetActive(false);
            nextPanel.SetActive(true);
            username_static.email = email;
            username_static.userId = newUser.UserId;
            username_static.isLoggedIn = true;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }
    #endregion
}
