using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CarEngine))]
[RequireComponent (typeof (Transmission))]
public class CarController : Photon.MonoBehaviour {

	public enum DriveType {
		Front,
		Rear,
		FourWheel
	}

	public Transform steeringWheel;
	public float steeringWheelMaxAngle = 180;
	public Transform centerOfMass;
	public Material rearLamp;
	public Material frontLamp;
	public GameObject frontLights;

	public DriveType driveType;
	public WheelCollider wheelFrontLeft;
	public WheelCollider wheelFrontRight;
	public WheelCollider wheelRearLeft;
	public WheelCollider wheelRearRight;

	public float maxSteerAngle = 25;
	public float handBrakeTorque = 10000;
	public float brakeTorque = 2500;
	
	[Space(10)]
	[Header("ESC")]
	public bool electronicStabilityControl = true;
	public float electronicStabilityControlCoefficient = 0.1f;

	[Space(25)]
	public float speed = 0;
	public float averageRpm = 0;

	private Rigidbody carRigidbody;
	private CarEngine engine;
	private Transmission transmission;
	private DragForces drag;
	
	private bool lightsIsOn = false;
	private Vector3 startSteeringWheelRotation;

	private Vector3 speedVector = Vector3.zero;

	private Vector3 correctPlayerPos = Vector3.zero;
	private Quaternion correctPlayerRot = Quaternion.identity;

	public Vector3 positionAtLastPacket = Vector3.zero;
	private Quaternion rotationAtLastPacket = Quaternion.identity;
	public float currentTime = 0;
	public float currentPacketTime = 0;
	public float lastPacketTime = 0;
	public float timeToReachGoal = 0;

	public float oldLateralSpeed = 0;
	public float lateralAcceleration = 0;
	public float actualYawRate = 0;
	public float idealYawRate = 0;

	void Start () {
		drag = gameObject.GetComponent<DragForces> ();
		transmission = gameObject.GetComponent<Transmission> ();
		engine = gameObject.GetComponent<CarEngine> (); 
		carRigidbody = gameObject.GetComponent<Rigidbody> ();
		if (!photonView.isMine) {
			carRigidbody.useGravity = false;
			carRigidbody.isKinematic = true;
		}
		carRigidbody.centerOfMass = centerOfMass.localPosition;
		startSteeringWheelRotation = steeringWheel.localEulerAngles;
	}

	void Update(){
		if (photonView.isMine) {

			wheelFrontLeft.steerAngle = Mathf.Lerp (wheelFrontLeft.steerAngle, maxSteerAngle * Input.GetAxis ("Horizontal"), Time.deltaTime * 10);
			wheelFrontRight.steerAngle = Mathf.Lerp (wheelFrontRight.steerAngle, maxSteerAngle * Input.GetAxis ("Horizontal"), Time.deltaTime * 10);

			steeringWheel.localEulerAngles = startSteeringWheelRotation;
			steeringWheel.Rotate(0, wheelFrontLeft.steerAngle / maxSteerAngle * steeringWheelMaxAngle, 0);
			if ((Input.GetAxis ("Vertical") < 0 && transmission.currentGear > 0) || Input.GetButton ("Hand brake") || (Input.GetAxis ("Vertical") > 0 && transmission.currentGear == 0)) {
				rearLamp.mainTextureOffset = new Vector2 (1, 0);
			} else {
				if(lightsIsOn){
					rearLamp.mainTextureOffset = new Vector2 (0.2f, 0);
				}else{
					rearLamp.mainTextureOffset = new Vector2 (0, 0);
				}
			}
			if (Input.GetButtonDown ("Reset")) {
				carRigidbody.velocity = Vector3.zero;
				carRigidbody.angularVelocity = Vector3.zero;
				transform.rotation = Quaternion.identity;
				transform.position = Vector3.up;
			}
			if (Input.GetButtonDown ("Lights")) {
				if(!lightsIsOn){
					frontLamp.mainTextureOffset = new Vector2 (1, 0);
					frontLights.SetActive(true);
					lightsIsOn = true;
				}else{
					frontLamp.mainTextureOffset = new Vector2 (0, 0);
					frontLights.SetActive(false);
					lightsIsOn = false;
				}
			}
		} else {
			timeToReachGoal = currentPacketTime - lastPacketTime;
			currentTime += Time.deltaTime;
			transform.rotation = Quaternion.Lerp(rotationAtLastPacket, correctPlayerRot, (float)(currentTime / timeToReachGoal));

			float timeSinceLastUpdate = (float)(PhotonNetwork.time - currentPacketTime);
			float totalTimePassed = timeSinceLastUpdate;
			Vector3 exterpolatedTargetPosition = correctPlayerPos + speedVector * totalTimePassed;

			Debug.DrawLine(exterpolatedTargetPosition, exterpolatedTargetPosition+Vector3.up*1.5f, Color.blue, 2.5f);

			Vector3 newPosition = Vector3.MoveTowards(transform.position, exterpolatedTargetPosition, speedVector.magnitude * Time.deltaTime);
			Debug.DrawLine(transform.position, exterpolatedTargetPosition + Vector3.up * 0.1f, Color.green, 2.5f);
			if (Vector3.Distance(transform.position, exterpolatedTargetPosition) > 2f) {
				newPosition = exterpolatedTargetPosition;
			}
			transform.position = newPosition;
			Debug.DrawLine(transform.position, transform.position+Vector3.up, Color.white, 2.5f);
		}
	}

