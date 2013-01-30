using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LandingPadGroup))]
public class LevelBounds : MonoBehaviour
{
	
	// seconds to wait when game finishes
	public float timeOut = 20;
	private float secondsLeft;
	private bool outOfLevel;
	
	void Awake ()
	{
		outOfLevel = false;
		secondsLeft = timeOut;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (outOfLevel) {
			secondsLeft -= Time.deltaTime;
			if (secondsLeft <= 0) {
				// TODO GameOver
				Debug.Log ("Game Over!!!");
				LandingPadGroup.GetInstance().SetGameOver();
			}
		}
	}
	
	void OnGUI ()
	{
		// TODO Minimap mit allen Landeplattformen zeichnen
		if (outOfLevel) {
			int width = 240;
			int x = (Screen.width / 2) - (width / 2);
			//GUI.DrawTexture (new Rect (x, 10, health * 2, 20), healthBar);
			GUI.Label (new Rect (x, 210, width, 100), "Time left: " + secondsLeft.ToString("0.00") + "s");
		}
	}
	
	void OnTriggerEnter ()
	{
		outOfLevel = false;
		secondsLeft = timeOut;
	}
	
	void OnTriggerExit ()
	{
		outOfLevel = true;
	}
}
