using UnityEngine;
using System.Collections;

public class AntiRollBar : Photon.MonoBehaviour {

	public WheelCollider firstWheel;
	public WheelCollider secondWheel;
	public float antiRoll = 5000.0f;

	private Rigidbody carRigidbody;

	void Awake () {
		if (!photonView.isMine) {
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		carRigidbody = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		WheelHit hit = new WheelHit();
		float firstTravel = 1.0f;
		float secondTravel = 1.0f;

		if (firstWheel.GetGroundHit (out hit)) {
			firstTravel = (-firstWheel.transform.InverseTransformPoint(hit.point).y - firstWheel.radius) / firstWheel.suspensionDistance;
		}
		if (secondWheel.GetGroundHit (out hit)) {
			secondTravel = (-secondWheel.transform.InverseTransformPoint(hit.point).y - secondWheel.radius) / secondWheel.suspensionDistance;
		}

		float antiRollForce = (firstTravel - secondTravel) * antiRoll;

		carRigidbody.AddForceAtPosition (-antiRollForce * firstWheel.transform.up, firstWheel.transform.position);
		carRigidbody.AddForceAtPosition (antiRollForce * secondWheel.transform.up, secondWheel.transform.position);

		Debug.DrawRay (firstWheel.transform.position, -antiRollForce * firstWheel.transform.up / antiRoll * 10, Color.yellow);
		Debug.DrawRay (secondWheel.transform.position, antiRollForce * secondWheel.transform.up / antiRoll * 10, Color.yellow);
	}
}
