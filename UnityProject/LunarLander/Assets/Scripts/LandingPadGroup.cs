using UnityEngine;
using System.Collections;

// TODO umbenennen in LevelManager
[RequireComponent (typeof(LevelBounds))]
public class LandingPadGroup: MonoBehaviour
{
	private static LandingPadGroup instance;
	public GUISkin NormalSkin;
	//float nativeVerticalResolution = 1200;
	private bool isPaused;
	private bool gameOver;
	private float toggleTime;
	public int finishScore = 200;
	public float timeToBeat;
	private float elapsedTime;
	private LandingPlatform[] landingPads;
	public GUIText timeLabel;
	public GUIText remainingPadsLabel;
	public GUIText scoreLabel;
	public AudioClip selectSound;
	//private GUIText messageLabel;
	//private float startingTime = 4;
	
	public float goldMedalTime;
	public float silverMedalTime;
	public float bronzeMedalTime;
	public float medalScore;
	private string messageLabelText;
	private float messageLabelDisplayTime;
	private int messageLabelfontSize;
	private int activeMenuButton;
	public float gravityForce = 9.81f;
	private static readonly string[] pauseMenuButtonNames = {"Continue", "Restart", "Main Menu"};
	private static readonly string[] endMenuButtonNames = {"Restart", "Main Menu"};
	
	// Score stuff
	private int currentLevelScore = 0;
	
	public static LandingPadGroup GetInstance ()
	{
		return instance;
	}
	
	//static private int currentLandingPad;
	private LandingPadGroup ()
	{
	}
	
	void Start ()
	{
		//currentLandingPad = 0;
		Debug.Log ("Fake load level in LandingPadGroup");
		instance = this;
		elapsedTime = 0;
		this.landingPads = gameObject.GetComponentsInChildren<LandingPlatform> ();
		TheBrain.GetInstance ().StartNewLevel (Application.loadedLevelName);
		this.currentLevelScore = 0;
		Physics.gravity = new Vector3 (0, -gravityForce, 0);
		this.messageLabelDisplayTime = 4;
		this.messageLabelfontSize = 48;
		print ("Finished Fake load");
		Time.timeScale = 1;
		if(gameObject.audio == null) {
			gameObject.AddComponent(typeof(AudioSource));
		}
	}
	
	// Update is called once per frame
	private string[] currentButtons;
	private bool scalingUp;

	void Update ()
	{
		
		if ((isPaused || gameOver) && WiimoteReceiver.Instance.wiimotes.ContainsKey (1)) {
			currentButtons = isPaused ? pauseMenuButtonNames : endMenuButtonNames;
			//print ("Got WiiMote in Menu");
			Wiimote wiimote = WiimoteReceiver.Instance.wiimotes [1];
			float elapsedToggleTime = Time.realtimeSinceStartup - lastButtonToggleTime;
			if (wiimote.BUTTON_DOWN > 0 && elapsedToggleTime > 0.3f) {
				print ("DOWN");
				activeMenuButton++;
				if (activeMenuButton >= currentButtons.Length) {
					activeMenuButton = 0;
				}
				lastButtonToggleTime = Time.realtimeSinceStartup;
				audio.PlayOneShot(selectSound);
			}
			if (wiimote.BUTTON_UP > 0 && elapsedToggleTime > 0.3f) {
				print ("UP");
				activeMenuButton--;
				if (activeMenuButton < 0) {
					activeMenuButton = currentButtons.Length - 1;
				}
				lastButtonToggleTime = Time.realtimeSinceStartup;
				audio.PlayOneShot(selectSound);
			}
		}
		if (Lander.GetInstance ().isStarted () && !isPaused) {
			this.timeToBeat -= Time.deltaTime;
			this.elapsedTime += Time.deltaTime;
		
			this.timeLabel.text = ((int)Mathf.Round (timeToBeat)).ToString () + " s";
			
			this.remainingPadsLabel.text = GetRemainingLandingPads ().ToString () + "/" + 
				landingPads.Length.ToString ();
			
			this.scoreLabel.text = "Score: " + currentLevelScore.ToString ();
			
			this.messageLabelDisplayTime -= Time.deltaTime;
			
			if(timeToBeat <= 0){
				SetGameOver();
			}
		} else {
			//print("Countdown " + this.messageLabelDisplayTime.ToString());
			this.messageLabelDisplayTime -= Time.deltaTime;
			//print("Countdown2 " + this.messageLabelDisplayTime.ToString() + " " + Time.deltaTime);
			int displayTime = (int)Mathf.Round (this.messageLabelDisplayTime - 1);
			this.messageLabelText = displayTime.ToString ();
			if (displayTime == 0) {
				this.messageLabelText = "Start";
			} else if (displayTime < 0) {
				this.messageLabelText = null;
				Lander.GetInstance ().startSimulation ();
			}
		}
		/*if(landedRecently) {
			if(scoreLabelScale < 2 && scalingUp){
				scalingUp = true;
				scoreLabelScale += Time.deltaTime;
			}
			else if(scoreLabelScale >= 2){
				scalingUp = false;
				scoreLabelScale -= Time.deltaTime; 
			}
			if(!scalingUp && scoreLabelScale <= 2) {
				scoreLabelScale = 1;
				landedRecently = false;
			}
			scoreLabel.fontSize = (int)((18 * scoreLabelScale)+0.5f);
		}*/
		
	}
	
