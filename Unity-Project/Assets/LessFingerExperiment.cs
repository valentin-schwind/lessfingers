using UnityEngine;
using System.Collections;
using Leap.Unity;
using System.Timers;

public class LessFingerExperiment : MonoBehaviour
{

	[Header ("Participant")]
	public int Participant;
	private string[] layerNames;
	private Camera cam;
	private int handCounter = 0;
	private StraysGameManager straygm;

	void Start ()
	{
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		straygm = GameObject.Find ("GameController").GetComponent<StraysGameManager> ();

		print (straygm.ToString ());
		layerNames = new string[8];
		layerNames [0] = "R5";
		layerNames [1] = "R4";
		layerNames [2] = "R3";
		layerNames [3] = "R2";
		layerNames [4] = "A5";
		layerNames [5] = "A4";
		layerNames [6] = "A3";
		layerNames [7] = "A2";

		layerNames = GetLatinSquare (layerNames, Participant);


		foreach (string layer in layerNames)
			setVisible (layer, false);
		setVisible (layerNames [handCounter], true);
	}

	void setVisible (string _handRig, bool _visible)
	{
        
		if (_visible)
			cam.cullingMask |= 1 << LayerMask.NameToLayer (_handRig);
		else
			cam.cullingMask &= ~(1 << LayerMask.NameToLayer (_handRig));
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.N)) {
			foreach (string layer in layerNames)
				setVisible (layer, false);
			if (handCounter < layerNames.Length)
				handCounter++;
			Debug.Log ("handRig: " + layerNames [handCounter] + " " + (handCounter));
			straygm.RandomiseTaskOrder ();
		}

		if (Input.GetKeyDown (KeyCode.B)) {
			foreach (string layer in layerNames)
				setVisible (layer, false);
			if (handCounter > 0)
				handCounter--;
			Debug.Log ("handRig: " + layerNames [handCounter] + " " + (handCounter));
			straygm.RandomiseTaskOrder ();
		}

		setVisible (layerNames [handCounter], true);
        
	}

	public string getCurrentHandName ()
	{
		return layerNames [handCounter];
	}

	public static T[] GetLatinSquare<T> (T[] array, int participant)
	{
		int[,] latinSquare = new int[array.Length, array.Length];

		latinSquare [0, 0] = 1;
		latinSquare [0, 1] = 2;

		for (int i = 2, j = 3, k = 0; i < array.Length; i++) {
			if (i % 2 == 1)
				latinSquare [0, i] = j++;
			else
				latinSquare [0, i] = array.Length - (k++);
		}


		for (int i = 1; i <= array.Length; i++) {
			latinSquare [i - 1, 0] = i;
		}

		for (int row = 1; row < array.Length; row++) {
			for (int col = 1; col < array.Length; col++) {
				latinSquare [row, col] = (latinSquare [row - 1, col] + 1) % array.Length;

				if (latinSquare [row, col] == 0)
					latinSquare [row, col] = array.Length;
			}
		}

		T[] newArray = new T[array.Length];

		for (int col = 0; col < array.Length; col++) {
			int row = (participant + 1) % (array.Length);
			newArray [col] = array [latinSquare [row, col] - 1];
		}

		return newArray;
	}
}
