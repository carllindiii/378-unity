using UnityEngine;
using System.Collections;

public class RobotText : MonoBehaviour {

	public TextMesh robotText;

	void Awake() {
		robotText = (TextMesh)GetComponent(typeof(TextMesh));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
