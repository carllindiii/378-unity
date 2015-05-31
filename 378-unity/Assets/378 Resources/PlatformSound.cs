using UnityEngine;
using System.Collections;

public class PlatformSound : MonoBehaviour {

	public AudioClip LandingSound;
	private AudioSource source;
	
	void Awake () {
		
		source = GetComponent<AudioSource>();
	}
	
	
	void OnCollisionEnter (Collision coll)
	{
		source.PlayOneShot(LandingSound, 0.5f);
	}
	
}