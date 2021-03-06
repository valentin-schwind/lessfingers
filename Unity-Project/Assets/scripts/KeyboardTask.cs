﻿using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap.Unity.DetectionExamples;
using UnityEngine.UI;
using System.Collections.Generic;
using Leap.Unity.InputModule;

public class KeyboardTask : MonoBehaviour {
    /*
        The script which handles the keyboard task.
        written by : Stray
    */
    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private GameObject keybey;      // the actual keyboard
    [SerializeField]
    private GameObject _LeapEventSystem;
	[SerializeField]
	private Text keyboardTopBarText;
    LeapInputModule leapInputModule;
    Typer typerCanvas;
    Text titleText;
    public int taskNumber;
    StraysGameManager gameManager;

	public void enableTask ()
	{
		leapInputModule = _LeapEventSystem.GetComponent<LeapInputModule> ();
		/*  the mode defines how interaction with the keyboard functions:
            tactile : buttons need to be "physically" pressed
            projectile : buttons must be hovered over and pinched
            hybrid : tactile in close range - else : projectile     */
		leapInputModule.InteractionMode = LeapInputModule.InteractionCapability.Hybrid;
		gameManager = transform.GetComponent<StraysGameManager> ();
		keyboardTopBarText.text = "";
        // the types is what "letter for letter" writes onto the text field.
        // this could be used to set the text on the canvas in code ( for simplicity )
        typerCanvas = infoCanvas.transform.Find ("Dialogue").Find ("Text").GetComponent<Typer> ();
		titleText = infoCanvas.transform.Find ("TitleBar").Find ("Text").GetComponent<Text> ();
		titleText.text = "Task " + gameManager.getCurrentTaskNumber () + ": KeyboardTask";
		typerCanvas.msg = "Write: \"I Love VR\" using the keyboard in front of you. " + "if you are done, just press the button below! ";
		infoCanvas.SetActive (true);
		infoCanvas.GetComponent<Canvas> ().enabled = true;
		typerCanvas.enabled = true;

        keybey.transform.Find("Top Bar").GetComponent<KeyboardInputHandler>().ClearStringBuilder();

        keybey.SetActive (true);
	}

    void Start () {
        //enableTask ();
    }



	public void disableTask () {

        leapInputModule.InteractionMode = LeapInputModule.InteractionCapability.Projective;
        keybey.SetActive(false);
            infoCanvas.GetComponent<Canvas>().enabled = false;
            this.enabled = false;
    }
}
