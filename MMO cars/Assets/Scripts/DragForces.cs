using UnityEngine;
using System.Collections;

public class DragForces : Photon.MonoBehaviour {

	[Header("Front, side, top")]
	public Vector3 airDrag = Vector3.zero;
	// ~ 30*airDrag
	public float frictionDrag = 0;
	public float downforce = 0; 

	void Awake () {
		if (!photonView.isMine) {
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {

	}

	private float SqrZ(float x){
		if(x >= 0)
			return x * x;
		return -x * x;
	}

	public Vector3 GetDownforce(Vector3 speed, Transform car){
		return -downforce * SqrZ(Vector3.Project(speed, car.forward).magnitude) * car.up;
	}

	public Vector3 GetDragForce (Vector3 speed, Transform car){
		Vector3 resultForce = Vector3.zero;
		resultForce.x = SqrZ(speed.x) * (airDrag.x*Vector3.Project(car.forward, Vector3.right).magnitude   + airDrag.y*Vector3.Project(car.right, Vector3.right).magnitude   + airDrag.z*Vector3.Project(car.up, Vector3.right).magnitude);
		resultForce.y = SqrZ(speed.y) * (airDrag.x*Vector3.Project(car.forward, Vector3.up).magnitude      + airDrag.y*Vector3.Project(car.right, Vector3.up).magnitude      + airDrag.z*Vector3.Project(car.up, Vector3.up).magnitude);
		resultForce.z = SqrZ(speed.z) * (airDrag.x*Vector3.Project(car.forward, Vector3.forward).magnitude + airDrag.y*Vector3.Project(car.right, Vector3.forward).magnitude + airDrag.z*Vector3.Project(car.up, Vector3.forward).magnitude);
		resultForce += speed * frictionDrag;
		return -resultForce;
	}
}
