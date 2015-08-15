using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {

	CharacterController cc;

	public float hSpeed = 3f;
	public float vSpeed = 5f;
	public float jumpSpeed = 10f;
	public float mouseSpeed = 5f;
	public bool doubleJump = false;

	float rotY = 0f;
	public float clamp = 60f;

	float velocity = 0f;

	public float mass = 3.0f;
	Vector3 impact = Vector3.zero;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		//Rotation
		float rotX = Input.GetAxis("Mouse X") * mouseSpeed;
		rotY -= Input.GetAxis("Mouse Y") * mouseSpeed;
		rotY = Mathf.Clamp (rotY, -clamp, clamp);

		transform.Rotate(0, rotX, 0);
		Camera.main.transform.localRotation = Quaternion.Euler(rotY, 0, 0);

		//Jump
		velocity += Physics.gravity.y * Time.deltaTime;

		if(Input.GetButtonDown("Jump") && cc.isGrounded){
			velocity = jumpSpeed;
			doubleJump = true;
		}
		else if(Input.GetButtonDown("Jump") && !cc.isGrounded && doubleJump){
			velocity = jumpSpeed;
			doubleJump = false;
		}

		//Movement
		float h = Input.GetAxis("Horizontal") * hSpeed;
		float v = Input.GetAxis("Vertical") * vSpeed;
		Vector3 move = transform.rotation * new Vector3(h, velocity, v);

		cc.Move (move * Time.deltaTime);

		//Impact
		if(impact.magnitude > 0.2f)
			cc.Move(impact * Time.deltaTime);

		impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
	}

	[PunRPC]
	public void AddImpact(Vector3 dir, float force){	//Might want to check if we are the "master client"?
		dir.Normalize();
		if(dir.y < 0)
			dir.y *= -1;

		impact += dir.normalized * force / mass;
	}
}
