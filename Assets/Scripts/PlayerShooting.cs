using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

	Animator anim;

	public GameObject firePoint;
	public float fireRate = 0.5f;
	float timer = 0f;

	//public GameObject impactPrefab;
	//GameObject[] impacts;
	//int currentImpact = 0;
	//int maxImpacts = 5;

	[HideInInspector] public bool shooting = false;
	public Color[] colors = new Color[10];

	FXManager fx;

	// Use this for initialization
	void Start () {
		//impacts = new GameObject[maxImpacts];
		//for(int i = 0; i < maxImpacts; i++){
			//impacts[i] = Instantiate (impactPrefab) as GameObject;
		//}

		anim = GetComponentInChildren<Animator>();
		fx = GameObject.FindObjectOfType<FXManager>();

		if(fx == null)
			Debug.LogError("No FXManager!");
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if(Input.GetButtonDown("Fire1") && timer <= 0){
			anim.SetTrigger("Fire");
			//Shake shake = GetComponentInChildren<Shake>();
			//shake.ShakeCamera(0.1f, 0.3f);
			shooting = true;
			timer = fireRate;
		}
	}

	void FixedUpdate(){
		if(shooting){
			shooting = false;

			GetComponentInChildren<ParticleSystem>().Play();

			RaycastHit hit;
			if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50f)){
				if(hit.transform.tag == "Player"){
					hit.transform.GetComponent<PhotonView>().RPC("GetShot", PhotonTargets.All);
					hit.transform.GetComponent<PhotonView>().RPC("AddImpact", PhotonTargets.All, hit.transform.position - transform.position, 100f);
				}
				else if(hit.transform.tag == "Minion"){
					hit.transform.GetComponent<PhotonView>().RPC("GetShot", PhotonTargets.All);
				}

				StartCoroutine("Feedback", hit);

				//impacts[currentImpact].transform.position = hit.point;
				//impacts[currentImpact].GetComponent<ParticleSystem>().Play();

				//if(++currentImpact >= maxImpacts)
					//currentImpact = 0;
				if(fx != null){
					fx.GetComponent<PhotonView>().RPC("SpawnFeedback", PhotonTargets.All, firePoint.transform.position, hit.point);
					fx.GetComponent<PhotonView>().RPC ("SpawnParticleEffect", PhotonTargets.All, firePoint.transform.position, hit.point);
				}
				//StartCoroutine("Sleep");
			}
			else{
				Vector3 point = Camera.main.transform.position + (Camera.main.transform.forward * 50f);
				fx.GetComponent<PhotonView>().RPC("SpawnFeedback", PhotonTargets.All, firePoint.transform.position, point);
				//StartCoroutine("Sleep");
			}
		}
	}

	[PunRPC]
	IEnumerator Feedback(RaycastHit info){
		Renderer rend = info.transform.GetComponent<Renderer>();
		//Color normal = Color.white;
		Color feedback = colors[Random.Range(0, colors.Length)];
		if(rend.material.color != feedback){
			rend.material.color = feedback;
			//yield return new WaitForSeconds(0.1f);
			//rend.material.color = normal;
		}
		else
			yield return null;
	}
//
//	IEnumerator Sleep(){
//		Time.timeScale = 1f;
//		Time.fixedDeltaTime = 1f;
//		yield return new WaitForSeconds(0.001f);
//		Time.timeScale = 1f;
//		Time.fixedDeltaTime = 1f;
//	}
}

