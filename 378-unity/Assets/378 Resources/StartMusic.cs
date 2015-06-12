using UnityEngine;
using System.Collections;

public class StartMusic : MonoBehaviour {

	private AudioSource source;
	
	void Awake () {
		source = GetComponent<AudioSource>();
	}
	
	
	void OnTriggerEnter (Collider coll)
	{
		transform.parent.GetComponent<MusicManagerScript>().MainSong(source.clip);
		Debug.Log("QUEUE MUSIC");
	}
}
