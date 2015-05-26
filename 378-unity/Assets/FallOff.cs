using UnityEngine;
using System.Collections;

public class FallOff : MonoBehaviour {
	
	public AudioClip LoseSound;
	private AudioSource source;
	
	void Awake () {
		
		source = GetComponent<AudioSource>();
	}

	/*
	// Not being used anymore
	void OnCollisionEnter (Collision coll)
	{
		source.PlayOneShot(LoseSound, 1.0f);
	}
	*/


	void OnTriggerEnter (Collider coll)
	{
		source.PlayOneShot(LoseSound, 1.0f);
	}
}