	private float lastButtonToggleTime;
	
	void OnGUI ()
	{	
		// Set up gui skin
		//GUI.skin = guiSkin;

		// Our GUI is laid out for a 1920 x 1200 pixel display (16:10 aspect). The next line makes sure it rescales nicely to other resolutions.
		//GUI.matrix = Matrix4x4.TRS (new Vector3 (0, 0, 0), Quaternion.identity, new Vector3 (Screen.height / nativeVerticalResolution, Screen.height / nativeVerticalResolution, 1)); 

		if (isPaused || gameOver) {
			
			// TODO wrap with GUI.Box
			//print ("center: " + (Screen.width / 2).ToString ());
			float width = 100;
			float heigthButton = 50;
			float x = (Screen.width / 2) - (width / 2);
			float yPadding = 10;
			float y = (Screen.height / 2) - (heigthButton * 2);
			
			if (WiimoteReceiver.Instance.wiimotes.ContainsKey (1)) {
				//print ("Got WiiMote in Menu");
				Wiimote wiimote = WiimoteReceiver.Instance.wiimotes [1];
				if (wiimote.BUTTON_A > 0) {
					//GUI.b
					string focusedControl = GUI.GetNameOfFocusedControl ();
					if (focusedControl.Length > 0) {
						if (isPaused && activeMenuButton == 0) {
							print ("Continue");
							Time.timeScale = 1;
							isPaused = false;   
						} else if (currentButtons [currentButtons.Length - 2].Equals (focusedControl)) {
							print ("Restart");
							Application.LoadLevel (TheBrain.GetInstance ().GetCurrentLevelName ());
						} else if (currentButtons [currentButtons.Length - 1].Equals (focusedControl)) {
							print ("Main Menu!");
							Application.LoadLevel (0);
						}
					}
				}
			}
			//activeMenuButton = GUI.SelectionGrid (new Rect (x, y, width, height*4), activeMenuButton, selectionStrings, 1);
			GUI.skin = NormalSkin;
			string menuTitle = isPaused ? "Pause" : "Game Over";
			float heightb = (heigthButton * currentButtons.Length) + (yPadding * (currentButtons.Length - 1));
			GUI.Box (new Rect (x - 20, y - 30, width + 40, heightb + 40), menuTitle);
			GUI.FocusControl (currentButtons [activeMenuButton]);
			
			for (int i=0; i<currentButtons.Length; i++) {
				GUI.SetNextControlName (currentButtons [i]);
				GUI.Button (new Rect (x, y + (i * (heigthButton + yPadding)), width, heigthButton), currentButtons [i]);
			}
			//GUI.F
		} else {
			if (messageLabelText != null) {
				if (messageLabelDisplayTime > 0) {
					GUI.skin = NormalSkin;
					NormalSkin.label.fontSize = messageLabelfontSize;
					float width = 300;
					float height = 150;
					float x = (Screen.width / 2) - (width / 2);
					float y = (Screen.height / 2) - (height / 2);
					GUI.Label (new Rect (x, y, width, height), messageLabelText, "label");
					//messageLabelDisplayTime -= Time.smoothDeltaTime;
				} else {
					messageLabelText = null;
				}
			}
		}
		
	}
	
	/*public static void NextLandingPad() {
		currentLandingPad++;
		if(currentLandingPad < landingPads.Length){
			GameObject landingPad = landingPads[currentLandingPad];
			landingPad.SendMessage("Activate");
			Debug.Log("");
		} else {
			Debug.Log("Level Complete!!");
		}
	}*/
	
	// useful for the arrow of the Lander
	public GameObject GetNearestLandingPad (GameObject ship)
	{
		GameObject nearestLandingPad = null;
		foreach (LandingPlatform landingPad in this.landingPads) {
			if (nearestLandingPad == null) {
				if (landingPad.activated) {
					nearestLandingPad = landingPad.gameObject;
				}
				continue;
			}
			float landingPadDist = Vector3.Distance (landingPad.transform.position, ship.transform.position);
			float nearestLandingPadDist = Vector3.Distance (nearestLandingPad.transform.position, ship.transform.position);
			if (landingPadDist < nearestLandingPadDist && landingPad.activated) {
				nearestLandingPad = landingPad.gameObject;
			}
		}
		return nearestLandingPad;
	}
	
