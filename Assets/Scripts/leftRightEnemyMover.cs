using UnityEngine;
using System.Collections;

public class leftRightEnemyMover : MonoBehaviour {
	public float moveSpeed = 0.1f;
	public float moveDistance = 1.0f;

	private float leftmostX = 0;
	private float rightmostX = 0;


	void OnDrawGizmos() {
		Vector3 moveToPos = transform.position;
		float direction = transform.localScale.x < 0 ? 1 : -1;

		moveToPos.x = moveToPos.x + (direction * moveDistance);

		// Are actually running the game? If so, compensate for current position.
		if (leftmostX != 0 || rightmostX != 0) {
			if (direction == 1) {
				moveToPos.x = Mathf.Min(moveToPos.x, rightmostX);
			} else {
				moveToPos.x = Mathf.Max(moveToPos.x, leftmostX);
			}
		}

		Gizmos.DrawLine(transform.position, moveToPos);
	}

	// Use this for initialization
	void Start () {
		Transform t = transform;
		if (t.localScale.x > 0) { // left facing
			rightmostX = t.position.x;
			leftmostX = rightmostX - moveDistance;
		} else {
			leftmostX = t.position.x;
			rightmostX = leftmostX + moveDistance;
		}
	}

	void FixedUpdate () {
		if (GameManager.Get().GameFrozen)
		{ return; }

		if (transform.localScale.x > 0) { // left facing
			if (transform.position.x <= leftmostX) {
				transform.localScale = new Vector3(-transform.localScale.x,
				                                          transform.localScale.y,
				                                          transform.localScale.z);
			} else {
				transform.position = new Vector3(transform.position.x - moveSpeed, 
			    	                                    transform.position.y,
			        	                                transform.position.z);
			}
		}

		else { // right facing
			if (transform.position.x >= rightmostX) {
				transform.localScale = new Vector3(-transform.localScale.x,
				                                          transform.localScale.y,
				                                          transform.localScale.z);
			} else {
				transform.position = new Vector3(transform.position.x + moveSpeed, 
				                                 transform.position.y,
				                                 transform.position.z);
			}
		}
	}
}
