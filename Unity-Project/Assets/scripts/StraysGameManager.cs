using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

/*______________________________________________________________________________
GAME MANAGER
------------------------------------------------------------------------------
Skript which handles the correct procedure.
Sets the desired order of the task : either manually or by randomizing
The core of this script is the decisionmaking()-algorithm.
This method is called everytime the user pressed the load/finish task button.
It decides if and which task should be loaded or stopped and logs the corresponding task time.
loads next text after each succesful task.
loads next scene if all tasks are complete

ADDITIONAL INFORMATION about the procedure in general:

This script is a component of the GameManager-GameObject, which contains all the task-scripts.
We do not check if a user has completed a task correctly!!!
The user can finish any task at any given time by pressing a canvas-button ( which calls the decisionmaking method in this script)
if the button to end a task is pressed : the question_after_task shows up,
the user rates the task and by commiting his answer this script loads the next task.

make sure that all the gameobjects and their SerializeField are assigned correctly.
many of the scripts can be optimized since they have been implemented poorly on some ends.
written by : Stray
______________________________________________________________________________
*/
using UnityEngine.UI;

public class StraysGameManager : MonoBehaviour{
	public enum WhichTask : int
	{
		keyboardtask,
		drawtask,
		cubetask
	}
	// Alternativ Shuffle
	[Header ("Task Order (descending priority!)")]

	[Tooltip ("Highest priority! Ignores the  manual and random settings.")]
	public bool shuffleTask = true;

	[Tooltip ("Ignores the setting of first, second and third task and instead, chooses it randomly.")]
	public bool randomTask = false;

	int taskToStart;

	[Tooltip ("Task which will be started first.")]
	public WhichTask firstTask = WhichTask.cubetask;

	[Tooltip ("Task which will be started first.")]
	public WhichTask secondTask = WhichTask.drawtask;

	[Tooltip ("Task which will be started first.")]
	public WhichTask thirdTask = WhichTask.keyboardtask;

	private WhichTask[] tasks = new WhichTask[3];

	public LinkedList<WhichTask> taskslist = new LinkedList<WhichTask> ();
	public LinkedListNode<WhichTask> currenttask;

	// number of tasks completed in current scene.
	[HideInInspector] public int taskscomplete = 0;
	[HideInInspector] public bool taskisrunning = false;
	private double tasktime;
	//path in which the logfile will be created
	[HideInInspector] public string filePath;

	[Header ("required GameObjects")]
	[SerializeField] private GameObject introCanvas;
	private Text introCanvasText;
	[SerializeField] private GameObject infoCanvas;
	private Text infoCanvasText;
	[Tooltip ("The Questionaire for after all 3 tasks are complete")]
	[SerializeField] private GameObject handsQuestionaire;
	[SerializeField] GameObject uipanelmovement;

	[HideInInspector]public bool introActive = true;
	// Number of the current participant. Needed for the logs. Increase manualy
	private int parnumber;

	//PyramidTask pyramidTask;
	DrawTask drawTaskScript;
	KeyboardTask keyboardTaskScript;
	CubesTaskScript cubesTaskScript;
	LessFingerExperiment lfexperiment;
	PostTaskQuestionScript postTaskquestionScripter;
    UI_panel_movement uiMovementScript;

	public void RandomiseTaskOrder ()
	{
		taskslist.Clear ();

		taskToStart = PlayerPrefs.GetInt ("First Task");
		tasks [taskToStart] = firstTask;
		tasks [(taskToStart + 1) % 3] = secondTask;
		tasks [(taskToStart + 2) % 3] = thirdTask;
		PlayerPrefs.SetInt ("First Task", (taskToStart + 1) % 3);


		for (int j = 0; j < tasks.Length; j++) {
			taskslist.AddLast (tasks [j]);
		}
		currenttask = taskslist.First;
	}


	public void Awake (){
		// load the scripts from the current gameobject to enable/disable them.
		//pyramidTask = transform.GetComponent<PyramidTask>();
		drawTaskScript = transform.GetComponent<DrawTask> ();
		keyboardTaskScript = transform.GetComponent<KeyboardTask> ();
		lfexperiment = transform.GetComponent<LessFingerExperiment> ();
		postTaskquestionScripter = transform.GetComponent<PostTaskQuestionScript> ();
		cubesTaskScript = transform.GetComponent<CubesTaskScript> ();

        uiMovementScript = uipanelmovement.GetComponent<UI_panel_movement>();

		parnumber = lfexperiment.Participant;
		filePath = @"StudyLog_" + "participant_" + parnumber + "_" +System.DateTime.Now.Hour + System.DateTime.Now.Minute + ".csv";
		/*To set the order of the tasks, you have 3 options:
            (1) Set the order manually
            (2) Shuffle the order by randomly swapping tasks (not perfect randomness)
            (3) randomize further
        */
		//When tasks are manually chosen and both randomTask and shuffletask set to false
		tasks [0] = firstTask;
		tasks [1] = secondTask;
		tasks [2] = thirdTask;

		// When randomTask is set to true and shuffleTask is false in Inspector
		if (!shuffleTask && randomTask) {
			ShuffleArray (tasks);
		}

		// When shuffleTask is set to true in Inspector (highest priority)
		if (shuffleTask) {
			RandomiseTaskOrder ();
		}

		// print the order in which the tasks will be loaded -> just for visibility
		for (int i = 0; i < 3; i++) {
			print ("task" + i + tasks [i]);
		}
		// write the current participants number into a string and ińto the file
		string firstscenestring = "Participant" + parnumber;
		writeTextIntoFile (firstscenestring);

		currenttask = taskslist.First;
		// Start the checkinput coroutine
		StartCoroutine (checkinput ());
	}

