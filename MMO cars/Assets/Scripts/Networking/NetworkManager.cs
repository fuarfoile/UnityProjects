using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class NetworkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PhotonNetwork.sendRate = 25;
		PhotonNetwork.sendRateOnSerialize = 25;
		PhotonNetwork.ConnectUsingSettings("0.1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private const string roomName = "RoomName";
	private RoomInfo[] roomsList;
	
	void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		if (PhotonNetwork.connected) {
			GUILayout.Label (PhotonNetwork.GetPing ().ToString ());
		}
	}
	
	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();
	}

	void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom(null);
	}

	public GameObject playerPrefab;
	
	void OnJoinedRoom()
	{
		// Spawn player
		GameObject car = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
		GameObject camera = GameObject.Find ("Main Camera");
		camera.GetComponent<CameraFollow> ().followTarget = car.transform.FindChild("Helpers").FindChild ("CameraFollowPoint");
		camera.GetComponent<CameraFollow> ().car = car;
		camera.GetComponent<CameraFollow> ().carRigidbody = car.GetComponent<Rigidbody> ();
		camera.GetComponent<CameraFollow> ().carController = car.GetComponent<CarController> ();
	}
}
