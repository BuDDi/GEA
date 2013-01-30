using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class Keyboard : MonoBehaviour
{
	
	public float maxSpeed;
	public float acceleration;
	private float speed;
	public float jetRotSpeed;
	public float maxRotX;
	public float minRotX;
	public float maxRotZ;
	public float minRotZ;
	private float currentAngleX;
	private float currentAngleZ;
	private Vector3 direction = Vector3.up;
	private WiimoteReceiver receiver;

	// Use this for initialization
	void Start ()
	{
		receiver = WiimoteReceiver.Instance;
		receiver.connect ();
	}
	
	void FixedUpdate ()
	{
		rigidbody.AddForce (direction * speed * Time.fixedDeltaTime);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (receiver.wiimotes.ContainsKey (1)) {	 // If mote 1 is connected
			Debug.Log ("found wiimote");
			Wiimote mymote = receiver.wiimotes [1];
			if (mymote.BUTTON_B > 0) {
				speed += acceleration * Time.deltaTime;
				if (speed > maxSpeed) {
					speed = maxSpeed;	
				}
			} else {
				speed = 0;
			}
			
			if(mymote.BUTTON_LEFT>0){
				gameObject.SendMessage("switchCameraLeft");
			}
			if(mymote.BUTTON_RIGHT>0){
				gameObject.SendMessage("switchCameraRight");
			}
			//GUI.DrawTexture(new Rect(mymote.IR_X * Screen.width, (1-mymote.IR_Y)* Screen.height,32,32), cursorOne);
		}
	
		//currentAngleX += jetRotSpeed * Time.deltaTime * Input.GetAxis ("Vertical");
		//Vector3 xAxis = Vector3.right;
		//Quaternion quatX = Quaternion.AngleAxis (currentAngleX, xAxis);
		//direction = direction * quatX;
		
		
		//currentAngleY += jetRotSpeed * Time.deltaTime * Input.GetAxis ("Horizontal");
		//Vector3 zAxis = Vector3.forward;
		//Quaternion quatZ = Quaternion.AngleAxis (currentAngleZ, zAxis);
		//direction = direction * quatZ;
		
	}
	
	void OnGUI ()
	{
		if (receiver.wiimotes.ContainsKey (1)) {	 // If mote 1 is connected
			Wiimote mymote = receiver.wiimotes [1];
			GUI.BeginGroup (new Rect (10, 10, 150, 330));
			GUI.Box (new Rect (0, 0, 110, 500), "Wiimote 1:");
			GUI.Label (new Rect (5, 20, 110, 20), "Button A: " + mymote.BUTTON_A.ToString ());
			GUI.Label (new Rect (5, 35, 110, 20), "Button B: " + mymote.BUTTON_B.ToString ());
			GUI.Label (new Rect (5, 50, 110, 20), "Button Left: " + mymote.BUTTON_LEFT.ToString ());
			GUI.Label (new Rect (5, 65, 110, 20), "Button Right: " + mymote.BUTTON_RIGHT.ToString ());
			GUI.Label (new Rect (5, 80, 110, 20), "Button Up: " + mymote.BUTTON_UP.ToString ());
			GUI.Label (new Rect (5, 95, 110, 20), "Button Down: " + mymote.BUTTON_DOWN.ToString ());
			GUI.Label (new Rect (5, 110, 110, 20), "Button 1: " + mymote.BUTTON_ONE.ToString ());
			GUI.Label (new Rect (5, 125, 110, 20), "Button 2: " + mymote.BUTTON_TWO.ToString ());
			GUI.Label (new Rect (5, 140, 110, 20), "Button Plus: " + mymote.BUTTON_PLUS.ToString ());
			GUI.Label (new Rect (5, 155, 110, 20), "Button Minus: " + mymote.BUTTON_MINUS.ToString ());
				
			GUI.Label (new Rect (5, 170, 110, 20), "Pitch" + mymote.PRY_PITCH.ToString ());
			GUI.Label (new Rect (5, 185, 110, 20), "Roll" + mymote.PRY_ROLL.ToString ());
			GUI.Label (new Rect (5, 200, 110, 20), "Yaw" + mymote.PRY_YAW.ToString ());
			GUI.Label (new Rect (5, 215, 110, 20), "Accel" + mymote.PRY_ACCEL.ToString ());
			/*
				GUI.Label(new Rect(5,230,110,20), "Balance Board:");
				GUI.Label(new Rect(5,245,150,20), "Bot Left" + mymote.BALANCE_BOTTOMLEFT.ToString());
				GUI.Label(new Rect(5,260,150,20), "Bot Right" + mymote.BALANCE_BOTTOMRIGHT.ToString());
				GUI.Label(new Rect(5,275,150,20), "Top Left" + mymote.BALANCE_TOPLEFT.ToString());
				GUI.Label(new Rect(5,290,150,20), "Top Right" + mymote.BALANCE_TOPRIGHT.ToString());
				GUI.Label(new Rect(5,305,150,20), "Sum" + mymote.BALANCE_SUM.ToString());
				*/

	
			GUI.Label (new Rect (5, 230, 110, 20), "JOY_X: " + mymote.NUNCHUK_JOY_X.ToString ());
			GUI.Label (new Rect (5, 245, 110, 20), "JOY_Y: " + mymote.NUNCHUK_JOY_Y.ToString ());
			GUI.EndGroup ();
		}
	}
}
