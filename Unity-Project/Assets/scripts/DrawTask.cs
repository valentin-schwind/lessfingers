using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap.Unity.DetectionExamples;
using UnityEngine.UI;
using System.Collections.Generic;
using Leap;

public class DrawTask : MonoBehaviour {

	/*
	 Enables the pinch_draw script, which handles drawing in the scene.
	 Disables it after completion of the task.

	code written by nico h.
	revisited by stray.
	  */
    [SerializeField]
    private GameObject infoCanvas;
    private GameObject pinchDraw;
    PinchDraw pDraw;
    Typer typerCanvas;
    Text titleText;
    public int taskNumber;
    StraysGameManager gameManager;

    GameObject[] lineObjects;

	public void enableTask ()
	{
		/*
            When this script(on the current GameObject) is enabled,
            this script enables all the required Objects and Scripts for the task.
            First : find all the required Objects
        */
		gameManager = transform.GetComponent<StraysGameManager> ();
		typerCanvas = infoCanvas.transform.Find ("Dialogue").Find ("Text").GetComponent<Typer> ();
		pDraw = transform.GetComponent<PinchDraw> ();
		titleText = infoCanvas.transform.Find ("TitleBar").Find ("Text").GetComponent<Text> ();
		titleText.text = "Task " + gameManager.getCurrentTaskNumber () + ": DrawTask";
		typerCanvas.msg = "You can draw anywhere in the world by using the pinch gesture! " + "Use this to write \"Hello World\". ";
		// and then enable them
		infoCanvas.GetComponent<Canvas> ().enabled = true;
		typerCanvas.enabled = true;
		pDraw.enabled = true;
	}


	public void disableTask () {
        // Delete the drawn lines and disable the task-objects
        lineObjects = GameObject.FindGameObjectsWithTag("DynamicTag");
        for (int i = 0; i < lineObjects.Length; i++)
        {
            Destroy(lineObjects[i]);
        }

        pDraw.enabled = false;
        infoCanvas.GetComponent<Canvas>().enabled = false;
        this.enabled = false;        
    }
}