	void FixedUpdate () {
		if (photonView.isMine) {

			averageRpm = 0;

			if(driveType != DriveType.Rear){
				wheelFrontLeft.motorTorque = engine.GetTorque () * transmission.differenceCoefficient * transmission.driveEfficiency * transmission.gears [transmission.currentGear] / (driveType == DriveType.FourWheel ? 4:2);
				wheelFrontRight.motorTorque = engine.GetTorque () * transmission.differenceCoefficient * transmission.driveEfficiency * transmission.gears [transmission.currentGear] / (driveType == DriveType.FourWheel ? 4:2);
				averageRpm = (wheelFrontLeft.rpm + wheelFrontRight.rpm) / 2;
			}
			if(driveType != DriveType.Front){
				wheelRearLeft.motorTorque = engine.GetTorque () * transmission.differenceCoefficient * transmission.driveEfficiency * transmission.gears [transmission.currentGear] / (driveType == DriveType.FourWheel ? 4:2);
				wheelRearRight.motorTorque = engine.GetTorque () * transmission.differenceCoefficient * transmission.driveEfficiency * transmission.gears [transmission.currentGear] / (driveType == DriveType.FourWheel ? 4:2);
				averageRpm += (wheelFrontLeft.rpm + wheelFrontRight.rpm) / 2;
				if(driveType == DriveType.FourWheel){
					averageRpm *= 0.5f;
				}
			}

			engine.horsepower = (engine.GetTorque () * engine.rpm) / 5252;

			if((Input.GetAxis ("Vertical") < 0 && transmission.currentGear > 0) || (Input.GetAxis ("Vertical") > 0 && transmission.currentGear == 0)){
				wheelFrontLeft.brakeTorque = brakeTorque;
				wheelFrontRight.brakeTorque = brakeTorque;
				wheelRearLeft.brakeTorque = brakeTorque;
				wheelRearRight.brakeTorque = brakeTorque;
			}else{
				wheelFrontLeft.brakeTorque = 0;
				wheelFrontRight.brakeTorque = 0;
				if (Input.GetButton ("Hand brake")) {
					wheelRearLeft.brakeTorque = handBrakeTorque;
					wheelRearRight.brakeTorque = handBrakeTorque;
				}else{
					wheelRearLeft.brakeTorque = 0;
					wheelRearRight.brakeTorque = 0;
				}
			}

			if(electronicStabilityControl && speed > 10){
				lateralAcceleration = ( Vector3.Project ( carRigidbody.velocity, transform.right ).magnitude - oldLateralSpeed) / Time.fixedDeltaTime;
				oldLateralSpeed = Vector3.Project ( carRigidbody.velocity, transform.right ).magnitude;
				actualYawRate = wheelFrontLeft.steerAngle * carRigidbody.angularVelocity.y / 180 * Mathf.PI;
				idealYawRate = lateralAcceleration / Vector3.Project ( carRigidbody.velocity, transform.forward ).magnitude;
				float escOverSteerBrakeTorque = -electronicStabilityControlCoefficient * (actualYawRate - idealYawRate);

				if(escOverSteerBrakeTorque > 0){
					if(carRigidbody.angularVelocity.y > 0){
						Debug.DrawRay(wheelFrontLeft.transform.position, -wheelFrontLeft.transform.right * escOverSteerBrakeTorque, Color.red);
						wheelFrontLeft.brakeTorque += escOverSteerBrakeTorque;
					}else{
						Debug.DrawRay(wheelFrontRight.transform.position, wheelFrontRight.transform.right * escOverSteerBrakeTorque, Color.red);
						wheelFrontRight.brakeTorque += escOverSteerBrakeTorque;
					}
				} else {
					if(carRigidbody.angularVelocity.y < 0){
						Debug.DrawRay(wheelRearLeft.transform.position, wheelRearLeft.transform.right * escOverSteerBrakeTorque, Color.red);
						wheelRearLeft.brakeTorque -= escOverSteerBrakeTorque;
					}else{
						Debug.DrawRay(wheelRearRight.transform.position, -wheelRearRight.transform.right * escOverSteerBrakeTorque, Color.red);
						wheelRearRight.brakeTorque -= escOverSteerBrakeTorque;
					}
				}
			}
			
			Debug.DrawRay (transform.position, drag.GetDragForce (carRigidbody.velocity, transform), Color.white);
			Debug.DrawRay (transform.position, drag.GetDownforce (carRigidbody.velocity, transform), Color.blue);
			carRigidbody.AddForceAtPosition (drag.GetDragForce (carRigidbody.velocity, transform), centerOfMass.position, ForceMode.Force);
			carRigidbody.AddForceAtPosition (drag.GetDownforce (carRigidbody.velocity, transform), centerOfMass.position, ForceMode.Force);
			speed = carRigidbody.velocity.magnitude * 3.6f;
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			Vector3 sp = carRigidbody.velocity;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref sp);
		}
		else
		{
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			Vector3 sp = Vector3.zero;
			
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref sp);
			
			correctPlayerPos = pos;
			correctPlayerRot = rot;
			speedVector = sp;

			Debug.DrawLine(pos, pos+Vector3.up*2, Color.red, 2.5f);

			currentTime = 0;
			positionAtLastPacket = transform.position;
			rotationAtLastPacket = transform.rotation;
			lastPacketTime = currentPacketTime;
			currentPacketTime = (float)info.timestamp;
		}
	}
}
