using UnityEngine;
using System.Collections;

public class GizmoRegionRenderer : MonoBehaviour {

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(new Vector3(0,0,0), new Vector3(13.65f, 10f));
	}
}
