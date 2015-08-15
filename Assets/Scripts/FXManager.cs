using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {

	public AudioClip[] shots;
	public GameObject bullet;

	public GameObject impactPrefab;
	GameObject[] impacts;
	int currentImpact = 0;
	int maxImpacts = 5;

	// Use this for initialization
	void Start () {
		impacts = new GameObject[maxImpacts];
		for(int i = 0; i < maxImpacts; i++){
		impacts[i] = Instantiate (impactPrefab) as GameObject;
		}
	}

	[PunRPC]
	void SpawnFeedback(Vector3 startPos, Vector3 endPos){
		AudioSource.PlayClipAtPoint(shots[Random.Range(0, shots.Length)], startPos);
		GameObject fx = Instantiate (bullet, startPos, Quaternion.LookRotation(endPos - startPos)) as GameObject;
		LineRenderer lr = fx.GetComponent<LineRenderer>();
		lr.SetPosition(0, startPos);
		lr.SetPosition(1, endPos);
	}

	[PunRPC]
	void SpawnParticleEffect(Vector3 startPos, Vector3 endPos){
		impacts[currentImpact].transform.position = endPos;
		impacts[currentImpact].transform.rotation = Quaternion.LookRotation(startPos - endPos);
		impacts[currentImpact].GetComponent<ParticleSystem>().Play();
		
		if(++currentImpact >= maxImpacts)
			currentImpact = 0;
	}
}