	public IEnumerator checkinput (){
		/*
        checks every .001 seconds if the f key is pressed.
        if so : manually loads the decisionmaking() method.
        ONLY USE IN THE DRAW TASK,
        since the user cant end the scene by himself.
        TODO : Find an automated procedure for this.
        */
		while (true) {
			introActive = introCanvas.activeSelf;
			if (Input.GetKeyDown (KeyCode.F)) {
				decisionmaking ();
			}
			yield return new WaitForSeconds (.001f);
		}
	}



	public void decisionmaking (){
	/*
    Is called after user presses load/ finish button,
    or f key is pressed ( for draw task only)
    Checks which Task should be the task to run
    Starts that task if no task is already running.
    stops that taks if there is already one.

    TODO: for visiblity & reducing redundancy:
          create new methods :startATask & stopATask,
          to replace the content of the if-statements
    */
		WhichTask tasktoload;
		tasktoload = currenttask.Value;
		if (taskscomplete == 0) {
			startWithCurrentHands();
		}
		// if all tasks are already done : load next scene
		if (taskscomplete == 3) {
			handsQuestionaire.SetActive (true);
            uiMovementScript.enableQuestionaire();
			

        } else {
			runOrCompleteTask (tasktoload);
		}
	}


	public static void ShuffleArray<T> (T[] arr){
    	//Randomly swaps array elements with eachother.
		for (int i = arr.Length - 1; i >= 0; i--) {
			int r = UnityEngine.Random.Range (0, i);
			T tmp = arr [i];
			arr [i] = arr [r];
			arr [r] = tmp;
		}
	}

	public IEnumerator countTaskTime (string calledtask){
		/*
        once a task is started, counts the time it takes to complete the task.
        If task is stopped, writes time with current task into file
        */
		while (taskisrunning) {
			tasktime += Time.deltaTime;
			yield return null;
		}
		print ("time needed for current task: " + tasktime);
		//write the current task and the tasktime into the file via logtasktime() method
		writeTextIntoFile (calledtask);
		string tasktimestring = tasktime.ToString ("F3");
		writeTextIntoFile (tasktimestring);
		yield return null;
	}

	public void writeTextIntoFile (string texttoadd){
		/*
        simply writes its parameter into the logfile
        does not need to be the task time.
        e.g. : current participant-nr,taskname, annotations?
    	*/
		if (!File.Exists (filePath)) {
			//if there is no file yet :Create file to write to: (only needed once obv.)
			string createText = "Selfpresence in Virtual Reality Study" + Environment.NewLine;
			File.WriteAllText (filePath, createText);
		}
		// seperates all entrys by ";" for visibility
		string texttowrite = texttoadd + ",";               
		File.AppendAllText (filePath, texttowrite);
	}

	public int getCurrentTaskNumber (){
		return taskscomplete + 1;
	}

	void startWithCurrentHands ()
	{
		introCanvas.SetActive (false);
		string _currentHandName = lfexperiment.getCurrentHandName ();
		writeTextIntoFile (_currentHandName);
	}

	void runOrCompleteTask(WhichTask _task){

		if (taskisrunning == false) {
			tasktime = 0.0;
			taskisrunning = !taskisrunning;
			switch (_task) {
			case WhichTask.cubetask:
				print ("CubesTask started");
				cubesTaskScript.enableTask ();
				StartCoroutine (countTaskTime ("CubesTask"));
				break;
			case WhichTask.drawtask:
				print ("DrawTask started");
				drawTaskScript.enableTask ();
				StartCoroutine (countTaskTime ("DrawTask"));
				break;
			case WhichTask.keyboardtask:
				print ("KeyboardTask started");
				keyboardTaskScript.enableTask ();
				StartCoroutine (countTaskTime ("KeyboardTask"));
				break;
			}

		} else {
			switch (_task) {
			case WhichTask.cubetask:
				cubesTaskScript.disableTask ();
				break;
			case WhichTask.drawtask:
				drawTaskScript.disableTask ();
				break;
			case WhichTask.keyboardtask:
				keyboardTaskScript.disableTask ();
				break;
			}
			taskisrunning = false;
			taskscomplete = taskscomplete + 1;
			if (currenttask.Next != null) {
				currenttask = currenttask.Next;
			}
			postTaskquestionScripter.EnablePostTaskQuestionaire ();
		}
	}
}
