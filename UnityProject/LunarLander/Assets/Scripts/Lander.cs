using UnityEngine;
using System.Collections;

public class Lander : MonoBehaviour
{
	static private Lander instance;
	public float health;
	public float maxHealth;
	public float fuel;
	private int currentCamera = 0;
	public Camera[] cameras;
	public Camera miniMapCam;
	private bool exploded = false;
	public Texture2D healthBar;
	public Texture2D fuelBar;
	private bool started = false;
	private float distanceToGround = 0;
	public ParticleSystem dustEmitter;
	private LanderWiiMote controller;
	
	public Texture2D minimapIcon;
	
	public static Lander GetInstance() {
		return instance;
	}

	// Use this for initialization
	void Start ()
	{
		Camera cam = cameras [currentCamera];
		cam.enabled = true;
		cam.gameObject.GetComponent<AudioListener> ().enabled = true;
		instance = this;
		rigidbody.useGravity = false;
		controller = gameObject.GetComponent<LanderWiiMote> ();
		this.started = false;
	}
	
	void FixedUpdate ()
	{
		if (started) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
				distanceToGround = hit.distance;
				//print("Distance to Ground " + distanceToGround.ToString());
			}
		}
		//float height = 
	}
	
	public void startSimulation ()
	{
		started = true;
		gameObject.rigidbody.useGravity = true;
	}
	
	public bool isStarted ()
	{
		return started;
	}
	
	public float GetDistanceToGround ()
	{
		return distanceToGround;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Debug.Log("Current Cam: " + currentCamera.ToString());
		if (started) {
			UpdateArrow ();
			
			// blow dust if close to ground
			if (distanceToGround <= 10) {
				// only if main thruster is active
				if (controller.IsMainThrusterActive ()) {
				
					RaycastHit hitInfo;
					// only if ray intersects
					if (Physics.Raycast (controller.GetNozzleRay (), out hitInfo, 10)) {
						// only if ray intersects with ground
						if (hitInfo.collider is TerrainCollider) {
							dustEmitter.transform.position = new Vector3 (transform.position.x, transform.position.y - distanceToGround, transform.position.z);
							if (!dustEmitter.isPlaying) {
								dustEmitter.Play ();
							}
						} else {
							if (dustEmitter.isPlaying) {
								dustEmitter.Stop ();
							}
						}
					}
				} else {
					if(dustEmitter.isPlaying){
						dustEmitter.Stop();
					}
				}
			} else {
				if (dustEmitter.isPlaying) {
					dustEmitter.Stop ();
				}
			}
		}
	}
	
	void LateUpdate ()
	{
		UpdateCameraTranslates ();
	}
	
	void UpdateCameraTranslates ()
	{
		if (started) {
			if (!exploded) {
				foreach (Camera camera in cameras) {
					float x = gameObject.transform.position.x;
					float y = gameObject.transform.position.y;
					float z = gameObject.transform.position.z;
					if (camera.name.Equals ("CameraTop")) {
						y += 16;
					} else if (camera.name.Equals ("CameraSide")) {
						y += 1.5f;
						z -= 10;
					} else if (camera.name.Equals ("CameraSide2")) {
						x += 10;
					}
					camera.transform.position = new Vector3 (x, y, z);
				}
				//print("updating camera");
				miniMapCam.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + 150, gameObject.transform.position.z);
			} else {
				// GAME OVER or LEVEL OVER
			
				// Start cam animation
				Camera cam = cameras [currentCamera];
				Vector3 to = cam.gameObject.transform.position + Vector3.Scale (cam.gameObject.transform.forward, new Vector3 (-1, -1, -1));
				//cam.gameObject.transform.position = Vector3.Lerp(cam.gameObject.transform.position, to, 10 * Time.deltaTime);
				//MoveObject (cam.gameObject.transform, cam.gameObject.transform.position, to, 4);
				cam.transform.position = Vector3.Lerp (cam.transform.position, to, 5 * Time.deltaTime);
			}
		}
	}
	
	private int allowedPrints = 5;
	private int processedPrints = 0;

	void UpdateArrow ()
	{
		GameObject arrow = GameObject.Find ("Arrow");
		float x = transform.position.x;
		float y = transform.position.y + 5;
		float z = transform.position.z + 4.5f;
		arrow.transform.position = new Vector3 (x, y, z);
		if (!exploded) {
			
			GameObject nearestLandingPad = LandingPadGroup.GetInstance().GetNearestLandingPad (gameObject);
			//Debug.Log(nearestLandingPad.transform.ToString());
			Vector3 to = Vector3.forward;
			if (nearestLandingPad != null) {
				Vector3 landingPadSameYPosition = new Vector3 (nearestLandingPad.transform.position.x, arrow.transform.position.y, nearestLandingPad.transform.position.z);
				to = Vector3.Normalize ((landingPadSameYPosition - arrow.transform.position));
				//float y = Vector3.Angle (to, arrow.transform.forward);
				if (processedPrints < allowedPrints) {
					//Debug.Log ("Rotate Arrow from: " + arrow.transform.position.ToString () + " to: " + to.ToString ());
					processedPrints++;
				}
			}
			arrow.transform.forward = to;
			//arrow.transform.rotation = Quaternion.FromToRotation(arrow.transform.forward, to);
			//arrow.transform.forward = to;
		}
	}
	
	public void SwitchCameraLeft ()
	{
		//print ("Previous cam");
		if (cameras.Length == 1) {
			return;
		}
		float elapsed = Time.time - lastCamSwitchTime;
		if (elapsed >= 0.5f) {
			Camera cam1 = cameras [currentCamera];
			currentCamera--;
			if (currentCamera < 0) {
				currentCamera = 0;
			}
			Camera cam2 = cameras [currentCamera];
			cam1.enabled = false;
			cam1.gameObject.GetComponent<AudioListener> ().enabled = false;
			cam2.enabled = true;
			cam2.gameObject.GetComponent<AudioListener> ().enabled = true;
			lastCamSwitchTime = Time.time;
		}
		//yield return new WaitForSeconds(0.5f);
	}
	
	private float lastCamSwitchTime = 0;

	public void SwitchCameraRight ()
	{
		//print ("Next cam");
		if (cameras.Length == 1) {
			return;
		}
		float elapsed = Time.time - lastCamSwitchTime;
		if (elapsed >= 0.5f) {
			Camera cam1 = cameras [currentCamera];
			currentCamera++;
			if (currentCamera >= cameras.Length) {
				currentCamera = cameras.Length - 1;
			}
			Camera cam2 = cameras [currentCamera];
			cam1.enabled = false;
			cam1.gameObject.GetComponent<AudioListener> ().enabled = false;
			cam2.enabled = true;
			cam2.gameObject.GetComponent<AudioListener> ().enabled = true;
			lastCamSwitchTime = Time.time;
		}
	}
	
	void OnGUI ()
	{
		
		int textWidth = 200;
		int width = 240;
		int x = 10;
		int y = 64;
		int barHeight = 20;
		GUI.DrawTexture (new Rect (x, y, health * 2, barHeight), healthBar);		
		GUI.Label (new Rect (x + health, y, textWidth, 100), "Health " + health);
		y += barHeight+5;
		GUI.DrawTexture(new Rect(x, y, fuel*2, barHeight), fuelBar);		
		GUI.Label (new Rect (x + fuel, y, textWidth, 100), "Fuel " + fuel);
		y+=barHeight+5;
		
		// Minimap Icon of Lander
		Rect screenPoint = miniMapCam.rect;
		float miniMapCamHeight = (screenPoint.height*Screen.height)/2;
		float miniMapCamWidth = (screenPoint.width*Screen.width)/2;
		float miniMapCamCenterX = Screen.width * screenPoint.x + miniMapCamWidth;
		float miniMapCamCenterY = Screen.height - (Screen.height * (screenPoint.y + screenPoint.height))
			+ miniMapCamHeight;
		float iconWidth = 32;
		float iconHeight = 32;
		GUI.DrawTexture(new Rect(miniMapCamCenterX-(iconWidth/2), miniMapCamCenterY-(iconHeight/2), 
			iconWidth, iconHeight), minimapIcon);
		
		// TODO noch abhängig vom Scaling machen
		//int widthArrow = 120;
		//int heightArrow = 25;
		//int yArrow = (int)(Screen.height - distanceToGround - 10 - (heightArrow / 2));
		//int xArrow = (int)(Screen.width - 10 - (widthArrow / 2));
		//GUI.DrawTexture (new Rect (xArrow, yArrow, widthArrow, heightArrow), heightBarArrow);
		
		GUI.BeginGroup (new Rect (x, y, width, 50));
		GUI.Box (new Rect (0, 0, textWidth, 50), "Status:");
		/*GUI.Label (new Rect (5, 20, textWidth, 20), "Score: " + LandingPadGroup.GetInstance ().GetCurrentLevelScore ());
		GUI.Label (new Rect (5, 35, textWidth, 20), "Health: " + health.ToString ());
		GUI.Label (new Rect (5, 50, textWidth, 20), "Fuel: " + fuel.ToString ());*/
		GUI.Label (new Rect (5, 20, textWidth, 20), "Height: " + distanceToGround.ToString ());
		//GUI.Label (new Rect (5, 80, textWidth, 20), "Remaining: " + LandingPadGroup.GetInstance().GetRemainingLandingPads ().ToString ());
		GUI.EndGroup ();
	}
	
	void ReduceHealth (float subHealth)
	{
		//Debug.Log("Reduce Health: " + subHealth.ToString());
		health -= subHealth;
		if (health <= 0) {
			health = 0;
			Explode ();
		}
	}
	
	public void ReduceFuel (float speed)
	{
		float reducement = speed / 15000;
		// Debug.Log("Reduce Fuel: " + reducement);
		fuel -= reducement;
		if(fuel <= 0){
			LandingPadGroup.GetInstance().SetGameOver();
		}
	}
	
	public float GetFuel ()
	{
		return fuel;	
	}
	
	public void AddFuel (float bonusFuel)
	{
		fuel += bonusFuel;
	}
	
	private void Explode ()
	{
		// TODO Detonator starten
		if (!exploded) {
			StartCoroutine (BlowAway());
		}
	}
	
	private IEnumerator BlowAway ()
	{
			
		GameObject detonator = GameObject.Find ("Detonator");
		Detonator det = gameObject.GetComponentInChildren<Detonator> ();
		//det.gameObject.transform.parent = null;
		//transform.DetachChildren ();
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform> ();
		foreach (Transform childTransform in transforms) {
			//Debug.Log ("Boom for: " + childTransform.gameObject.name + " Mesh: " + childTransform.name);
			childTransform.parent = null;
			MeshFilter mesh = childTransform.gameObject.GetComponent<MeshFilter> ();
			if (mesh != null) {
				mesh.gameObject.AddComponent (typeof(Rigidbody));
				mesh.gameObject.rigidbody.isKinematic = false;
				mesh.gameObject.rigidbody.useGravity = true;
				mesh.gameObject.rigidbody.mass = 10;
			}
			if (childTransform.gameObject != detonator) {
				// detonator kills itself
				Destroy (childTransform.gameObject, 5);
			}
		
		}
		//GameObject boom = GameObject.Find ("Detonator");
		
		//if (boom != null) {
		//yield return new WaitForSeconds(1);
		//gameObject.transform.DetachChildren ();
		//Detonator det = GameObject.Find("Detonator").GetComponent<Detonator>();
		if (det != null) {
			det.Explode ();
		} else {
			Debug.Log ("Det is null");
		}
		Destroy (gameObject, 5);
		
		exploded = true;
		yield return new WaitForSeconds(3);
		//} else {
		//	Debug.Log ("Boom is null");
		//}

		//goBoom = false;
		//Transform[] childs = gameObject.GetComponentsInChildren<Transform>();
		//foreach(Transform child in childs){
		//Debug.Log("Destroy " + child.gameObject.name + " Mesh: " + child.name);
		//}
		LandingPadGroup.GetInstance().SetGameOver();
	}
}
