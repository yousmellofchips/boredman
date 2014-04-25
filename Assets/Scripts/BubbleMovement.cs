using UnityEngine;
using System.Collections;

public class BubbleMovement : MonoBehaviour {

	public float moveSpeed = 0.1f;
	public float moveDistance = 15.0f; // before bubble bursts (and resets position)

	private Vector3 startPosition;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameManager.Get().GameFrozen)
		{ return; }

		transform.position = new Vector3(transform.position.x, 
		                                 transform.position.y + moveSpeed,
		                                 transform.position.z);

		if (transform.position.y - startPosition.y >= moveDistance) {
			// Burst, reset.
			// TODO: play POP sound
			transform.position = startPosition;
		}

		// TODO: scale variations for flexing bubble?
	}
}