	public int GetRemainingLandingPads ()
	{
		int remainingLandingPads = 0;
		foreach (LandingPlatform landingPad in this.landingPads) {
			if (landingPad.activated) {
				remainingLandingPads++;
			}
		}
		return remainingLandingPads;
	}
	
	private float GetElapsedTime ()
	{
		return elapsedTime;
	}
	
	public void TogglePause ()
	{
		print ("TogglePause " + toggleTime);
		if ((Time.realtimeSinceStartup - toggleTime) > 0.2f) {
			if (isPaused) {
				UnPauseGame ();
			} else {
				PauseGame ();
			}
			toggleTime = Time.realtimeSinceStartup;
		}
	}
	
	private void PauseGame ()
	{
		//print ("Paused");
		Time.timeScale = 0;
		//Lander.pauseSimulation();
		activeMenuButton = 0;
		isPaused = true;
	}
	
	private void UnPauseGame ()
	{
		//print ("Unpaused");
		Time.timeScale = 1;
		//Lander.startSimulation();
		isPaused = false;
	}
	
	public void SetMessageLabelText (string text, int fontSize, float displayTime)
	{
		Debug.Log ("Setting MessagLabel " + messageLabelText);
		this.messageLabelText = text;
		this.messageLabelDisplayTime = displayTime;
		this.messageLabelfontSize = fontSize;
		//yield return new WaitForSeconds(displayTime);
	}
	
	private class TextTimePair
	{
		private string text;
		private float time;
	}
	
	
	private int amountOfFeet;
	private GameObject lastLandingPad;
	private float impact;

	public void AboutToLand (GameObject landingPad, float impact, int amountOfFeet)
	{
		print ("About to land ...");
		this.lastLandingPad = landingPad;
		this.impact = impact;
		this.amountOfFeet = amountOfFeet;
	}
	
	float scoreLabelScale = 1;
	bool landedRecently = false;
	public IEnumerator LandedOnPlatform (GameObject landingPad, float distanceToCenter, float maxFuelRestore)
	{
		if (landingPad.Equals (lastLandingPad)) {
			
			Debug.Log ("Feet on the Ground: " + amountOfFeet + " ... you get Points and Fuel! Dist: " + distanceToCenter.ToString ());
			bool perfectLanding = amountOfFeet == 4;
			int multiplier = amountOfFeet;
			/*if(perfectLanding){
				// TODO perfect landing
				Debug.Log("Perfect Landing");
				SetMessageLabelText("Perfect Landing", 1f);
				yield return new WaitForSeconds(1f);
			}*/
			LandingPlatform platformScript = landingPad.GetComponent<LandingPlatform>();
			float points = platformScript.scorePoints - distanceToCenter - impact;
			int bonusMultiplier = platformScript.multiply;
			int score = ((int)(points+0.5f)) * (multiplier + bonusMultiplier);
			string scoreText = (perfectLanding ? "Perfect Landing\n" : "") +
				(multiplier + bonusMultiplier).ToString () + " x " + ((int)(score/(multiplier + bonusMultiplier))).ToString ();
			SetMessageLabelText (scoreText, 24, 2f);
			yield return new WaitForSeconds(2f);
			
			currentLevelScore += score;
			
			// TODO Score textlabel aufblitzen lassen
			float bonusFuel = (maxFuelRestore * multiplier) - distanceToCenter;
			Lander.GetInstance ().AddFuel (bonusFuel);
			Debug.Log ("Added Fuel");
			if (GetRemainingLandingPads () == 0) {
				yield return StartCoroutine(EndLevel());
			}
		}
	}
	
	private IEnumerator EndLevel ()
	{
		
		this.currentLevelScore += (int)((finishScore + timeToBeat + Lander.GetInstance ().GetFuel ())+0.5f);
		//this.currentLevelScore += (int)( + 0.5f);
		TheBrain.GetInstance ().ApplyCurrentScore (currentLevelScore);
		
		SetMessageLabelText ("Well done ...", 48, 3);
		
		yield return new WaitForSeconds(3);
		// TODO vernünftiges Verhalten für das Beenden eines Levels
		SetGameOver ();
	}
	
	public void SetGameOver ()
	{
		Time.timeScale = 0;
		//Lander.pauseSimulation();
		activeMenuButton = 0;
		isPaused = false;
		gameOver = true;
	}
	
	public float GetCurrentLevelScore ()
	{
		return currentLevelScore;
	}
}
