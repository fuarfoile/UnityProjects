using UnityEngine;
using System.Collections;

public class Transmission : Photon.MonoBehaviour {

	public bool automatic = true;
	public float[] gears;
	public float[] gearsSpeedToShift;
	public float speedOffset = 5;
	public float shiftTime = 0.5f;
	public float differenceCoefficient = 3.42f;
	[Range(0, 1)]
	public float driveEfficiency = 0.7f;

	[Space(20)]
	public byte currentGear = 1;
	public bool shifting = false;


	private byte runtimeGear = 0;
	private CarController carController;
	private Rigidbody carRigidbody;

	IEnumerator ShiftDelay(){
		yield return new WaitForSeconds(shiftTime);
		currentGear = runtimeGear;
		shifting = false;
	}

	void Awake () {
		if (!photonView.isMine) {
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		carController = gameObject.GetComponent<CarController> ();
		carRigidbody = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!shifting) {
			if (automatic) {
				if (currentGear > 0 && carController.speed * Vector3.Dot (carRigidbody.velocity.normalized, transform.forward) > gearsSpeedToShift [currentGear + 1] + speedOffset) {
					UpShift ();
				}
				if (currentGear > 1 && carController.speed * Vector3.Dot (carRigidbody.velocity.normalized, transform.forward) < gearsSpeedToShift [currentGear] - speedOffset) {
					if(Input.GetAxis ("Vertical") <= 0 || currentGear != 2){
						DownShift ();
					}
				}
				if ((currentGear == 0 || currentGear == 1) && Input.GetAxis ("Vertical") > 0 && -carController.speed * Vector3.Dot (carRigidbody.velocity.normalized, transform.forward) < gearsSpeedToShift [0]) {
					UpShift ();
				}
				if (currentGear == 1 && Input.GetAxis ("Vertical") < 0 && carController.speed * Vector3.Dot (carRigidbody.velocity.normalized, transform.forward) < gearsSpeedToShift [0]) {
					DownShift ();
				}
				if(shifting){
					currentGear = 1;
				}
			}
		}
	}

	void UpShift(){
		if (currentGear != 1) {
			runtimeGear = currentGear;
			runtimeGear++;
			shifting = true;
			StartCoroutine ("ShiftDelay");
		} else {
			currentGear++;
		}
	}

	void DownShift(){
		if (currentGear != 1) {
			runtimeGear = currentGear;
			runtimeGear--;
			shifting = true;
			StartCoroutine ("ShiftDelay");
		} else {
			currentGear--;
		}
	}
}
