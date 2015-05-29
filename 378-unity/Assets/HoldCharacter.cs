using UnityEngine;
using System.Collections;

public class HoldCharacter : MonoBehaviour {

	/* When the entity collides with this object, 
	 * the entity's movement is attached to this object */
	void OnTriggerEnter(Collider col) {
		Debug.Log ("Entered");
		col.transform.parent = gameObject.transform;
	}

	/* When the entity leaves this object, 
	 * remove the attached relationship to this object */
	void OnTriggerExit(Collider col) {
		Debug.Log ("Exited");
		col.transform.parent = null;
	}
}
