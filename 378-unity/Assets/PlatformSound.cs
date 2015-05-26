using UnityEngine;
using System.Collections;

public class PlatformSound : MonoBehaviour {

	public AudioClip JetEngine;
	private AudioSource source;
	
	void Awake () {
		
		source = GetComponent<AudioSource>();
	}
	
	
	void OnCollisionEnter (Collision coll)
	{
		source.PlayOneShot(JetEngine, 1.0f);
	}
	
}