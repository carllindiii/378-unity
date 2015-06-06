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
	
	// Change to PRESS E to play music (checked in MusicTriggerBox script)
	public void activateMusic()
	{
		transform.parent.GetComponent<MusicManagerScript>().PlaySong(source.clip);
	}
}
