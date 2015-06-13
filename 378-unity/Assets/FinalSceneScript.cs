using UnityEngine;
using System.Collections;

public class FinalSceneScript : MonoBehaviour {
	
	private AudioSource source;
	
	void Awake () {
		source = GetComponent<AudioSource>();
	}
	
	
	void OnTriggerEnter (Collider coll)
	{
		//coll.parent.parent.parent.Find<MusicManagerScript>().GetComponent<MusicManagerScript>().MainSong(source.clip);
	}
}
