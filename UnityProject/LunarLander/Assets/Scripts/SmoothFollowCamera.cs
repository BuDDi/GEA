using UnityEngine;
using System.Collections;

public class SmoothFollowCamera : MonoBehaviour
{
	public Rigidbody target;
	//public float damping = 1;
	public float smoothTime = 0.4f;
	//public float maxSpeed;
	Vector3 offset;
	private Vector3 velocity;
    
	void Start ()
	{
		offset = transform.position - target.position;
		velocity = Vector3.zero;
	}
	
	void FixedUpdate ()
	{
		if (target != null) {
			Vector3 desiredPosition = target.position + offset;
			Vector3 position = Vector3.SmoothDamp (transform.position, desiredPosition, ref velocity, smoothTime);
			//Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * damping);
			transform.position = position;
			//transform.LookAt(target.transform.position);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, target.rotation.eulerAngles.y, transform.localEulerAngles.z);
		}
	}
}