using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	public float timer = 3f;
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if(timer <= 0){
		
			PhotonView pv = GetComponent<PhotonView>();
			if(pv != null && pv.instantiationId != 0){
				PhotonNetwork.Destroy(gameObject);
			}
			else{
				Destroy (gameObject);
			}
		}
	}
}
