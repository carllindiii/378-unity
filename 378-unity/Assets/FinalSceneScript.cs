using UnityEngine;
using System.Collections;

public class FinalSceneScript : MonoBehaviour {
	
	private AudioSource source;
	private Camera camera;
	
	void Awake () {
		source = GetComponent<AudioSource> ();
		camera = GameObject.FindObjectOfType<Camera>();
	}
	
	
	void OnTriggerEnter (Collider coll)
	{
		float fov = camera.fieldOfView;
		camera.fieldOfView += 40;
		camera.depth += 100;
	}
}
