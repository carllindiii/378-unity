using UnityEngine;
using System.Collections;

public class TriggerMusic : MonoBehaviour {
	
	//GameObject manager; // Reference to parent music manager
	private AudioSource source;
	public bool playing;
	
	void Awake () {
		source = GetComponent<AudioSource>();
		//manager = transform.parent.GetComponent<MusicManagerScript>();
		playing = false;
	}
	
	// Change to PRESS E to play music
	void OnCollisionEnter (Collision coll)
	{
		if (playing == false) {
			transform.parent.GetComponent<MusicManagerScript>().PlaySong(source.clip);
			playing = true;
		}
	}
}
