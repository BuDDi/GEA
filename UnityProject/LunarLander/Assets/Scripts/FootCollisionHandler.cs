using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class FootCollisionHandler : MonoBehaviour {

	void OnCollisionEnter(Collision collision) {
		Debug.Log("Hit " + collider.name.ToString());
		Debug.Log("Hit with Force: " + collision.impactForceSum.ToString());
        if(collision.gameObject.name.Equals("foot1")){
			Debug.Log("Hit Foot 1");
		}else if(collision.gameObject.name.Equals("foot2")){
			Debug.Log("Hit Foot 2");
		}else if(collision.gameObject.name.Equals("foot3")){
			Debug.Log("Hit Foot 3");
		}else if(collision.gameObject.name.Equals("foot4")){
			Debug.Log("Hit Foot 4");
		}
    }
}
