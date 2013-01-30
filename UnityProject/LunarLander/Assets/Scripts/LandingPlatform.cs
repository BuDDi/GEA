using UnityEngine;
using System.Collections;

public class LandingPlatform : MonoBehaviour
{
	
	private bool landedBefore = false;
	public bool activated = false;
	public int multiply = 1;
	private float maxFuelRestore = 15;
	
	public float scorePoints = 50;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	void Awake ()
	{
		ParticleSystem particleSys = gameObject.GetComponentInChildren<ParticleSystem> ();
		if (activated && !particleSys.isPlaying) {
			particleSys.Play ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnTriggerEnter ()
	{
		if (activated) {
			ParticleSystem paricleSys = gameObject.GetComponentInChildren<ParticleSystem> ();
			paricleSys.Stop ();
		}
			
	}
	
	void OnTriggerExit ()
	{
		if (activated) {
			ParticleSystem paricleSys = gameObject.GetComponentInChildren<ParticleSystem> ();
			paricleSys.Play ();
		}
	}
	
	void Activate ()
	{
		activated = true;
		ParticleSystem particleSys = gameObject.GetComponentInChildren<ParticleSystem> ();
		particleSys.Play ();
	}
	
	void OnCollisionEnter (Collision collision)
	{
		if (!landedBefore && activated) {
			int amountOfFeet = 0;
			foreach (ContactPoint contact in collision.contacts) {
				//Debug.Log("Stay on Platform " + contact.otherCollider.name.ToString());
				if (contact.otherCollider.name.StartsWith ("foot") || contact.thisCollider.name.StartsWith ("foot")) {
					amountOfFeet++;
				}
			}
			float impact = collision.impactForceSum.magnitude;
			LandingPadGroup.GetInstance().AboutToLand(gameObject, impact, amountOfFeet);
		}
	}
	
	void OnCollisionStay (Collision collision)
	{
		
		if (!landedBefore && activated) {
			//Debug.Log ("Stay, landedbefore: " + landedBefore + " activated: " + activated + " length: " + collision.contacts.Length.ToString ());
			if (collision.contacts.Length == 4) {
				bool allFeet = true;
				float maxDist = 0;
				float minDist = float.MaxValue;
				foreach (ContactPoint contact in collision.contacts) {
					//Debug.Log("Stay on Platform " + contact.otherCollider.name.ToString());
					minDist = Mathf.Min ((contact.point - gameObject.transform.position).magnitude, minDist);
					maxDist = Mathf.Max ((contact.point - gameObject.transform.position).magnitude, maxDist);
					Debug.Log (contact.otherCollider.name + " " + contact.thisCollider.name);
					if (!contact.otherCollider.name.StartsWith ("foot") && !contact.thisCollider.name.StartsWith ("foot")) {
						
						allFeet = false;
						break;
					}
				}
				if (allFeet) {
					
					float averageDist = (minDist + maxDist) / 2;
					landedBefore = true;
					activated = false;
					
					StartCoroutine(LandingPadGroup.GetInstance().LandedOnPlatform(gameObject, averageDist, maxFuelRestore));
				}
			}
		}
	}
	
}
