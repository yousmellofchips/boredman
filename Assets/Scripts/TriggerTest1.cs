using UnityEngine;
using System.Collections;

public class TriggerTest1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider) {
		Debug.Log("Entering... " + collider.gameObject.name);
	}

	void OnTriggerExit2D(Collider2D collider) {
		Debug.Log("Exiting... " + collider.gameObject.name);
	}
}
