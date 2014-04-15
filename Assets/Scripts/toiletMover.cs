using UnityEngine;
using System.Collections;

public class toiletMover : MonoBehaviour {
	public GameObject toilet;
	public float moveSpeed = 0.1f;
	public float moveDistance = 1.0f;

	private float leftmostX = 0;
	private float rightmostX = 0;

	// Use this for initialization
	void Start () {
		//Transform t = this.GetComponent<Transform>();
		Transform t = toilet.transform;
		if (t.localScale.x > 0) { // left facing
			rightmostX = t.position.x;
			leftmostX = rightmostX - moveDistance;
		} else {
			leftmostX = t.position.x;
			rightmostX = leftmostX + moveDistance;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (toilet.transform.localScale.x > 0) { // left facing
			if (toilet.transform.position.x <= leftmostX) {
				toilet.transform.localScale = new Vector3(-toilet.transform.localScale.x,
				                                          toilet.transform.localScale.y,
				                                          toilet.transform.localScale.z);
			} else {
				toilet.transform.position = new Vector3(toilet.transform.position.x - moveSpeed, 
			    	                                    toilet.transform.position.y,
			        	                                toilet.transform.position.z);
			}
		}

		else { // right facing
			if (toilet.transform.position.x >= rightmostX) {
				toilet.transform.localScale = new Vector3(-toilet.transform.localScale.x,
				                                          toilet.transform.localScale.y,
				                                          toilet.transform.localScale.z);
			} else {
				toilet.transform.position = new Vector3(toilet.transform.position.x + moveSpeed, 
				                                        toilet.transform.position.y,
				                                        toilet.transform.position.z);
			}
		}
	}
}
