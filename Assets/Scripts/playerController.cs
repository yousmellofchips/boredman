﻿using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

	private const float maxBloat = 2.0f;	// Maximum size for bloat on death
	private const float bloatStep = 0.02f;

	private Animator animator;
	private Rigidbody2D rigidBody;
	private bool jump = false;
	private bool isDying = false;
	private bool isDead = false;
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

	void OnCollisionEnter2D(Collision2D collision) {
		bool isPlatform = collision.collider.gameObject.CompareTag("Platform"); // TODO: actually, check for enemy tag.
		if (!isPlatform) {
			isDying = true;
		}
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.CompareTag("GameRegion")) {
			// player outside of playfield, reset game.
			Application.LoadLevel(0);
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
