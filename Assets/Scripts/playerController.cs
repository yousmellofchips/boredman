using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

	private Animator animator;
	private Rigidbody2D rigidBody;
	private bool jump = false;
	private bool grounded = false;			// Whether or not the player is grounded.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.

	// Use this for initialization
	void Awake () {
		animator = this.GetComponents<Animator>()[0];
		groundCheck = transform.Find("groundCheck");
	}
	
	// Update is called once per frame
	void Update () {
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		int layerNum = LayerMask.NameToLayer("Ground");
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << layerNum);  

		if (Input.GetButtonDown("Fire1") && grounded) {
			jump = true;
		}
	}

	void FixedUpdate() {
		float xdelta = 0.0f;

		var horizontal = Input.GetAxis("Horizontal");
		
		if (horizontal > 0)
		{
			animator.SetInteger("Direction", 2);
			xdelta = 0.05f;
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else if (horizontal < 0)
		{
			animator.SetInteger("Direction", 1);
			xdelta = -0.05f;
			transform.localScale = new Vector3(-1f, 1f, 1f);
		} else {
			animator.SetInteger("Direction", 0);
		}

		Vector3 currPos = transform.position;
		transform.position = new Vector3(currPos.x + xdelta, currPos.y, currPos.z);

		if (jump) {
			rigidbody2D.AddForce(new Vector2(0.0f, 200.0f));
			jump = false;
		}
	}
}
