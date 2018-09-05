using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleButton : MonoBehaviour {

    public Button buttonComponent;
    public Text buttonLabel;
    private string mazeTitle;
    private MazeScrollList scrollList;
    private bool buildMode;
    private bool playMode;
    private string mazeAuthor;

	// Use this for initialization
	void Start () {
        buttonComponent.onClick.AddListener(HandleClick);
	}

    public void Setup(string title, MazeScrollList currentMazeScrollList, bool buildMode, bool playMode, string author)
    {
        mazeAuthor = author;
        mazeTitle = title;
        buttonLabel.text = title;
        scrollList = currentMazeScrollList;
        this.buildMode = buildMode;
        this.playMode = playMode;
    }

    public void HandleClick() {
        scrollList.loadMazeBlocks(mazeTitle, buildMode, playMode, mazeAuthor);
    }
}
