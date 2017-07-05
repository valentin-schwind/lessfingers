using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubesTaskScript : MonoBehaviour {
	/*
		IDEA: Once enabledTask is called : enable the TaskObjects,
		enable the cubes and reset their position
		code written by stray
	 */
	
	[SerializeField] private GameObject infoCanvas;
	[SerializeField] private GameObject cubeTaskParentObject;
	Typer typerCanvas;
	Text titleText;
	public int taskNumber;
	StraysGameManager gameManager;
	private GameObject cube;

	[SerializeField] GameObject cube_01;
	[SerializeField] GameObject cube_02;
	[SerializeField] GameObject cube_03;
	[SerializeField] GameObject cube_04;
	[SerializeField] GameObject cube_05;
	[SerializeField] GameObject cube_06;

	GameObject CubePrefab;

	[SerializeField]
	private Material cubeInUseMaterial;         //chose the material for the currently active cube

	public void enableTask ()
	{
		Vector3 temp_01 = new Vector3 (0.447f, 0.5048033f, 0.325f);
		Vector3 temp_02 = new Vector3 (0.687f, 0.5048033f, -0.012f);
		Vector3 temp_03 = new Vector3 (0.823f, 0.5048033f, -0.234f);
		Vector3 temp_04 = new Vector3 (-0.738f, 0.5048033f, 0.375f);
		Vector3 temp_05 = new Vector3 (-0.943f, 0.5048033f, 0.042f);
		Vector3 temp_06 = new Vector3 (-1.071f, 0.5048033f, -0.471f);
		cube_01.transform.position = temp_01;
		cube_02.transform.position = temp_02;
		cube_03.transform.position = temp_03;
		cube_04.transform.position = temp_04;
		cube_05.transform.position = temp_05;
		cube_06.transform.position = temp_06;

		cube_01.SetActive (true);
		cube_02.SetActive (true);
		cube_03.SetActive (true);
		cube_04.SetActive (true);
		cube_05.SetActive (true);
		cube_06.SetActive (true);

		gameManager = transform.GetComponent<StraysGameManager> ();
		typerCanvas = infoCanvas.transform.Find ("Dialogue").Find ("Text").GetComponent<Typer> ();
		titleText = infoCanvas.transform.Find ("TitleBar").Find ("Text").GetComponent<Text> ();
		titleText.text = "Task " + gameManager.getCurrentTaskNumber () + ": Cubes Task";
		typerCanvas.msg = "Use those cubes to your left and right to build a small Pyramid by stacking them." + "just grab the desired cube to pick it up and place it where you want it to be! ";
		infoCanvas.GetComponent<Canvas> ().enabled = true;
		cubeTaskParentObject.SetActive (true);
	}

	// disable all the cubes
	public void disableTask () {
		cubeTaskParentObject.SetActive(false);
		infoCanvas.GetComponent<Canvas>().enabled = false;

		cube_01.SetActive (false);
		cube_02.SetActive (false);
		cube_03.SetActive (false);
		cube_04.SetActive (false);
		cube_05.SetActive (false);
		cube_06.SetActive (false);

		this.enabled = false;
	}
		
}

