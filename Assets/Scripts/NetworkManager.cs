using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour{

	[SerializeField] Text connectionText;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] Camera sceneCamera;
	[SerializeField] string version = "0.1";

	[SerializeField] GameObject serverWindow;
	[SerializeField] InputField username;
	[SerializeField] InputField roomName;
	[SerializeField] InputField roomList;

	GameObject player;
	public bool started = false;

	// Use this for initialization
	void Start() {
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings(version);
		StartCoroutine("UpdateConnectionString");
	}

	// Update is called once per frame
	IEnumerator UpdateConnectionString(){
		while(true){
			connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
			yield return null;
		}
	}

	void OnJoinedLobby(){
		serverWindow.SetActive (true);
	}

	void OnReceivedRoomListUpdate(){
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		foreach(RoomInfo room in rooms){
			roomList.text += room.name + "\n";
		}
	}

	public void JoinRoom(){
		PhotonNetwork.player.name = username.text;
		RoomOptions ro = new RoomOptions(){isVisible = true, maxPlayers = 4};
		PhotonNetwork.JoinOrCreateRoom(roomName.text, ro, TypedLobby.Default);
	}

	void OnJoinedRoom(){
		serverWindow.SetActive(false);
		StartSpawnProcess(0f);
		StopCoroutine("UpdateConnectionString");
		connectionText.text = "";

		if(PhotonNetwork.isMasterClient)
			PhotonNetwork.InstantiateSceneObject("Minion", spawnPoints[1].position, spawnPoints[1].rotation, 0, null);	//Test 
	}

	void OnPhotonPlayerConnected(PhotonPlayer other){
		if(PhotonNetwork.playerList.Length > 2)
			started = true;
	}

	void StartSpawnProcess(float respawnTime){
		sceneCamera.enabled = true;
		StartCoroutine("SpawnPlayer", respawnTime);
	}

	IEnumerator SpawnPlayer(float respawnTime){
		yield return new WaitForSeconds(respawnTime);

		int index = Random.Range(0, spawnPoints.Length);
		player = PhotonNetwork.Instantiate("FPSPlayer", spawnPoints[index].position, spawnPoints[index].rotation, 0, null);
		player.GetComponent<PlayerNetworkMover>().RespawnMe += StartSpawnProcess;
		sceneCamera.enabled = false;
	}

	IEnumerator Waves(float timer){
		//Spawn minionswaves in two lanes at intervals
		return null;
	}
}
