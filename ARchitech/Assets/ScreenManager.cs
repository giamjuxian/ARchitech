using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour {

    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private GameObject mainPanel;

	// Use this for initialization
	void Start () {
        if (username_static.isLoggedIn) {
            loginPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
	}
}
