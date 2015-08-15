using UnityEngine;
using System.Collections;

public class PlayerNetworkMover : Photon.MonoBehaviour {

	public delegate void Respawn(float time);
	public event Respawn RespawnMe;

	Vector3 position;
	Quaternion rotation;
	Quaternion camRotation;
	float smoothing = 10f;

	// Use this for initialization
	void Start () {
		if(photonView.isMine){
			GetComponent<FPSController>().enabled = true;
			GetComponent<PlayerShooting>().enabled = true;
			foreach(Camera cam in GetComponentsInChildren<Camera>())
				cam.enabled = true;
		}
		else{
			StartCoroutine("UpdateData");
		}
	}

	IEnumerator UpdateData(){
		while(true){
			transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * smoothing);
			transform.FindChild("Main Camera").rotation = Quaternion.Lerp (transform.FindChild("Main Camera").rotation, camRotation, Time.deltaTime * smoothing);
			yield return null;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(transform.FindChild("Main Camera").rotation);
		}
		else{
			position = (Vector3)stream.ReceiveNext();
			rotation = (Quaternion)stream.ReceiveNext();
			camRotation = (Quaternion)stream.ReceiveNext();
		}
	}
	
	[PunRPC]
	public void GetShot(){
		if(photonView.isMine){
			if(RespawnMe != null)
				RespawnMe(3f);

			PhotonNetwork.Destroy(gameObject);
		}
	}
}
