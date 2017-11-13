using UnityEngine;
using System.Collections;

public class CarEngine : Photon.MonoBehaviour {

	public float engineMinRPM = 1000;
	public float engineMaxRPM = 7000;
	public AnimationCurve torqueCurve;

	[Space(25)]
	public float rpm = 1000;
	public float horsepower = 0;
	
	private CarController carController;
	private Transmission transmission;

	void Awake () {
		if (!photonView.isMine) {
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		carController = gameObject.GetComponent<CarController> ();
		transmission = gameObject.GetComponent<Transmission> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateRPM ();
	}

	void UpdateRPM(){

		rpm = carController.averageRpm * transmission.differenceCoefficient * transmission.gears [transmission.currentGear];

		if (rpm < engineMinRPM) {
			rpm = engineMinRPM;
		}
		if (rpm > engineMaxRPM) {
			//Do something bad
		}
	}

	//Torque ( rpm )
	public float GetTorque(){
		float result = torqueCurve.Evaluate(rpm);
		if (transmission.currentGear > 1 && Input.GetAxis ("Vertical") > 0) {
			return Mathf.Abs (result * Input.GetAxis ("Vertical"));
		}
		if (transmission.currentGear == 0 && Input.GetAxis ("Vertical") < 0) {
			return Mathf.Abs (result * Input.GetAxis ("Vertical"));
		}
		return 0;
	}
}
