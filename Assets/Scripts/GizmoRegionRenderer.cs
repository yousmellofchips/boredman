using UnityEngine;
using System.Collections;

public class GizmoRegionRenderer : MonoBehaviour {

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(16f, 10f));
	}
}
