using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class LanderWiiMote : MonoBehaviour
{
	
	private ParticleSystem mainThruster;
	public float maxSpeed;
	public float acceleration;
	private float speed;
	private float resetRotSpeed = 0;
	public float maxHeight = 550;
	
	/*public float maxRotX;
	public float minRotX;
	public float maxRotZ;
	public float minRotZ;*/
	private float currentAngleX;
	private float currentAngleZ;
	private Vector3 direction = Vector3.up;
	private WiimoteReceiver receiver;
	public bool keepUpRotation = false;
	public GameObject jetBone;
	private float impactMultiplier = 1.2f;
	private float impactLimit = 2.5f;
	
	// Side Thruster speed
	public float maxSideThrusterSpeed;
	public float sideThrusterAcceleration;
	public ParticleSystem leftThruster;
	private float leftThrusterSpeed = 0;
	public ParticleSystem rightThruster;
	private float rightThrusterSpeed = 0;
	public ParticleSystem frontThruster;
	private float frontThrusterSpeed = 0;
	public ParticleSystem backThruster;
	private float backThrusterSpeed = 0;
	
	// Top Thruster speed
	public float maxTopThrusterSpeed;
	public float topThrusterAcceleration;
	public ParticleSystem topThruster;
	private float topThrusterSpeed = 0;
	public bool printDebug = false;
	public GUITexture connectedIcon;
	private AudioSource mainThrusterSource;
	public AudioClip mainThrusterSound;
	public AudioClip sideThrusterSound;
	private AudioSource topThrusterSource;
	private AudioSource leftThrusterSource;
	private AudioSource rightThrusterSource;
	private AudioSource frontThrusterSource;
	private AudioSource backThrusterSource;

	// Use this for initialization
	void Start ()
	{
		receiver = WiimoteReceiver.Instance;
		receiver.connect ();
		mainThruster = gameObject.GetComponentInChildren<ParticleSystem> ();
		
		mainThrusterSource = gameObject.AddComponent<AudioSource> ();
		topThrusterSource = gameObject.AddComponent<AudioSource> ();
		leftThrusterSource = gameObject.AddComponent<AudioSource> ();
		rightThrusterSource = gameObject.AddComponent<AudioSource> ();
		frontThrusterSource = gameObject.AddComponent<AudioSource> ();
		backThrusterSource = gameObject.AddComponent<AudioSource> ();
		
		mainThrusterSource.clip = mainThrusterSound;
		
		topThrusterSource.clip = sideThrusterSound;
		topThrusterSource.volume = 0.7f;
		leftThrusterSource.clip = sideThrusterSound;
		leftThrusterSource.volume = 0.7f;
		rightThrusterSource.clip = sideThrusterSound;
		rightThrusterSource.volume = 0.7f;
		frontThrusterSource.clip = sideThrusterSound;
		frontThrusterSource.volume = 0.7f;
		backThrusterSource.clip = sideThrusterSound;
		backThrusterSource.volume = 0.7f;
	}
	
	void FixedUpdate ()
	{
		if (Lander.GetInstance().isStarted ()) {
			direction = jetBone.transform.right;
			//Debug.Log("Fly to: " + direction.ToString());
		
		
			if (speed > 0) {
				if (transform.position.y >= maxHeight) {
					direction.y = 0;
				}
				//rigidbody.AddForceAtPosition(direction * speed * Time.fixedDeltaTime, mainThruster.transform.position);
				rigidbody.AddForce (direction * speed * Time.fixedDeltaTime);
			
			
			}
			if (frontThrusterSpeed > 0) {
				//Debug.Log ("Backthruster angle " + finalAngleForward);
				float speedRot = frontThrusterSpeed * Time.fixedDeltaTime;
				Vector3 force = frontThruster.transform.forward * speedRot * -1;
				rigidbody.AddForceAtPosition (force, frontThruster.transform.position);
			}
			if (backThrusterSpeed > 0) {
				//Debug.Log ("Frontthruster angle: " + finalAngleForward);
				float speedRot = backThrusterSpeed * Time.fixedDeltaTime;
				Vector3 force = backThruster.transform.forward * speedRot * -1;
				//rigidbody.AddRelativeTorque (torque);
				rigidbody.AddForceAtPosition (force, backThruster.transform.position);
			}
			if (rightThrusterSpeed > 0) {
				//Debug.Log ("Rightthruster angle: " + finalAngleRight);
				float speedRot = rightThrusterSpeed * Time.fixedDeltaTime;
				Vector3 force = rightThruster.transform.forward * speedRot * -1;
				//rigidbody.AddRelativeTorque (torque);
				rigidbody.AddForceAtPosition (force, rightThruster.transform.position);
			}
			if (leftThrusterSpeed > 0) {
				//Debug.Log ("Leftthruster angle: " + finalAngleRight);
				float speedRot = leftThrusterSpeed * Time.fixedDeltaTime;
				Vector3 force = leftThruster.transform.forward * speedRot * -1;
				//rigidbody.AddRelativeTorque (torque);
				rigidbody.AddForceAtPosition (force, leftThruster.transform.position);
			}
			if (topThrusterSpeed > 0) {
				//Debug.Log ("Leftthruster angle: " + finalAngleRight);
				float speedRot = topThrusterSpeed * Time.fixedDeltaTime;
				Vector3 force = topThruster.transform.forward * speedRot * -1;
				//rigidbody.AddRelativeTorque (torque);
				rigidbody.AddForce (force);
			}
		}
	}
	
	public Ray GetNozzleRay() {
		return new Ray(jetBone.transform.position, -(jetBone.transform.right));
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (receiver.wiimotes.ContainsKey (1) && Lander.GetInstance().isStarted ()) {	 // If mote 1 is connected
			Wiimote mymote = receiver.wiimotes [1];
			rotateNozzle (-(mymote.PRY_PITCH * Mathf.Rad2Deg - 30), mymote.PRY_ROLL * Mathf.Rad2Deg - 30);
			if (mymote.BUTTON_B > 0) {
				ActivateThruster (mainThruster, mainThrusterSource, ref speed, acceleration, maxSpeed);
			} else {
				DisableThruster (mainThruster, mainThrusterSource, ref speed);
			}
			if (mymote.NUNCHUK_Z > 0 && mymote.NUNCHUK_JOY_X < 0.45) {
				ActivateThruster (rightThruster, rightThrusterSource, ref rightThrusterSpeed, sideThrusterAcceleration, maxSideThrusterSpeed);
			} else {
				DisableThruster (rightThruster, rightThrusterSource, ref rightThrusterSpeed);
			}
			if (mymote.NUNCHUK_Z > 0 && mymote.NUNCHUK_JOY_X > 0.55) {
				ActivateThruster (leftThruster, leftThrusterSource, ref leftThrusterSpeed, sideThrusterAcceleration, maxSideThrusterSpeed);
			} else {
				DisableThruster (leftThruster, leftThrusterSource, ref leftThrusterSpeed);
			}
			if (mymote.NUNCHUK_Z > 0 && mymote.NUNCHUK_JOY_Y > 0.55) {
				ActivateThruster (backThruster, backThrusterSource, ref backThrusterSpeed,
					sideThrusterAcceleration, maxSideThrusterSpeed);
			} else {
				DisableThruster (backThruster, backThrusterSource, ref backThrusterSpeed);
			}
			if (mymote.NUNCHUK_Z > 0 && mymote.NUNCHUK_JOY_Y < 0.45) {
				ActivateThruster (frontThruster, frontThrusterSource, ref frontThrusterSpeed, 
					sideThrusterAcceleration, maxSideThrusterSpeed);
			} else {
				DisableThruster (frontThruster, frontThrusterSource, ref frontThrusterSpeed);
			}
			if (mymote.NUNCHUK_C > 0) {
				ActivateThruster (topThruster, topThrusterSource, ref topThrusterSpeed, 
					 topThrusterAcceleration, maxTopThrusterSpeed);
			} else {
				DisableThruster (topThruster, topThrusterSource, ref topThrusterSpeed);
			}
			if (mymote.BUTTON_LEFT > 0) {
				Lander.GetInstance().SwitchCameraLeft ();
			}
			if (mymote.BUTTON_RIGHT > 0) {
				Lander.GetInstance().SwitchCameraRight ();
			}
			if (mymote.BUTTON_HOME > 0) {
				LandingPadGroup.GetInstance().TogglePause ();
			}
		} else {
			// TODO Problem to display (connect any Wiimote)
		}
	}
	
	public bool IsMainThrusterActive() {
		return speed > 0;
	}
	
	private void ActivateThruster (ParticleSystem particleSystem, AudioSource sound, ref float speedToAdjust, float accelerationVal, float maxSpeedVal)
	{
		// TODO play Sound
		float fuel = Lander.GetInstance().GetFuel ();
		if (fuel > 0) {
			if (!sound.isPlaying) {
				sound.Play ();
			}
			
			if (!particleSystem.isPlaying) {
				particleSystem.Play ();
			}
			speedToAdjust += accelerationVal * Time.deltaTime;
			if (speedToAdjust > maxSpeedVal) {
				speedToAdjust = maxSpeedVal;
			}
			Lander.GetInstance().ReduceFuel (speedToAdjust);
			
		} else {
			DisableThruster (particleSystem, sound, ref speedToAdjust);
		}
	}
	
	private void DisableThruster (ParticleSystem thruster, AudioSource sound, ref float speedToAdjust)
	{
		speedToAdjust = 0;
		if (thruster.isPlaying) {
			thruster.Stop ();
		}
		if (sound.isPlaying) {
			sound.Stop ();
		}
	}
	
	private void addjustRotationAutomatically ()
	{
		// set the rotation back to normal
		//Vector3 newSpin = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, resetRotSpeed * Time.fixedDeltaTime);
		//rigidbody.AddTorque(newSpin);
			
		// rotate towards this axis
		/*if (keepUpRotation) {
				Vector3 worldAxis = Vector3.up;
				Vector3 worldAxisRelative = rigidbody.transform.TransformDirection (worldAxis);
				Quaternion axisAlignRot = Quaternion.FromToRotation (worldAxisRelative, worldAxis);
				rigidbody.rotation = Quaternion.Slerp (transform.rotation, axisAlignRot * rigidbody.transform.rotation, resetRotSpeed * Time.deltaTime);
			} else {
				Quaternion to = Quaternion.AngleAxis (0, Vector3.up) * Quaternion.AngleAxis (0, Vector3.right);
				rigidbody.rotation = Quaternion.Slerp (rigidbody.rotation, to, resetRotSpeed * Time.fixedDeltaTime);
			}*/
//			float yRot = 0;
//			if (keepUpRotation) {
//				yRot = rigidbody.angularVelocity.y;
//			}
//			Vector3 newSpinVelocity = new Vector3 (rigidbody.angularVelocity.x, yRot, rigidbody.angularVelocity.z);
//			rigidbody.angularVelocity = newSpinVelocity;
		// rigidbody.angularVelocity = Vector3.Lerp (rigidbody.angularVelocity, newSpinVelocity, resetRotSpeed * Time.fixedDeltaTime);
		//float x = rigidbody.rotation.eulerAngles.x;
		//float angleVelX = rigidbody.angularVelocity.x;
		//float y = rigidbody.rotation.eulerAngles.y;
		//float angleVelY = rigidbody.angularVelocity.y;
		//float x = rigidbody.angularVelocity.x*Time.fixedDeltaTime;
		//float z = rigidbody.rotation.eulerAngles.z;
		//float angleVelZ = rigidbody.angularVelocity.z;
			
			
		/*if (activatedSideThrusters) {
				resetRotSpeed = 0;
			}*/
		//Vector3 torque = new Vector3(x,0,z);
			
		//torque.Normalize();
		//torque *= -10 * Time.fixedDeltaTime;
		//float z = rigidbody.angularVelocity.z*Time.fixedDeltaTime;
		//print("angleX: " + x + " angleY: " + y + " angleZ: " + z + " AngleSpeed: (" + angleVelX + ", " + angleVelY + ", " + angleVelZ + ")");
		//rigidbody.AddRelativeTorque(torque);
		// the vector that we want to measure an angle from
		Vector3 referenceForward = Vector3.forward;/* some vector that is not Vector3.up */
		Vector3 referenceRight = Vector3.right;/* some vector that is not Vector3.up */

		// the vector perpendicular to referenceForward (90 degrees clockwise)
		// (used to determine if angle is positive or negative)
		Vector3 signRefForward = Vector3.Cross (Vector3.right, referenceForward);
		Vector3 signRefRight = Vector3.Cross (Vector3.forward, referenceRight);
			
		// the vector of interest
		Vector3 currentForward = transform.forward;
		Vector3 currentRight = transform.right;/* some vector that we're interested in */

		// Get the angle in degrees between 0 and 180
		float angleForward = Vector3.Angle (currentForward, Vector3.up);
		float angleRight = Vector3.Angle (currentRight, Vector3.up);

		// Determine if the degree value should be negative.  Here, a positive value
		// from the dot product means that our vector is the right of the reference vector   
		// whereas a negative value means we're on the left.
		float signForward = (Vector3.Dot (currentForward, signRefForward) > 0.0f) ? -1.0f : 1.0f;
		float signRight = (Vector3.Dot (currentRight, signRefRight) > 0.0f) ? -1.0f : 1.0f;

		float finalAngleForward = signForward * angleForward;
		float finalAngleRight = signRight * angleRight;
			
		Debug.Log ("Angle to forward: " + finalAngleForward);
		Debug.Log ("Angle to right: " + finalAngleRight);
			
		bool accelerateRot = 
				finalAngleForward > 90 || finalAngleForward < 90 || finalAngleRight > 90 || finalAngleRight < 90;
			
		if (accelerateRot) {	
			resetRotSpeed += sideThrusterAcceleration;
			if (resetRotSpeed > maxSideThrusterSpeed) {
				resetRotSpeed = maxSideThrusterSpeed;
			}
		}
		//bool activatedSideThrusters = false;
		if (finalAngleForward < 90) {
			frontThruster.Play ();
			//Debug.Log ("Backthruster angle " + finalAngleForward);
			float speedRot = resetRotSpeed * Time.fixedDeltaTime;
			Vector3 force = -frontThruster.transform.forward * speedRot;
			rigidbody.AddForceAtPosition (force, frontThruster.transform.position);
		}
		if (finalAngleForward > 90) {
			backThruster.Play ();
			//Debug.Log ("Frontthruster angle: " + finalAngleForward);
			float speedRot = resetRotSpeed * Time.fixedDeltaTime;
			Vector3 force = -backThruster.transform.forward * speedRot;
			//rigidbody.AddRelativeTorque (torque);
			rigidbody.AddForceAtPosition (force, backThruster.transform.position);
		}
		if (finalAngleRight > 90) {
			rightThruster.Play ();
			//Debug.Log ("Rightthruster angle: " + finalAngleRight);
			float speedRot = resetRotSpeed * Time.fixedDeltaTime;
			Vector3 force = -rightThruster.transform.forward * speedRot;
			//rigidbody.AddRelativeTorque (torque);
			rigidbody.AddForceAtPosition (force, rightThruster.transform.position);
		}
		if (finalAngleRight < 90) {
			leftThruster.Play ();
			//Debug.Log ("Leftthruster angle: " + finalAngleRight);
			float speedRot = finalAngleRight * resetRotSpeed * Time.fixedDeltaTime;
			Vector3 force = -leftThruster.transform.forward * speedRot;
			//rigidbody.AddRelativeTorque (torque);
			rigidbody.AddForceAtPosition (force, leftThruster.transform.position);
		}	
	}
	
	/*void LateUpdate ()
	{
		if (receiver.wiimotes.ContainsKey (1)) {	 // If mote 1 is connected
			Wiimote mymote = receiver.wiimotes [1];
			rotateNozzle (-(mymote.PRY_PITCH * Mathf.Rad2Deg - 30), mymote.PRY_ROLL * Mathf.Rad2Deg - 30);
		}
	}*/
	
	void rotateNozzle (float pitch, float roll)
	{
		jetBone.transform.localEulerAngles = new Vector3 (0, pitch, roll);
	}
	
	void OnGUI ()
	{
		if (receiver.wiimotes.ContainsKey (1)) {	 // If mote 1 is connected
			Wiimote mymote = receiver.wiimotes [1];
			int textWidth = 200;
			if (printDebug) {
				GUI.BeginGroup (new Rect (10, 10, 240, 330));
				GUI.Box (new Rect (0, 0, textWidth, 500), "Wiimote 1:");
				GUI.Label (new Rect (5, 20, textWidth, 20), "Button A: " + mymote.BUTTON_A.ToString ());
				GUI.Label (new Rect (5, 35, textWidth, 20), "Button B: " + mymote.BUTTON_B.ToString ());
				GUI.Label (new Rect (5, 50, textWidth, 20), "Button Left: " + mymote.BUTTON_LEFT.ToString ());
				GUI.Label (new Rect (5, 65, textWidth, 20), "Button Right: " + mymote.BUTTON_RIGHT.ToString ());
				GUI.Label (new Rect (5, 80, textWidth, 20), "Button Up: " + mymote.BUTTON_UP.ToString ());
				GUI.Label (new Rect (5, 95, textWidth, 20), "Button Down: " + mymote.BUTTON_DOWN.ToString ());
				GUI.Label (new Rect (5, 110, textWidth, 20), "Button 1: " + mymote.BUTTON_ONE.ToString ());
				GUI.Label (new Rect (5, 125, textWidth, 20), "Button 2: " + mymote.BUTTON_TWO.ToString ());
				GUI.Label (new Rect (5, 140, textWidth, 20), "Button Plus: " + mymote.BUTTON_PLUS.ToString ());
				GUI.Label (new Rect (5, 155, textWidth, 20), "Button Minus: " + mymote.BUTTON_MINUS.ToString ());
				
				GUI.Label (new Rect (5, 170, textWidth, 20), "Pitch: " + mymote.PRY_PITCH.ToString ());
				GUI.Label (new Rect (5, 185, textWidth, 20), "Pitch Degree: " + (mymote.PRY_PITCH * Mathf.Rad2Deg * 1.5f - 45).ToString ());
				GUI.Label (new Rect (5, 200, textWidth, 20), "Roll: " + mymote.PRY_ROLL.ToString ());
				GUI.Label (new Rect (5, 215, textWidth, 20), "Roll Degree: " + (mymote.PRY_ROLL * Mathf.Rad2Deg * 1.5 - 45).ToString ());
				GUI.Label (new Rect (5, 230, textWidth, 20), "Yaw: " + mymote.PRY_YAW.ToString ());
				GUI.Label (new Rect (5, 245, textWidth, 20), "Yaw Degree: " + (mymote.PRY_YAW * Mathf.Rad2Deg).ToString ());
				GUI.Label (new Rect (5, 260, textWidth, 20), "Accel: " + mymote.PRY_ACCEL.ToString ());
				/*
				GUI.Label(new Rect(5,230,110,20), "Balance Board:");
				GUI.Label(new Rect(5,245,150,20), "Bot Left" + mymote.BALANCE_BOTTOMLEFT.ToString());
				GUI.Label(new Rect(5,260,150,20), "Bot Right" + mymote.BALANCE_BOTTOMRIGHT.ToString());
				GUI.Label(new Rect(5,275,150,20), "Top Left" + mymote.BALANCE_TOPLEFT.ToString());
				GUI.Label(new Rect(5,290,150,20), "Top Right" + mymote.BALANCE_TOPRIGHT.ToString());
				GUI.Label(new Rect(5,305,150,20), "Sum" + mymote.BALANCE_SUM.ToString());
				*/

	
				GUI.Label (new Rect (5, 275, textWidth, 20), "JOY_X: " + mymote.NUNCHUK_JOY_X.ToString ());
				GUI.Label (new Rect (5, 300, textWidth, 20), "JOY_Y: " + mymote.NUNCHUK_JOY_Y.ToString ());
				GUI.EndGroup ();
			}
			connectedIcon.gameObject.active = true;
		} else {
			connectedIcon.gameObject.active = false;
		}
	}
	
	void OnCollisionEnter (Collision collisionInfo)
	{
		/*foreach (ContactPoint contact in collisionInfo.contacts) {
			Debug.Log ("Hit " + contact.otherCollider.name.ToString ());
			Debug.Log ("Hit with Force: " + collisionInfo.impactForceSum.ToString ());
		}*/
		// reduce health on first collision no matter wich part
		float impactForce = collisionInfo.impactForceSum.magnitude;
		if (impactForce >= impactLimit) {			
			gameObject.SendMessage ("ReduceHealth", impactForce * impactMultiplier);
		}
		/*if(collisionInfo.gameObject.name.Equals("foot1")){
			Debug.Log("Hit Foot 1");
		}else if(collisionInfo.gameObject.name.Equals("foot2")){
			Debug.Log("Hit Foot 2");
		}else if(collisionInfo.gameObject.name.Equals("foot3")){
			Debug.Log("Hit Foot 3");
		}else if(collisionInfo.gameObject.name.Equals("foot4")){
			Debug.Log("Hit Foot 4");
		}*/
	}
	
	/*void OnCollisionStay (Collision collisionInfo)
	{
		// reduce health on first collision no matter wich part
		float impactForce = collisionInfo.impactForceSum.magnitude;
		if (impactForce >= impactLimit) {			
			gameObject.SendMessage ("ReduceHealth", impactForce * impactMultiplier);
		}
	}*/
	
}
