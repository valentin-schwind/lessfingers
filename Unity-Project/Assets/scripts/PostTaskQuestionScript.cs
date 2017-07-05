using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Timers;

public class PostTaskQuestionScript : MonoBehaviour {
	/*
		IDEA: after each completed task, ask the user how he would rate the task.
		enables the questionaire_panel after a task is completed,
		disables it once the user confirmed his answer & logs the answer

		code by : stray
	*/
	[SerializeField] Button forwardbutton_2;
	[SerializeField] GameObject UIElementsPanel_Question_after_Task;
	[SerializeField] GameObject ThanksPanel;
	[SerializeField] GameObject UIElementsPanel_Buttons;
	[SerializeField] GameObject Whole_Questionnaire;
	[HideInInspector] public bool answerGiven;
	private double currentTime;
    private string builder;


	// The gameobject called StraysGameManager
	[SerializeField] GameObject gameManager;
	//the actual script, which is a component of the gameobject
	StraysGameManager stray;
	//set the filepath here! make sure the name matches the one in StraysGameManager!!!
	private string filePath;


	void Start () {
		// instantiate the StraysGameManager script.
		stray = gameManager.GetComponent<StraysGameManager>();
		forwardbutton_2.onClick.AddListener (questionaftertask);
		filePath = stray.filePath;
	}

	public void EnablePostTaskQuestionaire(){
		Whole_Questionnaire.SetActive(true);
		UIElementsPanel_Buttons.SetActive (true);
		UIElementsPanel_Question_after_Task.SetActive (true);
	}

	private IEnumerator ShowThanksPanel(){
		UIElementsPanel_Buttons.SetActive (false);
		UIElementsPanel_Question_after_Task.SetActive (false);
		ThanksPanel.SetActive (true);
		currentTime = 0;
		while (currentTime < 4) {
			currentTime += Time.deltaTime;
			yield return null;
		}
		ThanksPanel.SetActive (false);
		DisablePostTaskQuestionaire();
		yield return null;

	}

	public void DisablePostTaskQuestionaire(){
		Whole_Questionnaire.SetActive (false);
		stray.decisionmaking ();	
	}

	void questionaftertask() {
		/*	triggers if the user presses the confirm-button.
			creates a ToggleGroup for the buttons.
			ToggleGroups ensure that at any time only one of the buttons can be toggled. */
		ToggleGroup question_after = UIElementsPanel_Question_after_Task.GetComponentInChildren<ToggleGroup>() as ToggleGroup;
		if (question_after.AnyTogglesOn ()) {
			IEnumerator<Toggle> toggleEnum = question_after.ActiveToggles().GetEnumerator();
			toggleEnum.MoveNext();
			Toggle toggle = toggleEnum.Current;
			// "collect" toggles -> read which answer the user has given
			string string_toggle = collectToggles (toggle); 
            question_after.SetAllTogglesOff();
			builder = string_toggle + ","; 
			//write the answer into the file
			StartCoroutine ("csvWrite");
			StartCoroutine("ShowThanksPanel");
		}
	}

	string collectToggles(Toggle toggle){	
		//Checks which button was toggled and returns it's value
		switch(toggle.name)
		{
		case "Toggle_1":
			return "1";
		case "Toggle_2":
			return "2";
		case "Toggle_3":
			return "3";
		case "Toggle_4":
			return "4";
		case "Toggle_5":
			return "5";
		case "Toggle_6":
			return "6";
		case "Toggle_7":
			return "7";
		default:
			return "f";
		}
	}

	void csvWrite() {
		/* Write user's answer into the log */
		File.AppendAllText (filePath, builder.ToString());
	}
		
}
