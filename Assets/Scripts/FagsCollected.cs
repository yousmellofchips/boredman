using UnityEngine;
using System.Collections;

public class FagsCollected : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.name.CompareTo("bloke") == 0) {
		}
	}
}
