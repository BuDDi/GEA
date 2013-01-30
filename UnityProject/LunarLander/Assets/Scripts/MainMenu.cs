using UnityEngine;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
	
	public GUISkin NormalSkin;
	private static readonly string[] menuButtonNames = {"Play", "Credits", "Quit"};
	//private static readonly string[] availableLevels = {"Moon", "Mars"};
	private WiimoteReceiver receiver;
	private int activeMenuButton;
	private float lastButtonToggleTime;
	private float lastSubButtonToggleTime;
	public Texture2D connectedIcon;
	private int selectedLevel = 0;
	private int selectedSaveGame = 1;
	private List<string> availableLevels;
	
	// Use this for initialization
	void Start ()
	{
		// should initialize TheBrain
		receiver = WiimoteReceiver.Instance;
		receiver.connect ();
		//if(TheBrain.GetInstance().LoadedSaveGame()){
			TheBrain.GetInstance ().LoadSaveGame (selectedSaveGame);
		/*} else {
			print("already loaded");
		}*/
		this.availableLevels = TheBrain.GetInstance ().GetAvailableLevels ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if (receiver.wiimotes.ContainsKey (1)) {	 // If mote 1 is connected
			Wiimote mymote = receiver.wiimotes [1];
		}*/
		
		if (receiver.wiimotes.ContainsKey (1)) {
			//print ("Got WiiMote in Menu");
			
			Wiimote wiimote = receiver.wiimotes [1];
			float elapsedToggleTime = Time.realtimeSinceStartup - lastButtonToggleTime;
			if (wiimote.BUTTON_DOWN > 0 && elapsedToggleTime > 0.2f) {
				print ("DOWN");
				activeMenuButton++;
				if (activeMenuButton >= menuButtonNames.Length) {
					activeMenuButton = 0;
				}
				lastButtonToggleTime = Time.realtimeSinceStartup;
			}
			if (wiimote.BUTTON_UP > 0 && elapsedToggleTime > 0.2f) {
				print ("UP");
				activeMenuButton--;
				if (activeMenuButton < 0) {
					activeMenuButton = menuButtonNames.Length - 1;
				}
				lastButtonToggleTime = Time.realtimeSinceStartup;
			}
			float elapsedSubMenuToggleTime = Time.realtimeSinceStartup - lastSubButtonToggleTime;
			if (wiimote.BUTTON_LEFT > 0 && elapsedSubMenuToggleTime > 0.2f) {
				print ("LEFT");
				selectedLevel--;
				if (selectedLevel < 0) {
					selectedLevel = 0;
				}
				lastSubButtonToggleTime = Time.realtimeSinceStartup;
			}
			if (wiimote.BUTTON_RIGHT > 0 && elapsedSubMenuToggleTime > 0.2f) {
				print ("RIGHT");
				selectedLevel++;
				if (selectedLevel >= availableLevels.Count) {
					selectedLevel = availableLevels.Count - 1;
				}
				lastSubButtonToggleTime = Time.realtimeSinceStartup;
			}
			
		}
	}
	
	// Update manipulates that values depending on button press
	private static readonly float maxWidth;
	
	void OnGUI ()
	{
		// TODO wrap with GUI.Box
		//print ("center: " + (Screen.width / 2).ToString ());
		if (receiver.isConnected () && receiver.wiimotes.ContainsKey (1)) {
			GUI.DrawTexture (new Rect (10, 10, 32, 32), connectedIcon);
		}
		
		float buttonWidth = 200;
		float buttonHeight = 50;
		
		// draw buttons
		float buttonX = (Screen.width / 2) - (buttonWidth / 2);
		float yPadding = 10;
		float buttonY = (Screen.height / 2) - (buttonHeight * 2);
			
		float height = (buttonHeight * menuButtonNames.Length) + 
			(yPadding * (menuButtonNames.Length - 1)) + 
				(yPadding * 2);
		float width = 500;
		//activeMenuButton = GUI.SelectionGrid (new Rect (x, y, width, height*4), activeMenuButton, selectionStrings, 1);
		GUI.skin = NormalSkin;
		float x = buttonX - yPadding;
		float y = buttonY - yPadding;
		GUI.Box (new Rect (x, y, width, height), "");
		GUI.FocusControl (menuButtonNames [activeMenuButton]);
			
		for (int i=0; i<menuButtonNames.Length; i++) {
			GUI.SetNextControlName (menuButtonNames [i]);
			GUI.Button (new Rect (buttonX, buttonY + (i * (buttonHeight + yPadding)), buttonWidth, buttonHeight), menuButtonNames [i]);
		}
		
		float xContent = x + yPadding + buttonWidth;
		float yContent = y + yPadding;
		float widthContent = x + width - xContent;
		float heightContent = height - (yPadding * 2);
		Rect drawingRect = new Rect (xContent, yContent, widthContent, heightContent);
		// draw content depending on current button
		switch (activeMenuButton) {
		case 0:
			// Play button -> level selection
			drawLevelSelection (drawingRect);
			break;
		case 1:
			// Credits
			drawCredits (drawingRect);
			break;
		case 2:
			// maybe animate menu because quit has nothing to display
			break;
		}
		if (receiver.wiimotes.ContainsKey (1)) {
			//print ("Got WiiMote in Menu");
			
			Wiimote wiimote = receiver.wiimotes [1];
			if (wiimote.BUTTON_A > 0) {
				//GUI.b
				string focusedControl = GUI.GetNameOfFocusedControl ();
				print ("Wanna ... " + focusedControl);
				if (focusedControl.Length > 0) {
					if (menuButtonNames [0].Equals (focusedControl)) {
						// start Level
						print (menuButtonNames [0] + ": " + availableLevels [selectedLevel]);
						//Time.timeScale = 1;
						Application.LoadLevel (availableLevels [selectedLevel]);
					} else if (menuButtonNames [1].Equals (focusedControl)) {
						// credits: nothing to do here (GUI handles that)
						print (menuButtonNames [0] + ": " + availableLevels [selectedLevel]);
							
					} else if (menuButtonNames [2].Equals (focusedControl)) {
						// Quit
						print (menuButtonNames [2]);
						Application.Quit ();
					}
				}
			}
		}
	}
	
	private void drawLevelSelection (Rect drawingRect)
	{
		int oldFontSize = GUI.skin.label.fontSize;
		TextAnchor oldAlignment = GUI.skin.label.alignment;
		int titleFontSize = 24;
		GUI.skin.label.fontSize = titleFontSize;
		//print ("Draw Level Selection at " + drawingRect.ToString ());
		string prefix = "< ";
		string suffix = " >";
		if (selectedLevel == 0) {
			prefix = "";
		} else if (selectedLevel == availableLevels.Count - 1) {
			suffix = "";
		}
		print(selectedLevel);
		string selectedLevelName = availableLevels [selectedLevel];
		string contentTitle = prefix + selectedLevelName + suffix;
		GUI.BeginGroup (drawingRect);
		GUI.Label (new Rect (0, 0, drawingRect.width, 50), contentTitle);
		// TODO LevelStats anzeigen
		if (TheBrain.GetInstance ().LoadedSaveGame ()) {
			int[] scoreList = TheBrain.GetInstance ().GetScoreListForLevel (selectedLevelName);
			float scoreLabelHeight = 20;
			int scoreLabelFontSize = 16;
			GUI.skin.label.fontSize = scoreLabelFontSize;
			//GUI.skin.label.alignment = TextAnchor.UpperLeft;
			//string scoreListString = "";
			for (int i=0; i<scoreList.Length; i++) {
				int score = scoreList [i];
				//print(score);
				string scoreRow = (i + 1).ToString () + ". " + (score.ToString ());
				//print(scoreRow);
				GUI.Label (new Rect (5, 50 + (i * 20), drawingRect.width / 2, 30), scoreRow);
			}
		} else {
			print ("not ready");
		}
		GUI.EndGroup ();
		GUI.skin.label.alignment = oldAlignment;
		GUI.skin.label.fontSize = oldFontSize;
		//print (scoreListString);
	}
	
	private void drawCredits (Rect drawingRect)
	{
		//print ("Draw Credits at " + drawingRect.ToString ());
		int oldFontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = 24;
		string contentTitle = "Credits";
		string contentBody = "Made by\nSteffen Buder";
		GUI.BeginGroup (drawingRect);
		GUI.Label (new Rect (0, 0, drawingRect.width, 30), contentTitle);
		GUI.skin.label.fontSize = 12;
		GUI.Label (new Rect (0, 60, drawingRect.width, 50), contentBody);
		GUI.EndGroup ();
		GUI.skin.label.fontSize = oldFontSize;
	}
}
