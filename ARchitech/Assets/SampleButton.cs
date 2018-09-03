using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour {

    public Button buttonComponent;
    public Text buttonLabel;

    private MazeData mazeData;
    private MazeScrollList scrollList;

	// Use this for initialization
	void Start () {
        buttonComponent.onClick.AddListener(HandleClick);
	}

    public void Setup(MazeData value, String key, MazeScrollList currentMazeScrollList) {
        mazeData = value;
        buttonLabel.text = key;
        scrollList = currentMazeScrollList;
        
    }

    public void HandleClick() {
        scrollList.LoadBlocks(mazeData.createdBlockData);
    }


}
