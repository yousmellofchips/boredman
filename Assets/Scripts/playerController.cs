using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

	private const float maxBloat = 2.0f;	// Maximum size for bloat on death
	private const float bloatStep = 0.02f;

	private Animator animator;
	private Rigidbody2D rigidBody;
	private bool jump = false;
	private bool isDying = false;
	private bool isDead = false;
	private bool isCratering = false;
	private int fallOutsideCount = 0;
	private bool grounded = false;			// Whether or not the player is grounded.
	private Transform groundCheckLeft;		// A position marking where to check if the player is grounded.
	private Transform groundCheckRight;

	private int levelCompleteDelay = 0;

	// Use this for initialization
	void Awake () {
		animator = this.GetComponents<Animator>()[0];
		groundCheckLeft = transform.Find("groundCheckLeft");
		groundCheckRight = transform.Find("groundCheckRight");
	}
	
	// Update is called once per frame
	void Update () {
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		int layerNum = LayerMask.NameToLayer("Ground");
		grounded = Physics2D.Linecast(transform.position, groundCheckLeft.position, 1 << layerNum);
		if (!grounded) {
			grounded = Physics2D.Linecast(transform.position, groundCheckRight.position, 1 << layerNum);
		}

		if (Input.GetButtonDown("Fire1") && grounded) {
			jump = true;
		}
	}

	void FixedUpdate() {
		if (levelCompleteDelay > 0) {
			// hang about doing nothing
			if (--levelCompleteDelay == 0) {
				Application.LoadLevel(0);
				GameManager.gameFrozen = false;
			}
			return;
		}

		if (isDead) {
			BloatBloke();
			return;
		}

		if (isDying) {
			animator.SetInteger("Direction", 3);
			rigidbody2D.AddForce(new Vector2(50f, 150.0f));
			rigidbody2D.AddTorque(1.5f);
			rigidbody2D.gravityScale = 2f;
			rigidbody2D.fixedAngle = false;
			BloatBloke();

			Collider2D[] colliders = this.GetComponents<Collider2D>();
			foreach (Collider2D c in colliders) {
				c.isTrigger = true; // turn of stop on collide (probably not the best way??)
			}

			audio.Play();

			isDead = true;
			isDying = false;
			return;
		}

		Vector3 currPos = transform.position;
		float xdelta = 0.0f;

		var horizontal = Input.GetAxis("Horizontal");
		
		if (horizontal > 0)
		{
			animator.SetInteger("Direction", 2);
			if (currPos.x < 6.5) {
				xdelta = 0.05f;
			}
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else if (horizontal < 0)
		{
			animator.SetInteger("Direction", 1);
			if (currPos.x > -6.5) {
				xdelta = -0.05f;
			}
			transform.localScale = new Vector3(-1f, 1f, 1f);
		} else {
			animator.SetInteger("Direction", 0);
		}

		transform.position = new Vector3(currPos.x + xdelta, currPos.y, currPos.z);

		if (jump) {
			rigidbody2D.AddForce(new Vector2(0.0f, 200.0f));
			jump = false;
		}

		// Falling too fast to survive?
		if (rigidbody2D.velocity.y < -10.0f) {
			isCratering = true;
		}

		if (fallOutsideCount > 0) {
			if (--fallOutsideCount == 0) {
				Application.LoadLevel(0);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		bool isPlatform = collision.collider.gameObject.CompareTag("Platform"); // TODO: actually, check for enemy tag.
		if (!isPlatform) {
			isDying = true;
		} else if (isCratering) {
			isDying = true;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.name.CompareTo("Fags") == 0) {
			levelCompleteDelay = 60;
			rigidbody2D.isKinematic = true;
			animator.SetInteger("Direction", 0);
			GameObject obj = GameObject.Find("levelCompleted");
			AudioSource source = obj.GetComponent<AudioSource>();
			audio.PlayOneShot(source.clip);
			GameManager.gameFrozen = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.CompareTag("GameRegion")) {
			if (isDead == false) {
			// player outside of playfield, reset game.
				GameObject obj = GameObject.Find("fallingScream");
				AudioSource source = obj.GetComponent<AudioSource>();
				audio.PlayOneShot(source.clip);
				fallOutsideCount = 80;
			} else {
				Application.LoadLevel(0);
			}
		}
	}

	void BloatBloke() {
		float yscale = transform.localScale.y;
		if (yscale > maxBloat) {
			return;
		}
		yscale += bloatStep;

		float xscale = transform.localScale.x;
		if (xscale > 0) {
			xscale += bloatStep;
		}
		else {
			xscale -= bloatStep;
		}

		transform.localScale = new Vector3(xscale, yscale, 1);
	}
}
