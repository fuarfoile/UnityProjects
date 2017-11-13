using UnityEngine;
using System.Collections;

public class Wheel : Photon.MonoBehaviour {

	public Transform wheel;
	public bool stearing = false;
	public bool switchDirection = false;

	private WheelCollider wheelCollider;

	private float fraction = 0;
	private Vector3 correctWheelPos = Vector3.zero;
	private Quaternion correctWheelRot = Quaternion.identity;

	void Start(){
		wheelCollider = GetComponent<WheelCollider> ();
		if (!photonView.isMine) {
			wheelCollider.enabled = false;
		}
	}

	void Update(){
		if (photonView.isMine) {
			if (wheelCollider.isGrounded) {
				WheelHit hit;
				wheelCollider.GetGroundHit (out hit);
				wheel.localPosition -= Vector3.up * (Vector3.Dot (wheel.transform.position - hit.point, transform.up) - wheelCollider.radius);
			} else {
				wheel.position = transform.position - transform.up * wheelCollider.suspensionDistance;
			}
			wheel.Rotate (wheelCollider.rpm / 60 * 360 * Time.deltaTime * (switchDirection ? -1 : 1), 0, 0);
			if (stearing) {
				wheel.localEulerAngles = new Vector3 (wheel.localEulerAngles.x, wheelCollider.steerAngle - wheel.localEulerAngles.z + (switchDirection ? 180 : 0), wheel.localEulerAngles.z);
			}
		} else {
			fraction += Time.deltaTime * 9;
			wheel.localPosition = Vector3.Lerp(wheel.localPosition, correctWheelPos, fraction);
			wheel.localRotation = Quaternion.Lerp(wheel.localRotation, correctWheelRot, fraction);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = wheel.localPosition;
			Quaternion rot = wheel.localRotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		}
		else
		{
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			
			correctWheelPos = pos;
			correctWheelRot = rot;
			
			fraction = 0; 
		}
	}
}
