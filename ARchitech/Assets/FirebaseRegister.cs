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

public class FirebaseRegister : MonoBehaviour
{
    #region Variables
    public GameObject email;
    public GameObject password;
    public GameObject confirmPassword;
    public GameObject currentPanel;
    public GameObject nextPanel;
    public GameObject failedPanel;
    public Button enter;
    private string Email;
    private string Password;
    private string ConfirmPassword;
    private string form;
    private bool valid = false;
    #endregion

    #region Registration Handler
    // This method is triggered on click of the Enter Button. Performs the Registration to Firebase
    public void ToggleOnClick()
    {
        // Enter Button
        Button enterButton = enter.GetComponent<Button>();
        Password = password.GetComponent<InputField>().text;
        Email = email.GetComponent<InputField>().text;
        ConfirmPassword = confirmPassword.GetComponent<InputField>().text;

        #region Validation Checks
        // Check if any field is empty
        if (Password != "" && Email != "" && ConfirmPassword != "")
        {
            valid = true;
        }
        else
        {
            Debug.Log("Empty Field");
        }

        // Check if password and confirm password inputs are the same. If not the same, valid set to false
        if (Password == ConfirmPassword)
        {
            valid = true;
        }
        else
        {
            Debug.Log("Password not same as Confirm Password");
        }
        #endregion

        #region Execute Registration
        // Execute registration. If error, show the error page
        if (valid)
        {
            CreateNewUserInFirebase(Email, Password);
        }
        else
        {
            currentPanel.SetActive(false);
            failedPanel.SetActive(true);
        }
        #endregion
    }
    #endregion

    #region Helper Method
    public void CreateNewUserInFirebase(string email, string password)
    {
        Debug.Log("HERE");
        //FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).
        //    ContinueWith((obj) =>
        //    {
        //        Debug.Log(obj + " Added into Firebase");
        //        currentPanel.SetActive(false);
        //        nextPanel.SetActive(true);
        //    });

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            currentPanel.SetActive(false);
            nextPanel.SetActive(true);
        });
    }
    #endregion
}
