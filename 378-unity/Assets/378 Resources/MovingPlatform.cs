using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	/* INSTANCE VARIABLES */
	public Transform movingPlatform;
	public Transform position1;
	public Transform position2;
	public Vector3 newPosition;
	public string currentState;
	public float smooth;
	public float resetTime;

	// Use this for initialization
	void Start() {
		ChangeTarget();
	}
	
	// FixedUpdate is different from Update in the sense that FixedUpdate remains the same even during Frame Rate drop
	void FixedUpdate() {
		movingPlatform.position = Vector3.Lerp(movingPlatform.position, newPosition, smooth * Time.deltaTime);
	}

	// Change the currentState depending on currentState (if that makes sense)
	void ChangeTarget() {
		if (currentState == "To Position 1") {
			currentState = "To Position 2";
			newPosition = position2.position;
		} else if (currentState == "To Position 2") {
			currentState = "To Position 1";
			newPosition = position1.position;
		} else if (currentState == "") {
			currentState = "To Position 2";
			newPosition = position2.position;
		}
		Invoke("ChangeTarget", resetTime);
	}

}
