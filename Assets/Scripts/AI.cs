using UnityEngine;
using System.Collections;

public class AI : Photon.MonoBehaviour {

	public float moveSpeed = 5f;
	public float distance = 0.2f;
	public GameObject[] checkpoints;
	int counter = 0;
	Vector3 targetPosition;

	CharacterController cc;

	public int id = 0;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}

	#region Movement

	// Update is called once per frame
	void Update () {
		if(checkpoints.Length > 1){
			Vector3 direction = Vector3.zero;
			direction = UpdateCheckPoint();

			if(direction.magnitude < distance){
				if(counter < checkpoints.Length - 1){
					counter++;
				}
				else
					Destroy(gameObject);

			}
			direction = direction.normalized;
			Vector3 dir = direction * Time.deltaTime * moveSpeed;
			cc.Move (new Vector3(dir.x, transform.position.y, dir.z));
		}
	}

	Vector3 UpdateCheckPoint(){
		Vector3 direction = checkpoints[counter].transform.position - transform.position;
		return direction;
	}

	#endregion

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			//Do something fun!
		}
	}

	[PunRPC]
	public void GetShot(){
		if(PhotonNetwork.isMasterClient){
			PhotonNetwork.Destroy(gameObject);
		}
	}
}
