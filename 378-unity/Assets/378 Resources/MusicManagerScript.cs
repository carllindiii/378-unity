using UnityEngine;
using System.Collections;

public class MusicManagerScript : MonoBehaviour {

	private AudioSource source;
	public AudioClip Main_Song;
	bool startPlaying = false;

	// Use this for initialization
	void Awake () {
		source = GetComponent<AudioSource>();
	}

	void Update() {
		if (startPlaying && !source.isPlaying) {
			source.clip = Main_Song;
			source.Play(0);
		}
	}

	public void PlaySong(AudioClip song) {
		if (source.isPlaying) {
			source.Stop();
		}

		source.clip = song;
		source.Play(0);
	}

	public void MainSong(AudioClip main) {
		startPlaying = true;
		Main_Song = main; 
	}
}
