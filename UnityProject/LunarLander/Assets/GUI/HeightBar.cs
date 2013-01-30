using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class HeightBar : MonoBehaviour
{
	
	public Texture2D arrowTexture;
	
	// different overlays need to be sorted ascending
	public Texture2D[] scalingOverlays;
	
	// different scalings need to be sorted ascending and match with overlays
	public float[] scalings;
	
	// must match the last overlays max visible height
	public float maxVisibleHeight;
	public GameObject referencedGameObject;
	private Lander lander;
	
	// private attributes
	private float distanceToGround;
	private int currentScalingIndex;

	// Use this for initialization
	void Start ()
	{
		if (scalingOverlays.Length < scalings.Length) {
			Debug.LogError ("Lengths of overlays and scalings have to match!!!");
		}
		lander = referencedGameObject.GetComponent<Lander>();
	}
	
	void Update ()
	{
		distanceToGround = lander.GetDistanceToGround();
		float currentVisibleHeight = scalings [currentScalingIndex] * maxVisibleHeight;
		if (distanceToGround > (currentVisibleHeight * 2 / 3) && currentScalingIndex < scalings.Length) {
			Debug.Log ("currScalingIndex1: " + currentScalingIndex);
			currentScalingIndex++;
			Debug.Log ("currScalingIndex2: " + currentScalingIndex);
			//currentScalingIndex %= scalings.Length;
		} else if (distanceToGround < (currentVisibleHeight * 1 / 3) && currentScalingIndex > 0) {
			Debug.Log ("currScalingIndex3: " + currentScalingIndex);
			currentScalingIndex--;
			Debug.Log ("currScalingIndex4: " + currentScalingIndex);
			currentScalingIndex %= scalings.Length;
			/*if(currentScalingIndex < 0) {
					currentScalingIndex = 0;
				}*/
		}
	}
	
	// Update is called once per frame
	void OnGUI ()
	{
		GUITexture thisGuiTexture = this.gameObject.guiTexture;
		Rect screenRect = thisGuiTexture.GetScreenRect ();
		//thisGuiTexture.s
		
		
		// position for overlay
		
		// get the right overlay (currentScalingIndex) depending on the current height of the lander and
		
		// Verhältnis der aktuellen Höhe zur gewählten aktuellen maximal sichtbaren Höhe
		float currentScaling = scalings [currentScalingIndex];
		float currentMaxVisibleHeight = maxVisibleHeight * currentScaling;
		float relativeArrowYPosition = (distanceToGround / currentMaxVisibleHeight) * screenRect.height;
		//relativeArrowYPosition += screenRect.height/2;
		//relativeArrowYPosition += 3;
		
		// position for arrow
		float healthBarBottomY = Screen.height - (this.transform.position.y * Screen.height) - thisGuiTexture.pixelInset.y;
		
		// is like scaling for the gui texture
		float arrowHeight = 20;
		float yposArrow = healthBarBottomY - relativeArrowYPosition - (arrowHeight / 2);
		
		//Debug.Log("currScalingIndex: " + currentScalingIndex.ToString() + "calc YPOS: " + relativeArrowYPosition.ToString() + " ScreenRect: " + screenRect.ToString());
		float xArrow = (this.transform.position.x * Screen.width) + thisGuiTexture.pixelInset.x;
		//Debug.Log("calc yArrow: " + relativeArrowYPosition.ToString() + " calc ypos2: " + yposArrow.ToString() + " Screen height: " + Screen.height.ToString());
		
		
		float arrowWidth = 60;
		xArrow = xArrow + (screenRect.width / 2);
		//Debug.Log("calc xArrow: " + xArrow.ToString() + " ScreenWidth: " + Screen.width.ToString());
		//GUI.DrawTexture(new Rect(xArrow, ypos, arrowWidth, 30), arrowTexture);
		
		//Debug.Log("ScreenRect: " + screenRect.ToString());
		float yposOverlay = healthBarBottomY - screenRect.height + 5;
		//GUI.BeginGroup(screenRect);
		GUI.DrawTexture (new Rect (xArrow, yposOverlay, 25, (screenRect.height * 0.78855f)), scalingOverlays [currentScalingIndex]);
		GUI.DrawTexture (new Rect (xArrow - (arrowWidth / 2), yposArrow, arrowWidth, arrowHeight), arrowTexture);
		//GUI.EndGroup();
		// texture to use
	}
}
