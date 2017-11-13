using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraFollow : MonoBehaviour {

	public GameObject car;
	public Transform followTarget;
	public float height = 1;
	public float heightSpeedFactor = 0.01f;
	public float maxHeight = 2;
	public float distance = 4;
	public float distanceFactor = -0.01f;
	public float minDistance = 2;
	public float drag = 10;
	public float dragFactor = 0.05f;
	public float lookOrientationOffset = 0.1f;
	public float fov = 60;
	public float fovFactor = 0.05f; 
	public float maxFov = 120;

	public float vignetteFactor = 0.01f;
	public float aberrationFactor = 0.01f;

	[HideInInspector]
	public Rigidbody carRigidbody;
	[HideInInspector]
	public CarController carController;
	private bool lookForward = true;
	private VignetteAndChromaticAberration aberration;
	private Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
		fov = camera.fieldOfView;
		aberration = GetComponent<VignetteAndChromaticAberration> ();
	}

	void Update (){
		if (car != null) {
			if (Vector3.Dot (car.transform.forward, carRigidbody.velocity) > lookOrientationOffset && Input.GetAxis("Vertical") >= 0) {
				lookForward = true;
			}
			if (Vector3.Dot (car.transform.forward, carRigidbody.velocity) < -lookOrientationOffset && Input.GetAxis("Vertical") <= 0) {
				lookForward = false;
			}
			camera.fieldOfView = Mathf.Min(maxFov, fov + fovFactor * carController.speed);
			aberration.intensity = carController.speed * vignetteFactor;
			aberration.chromaticAberration = carController.speed * aberrationFactor + 0.1f;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (followTarget != null) {
			transform.position = Vector3.Lerp (transform.position, followTarget.position - followTarget.forward * Mathf.Max (minDistance, distance + distanceFactor*carController.speed) * (lookForward ? 1:-1) + Vector3.up * (height + Mathf.Min(heightSpeedFactor * carController.speed, maxHeight-height)), Time.deltaTime * (drag + dragFactor * carController.speed));
			transform.LookAt (followTarget);
		}
	}
}
