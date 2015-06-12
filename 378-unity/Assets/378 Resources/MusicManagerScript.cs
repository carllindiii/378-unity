using UnityEngine;
using System.Collections;

public class MusicManagerScript : MonoBehaviour {

	private AudioSource source;

	private float defaultVolume;
	private float AudioVolume;
	
	public AudioClip Main_Song;
	public AudioClip nextSong;
	bool startPlaying = false;
	bool FadeOut = false;

	// Use this for initialization
	void Awake () {
		source = GetComponent<AudioSource>();
		defaultVolume = source.volume;
		AudioVolume = defaultVolume;

		// Transfers to next scene, if the other scene has a music manager.
		GameObject music_manager = GameObject.Find ("Music Manager");

		// even though DontDestroyOnLoad is present....it still destroys on load :(

//		if (music_manager != null) {
//			// makes sure that there is no duplicate sound.
//			Destroy(music_manager);
//		}
//		DontDestroyOnLoad (gameObject);
	}

	void Update() {
		if (startPlaying && !source.isPlaying) {
			source.clip = Main_Song;
			source.Play(0);
		}

		if (FadeOut) {
			FadeOutMusic();
		}
	}

	public void PlaySong(AudioClip song) {
		if (source.isPlaying) {
			FadeOut = true;
		}

		nextSong = song;
	}

	public void FadeOutMusic() {
		if (AudioVolume > 0) {
			AudioVolume -= 0.1f * Time.deltaTime;
			source.volume = AudioVolume;
		} else {
			FadeOut = false;
			source.Stop ();
			source.clip = nextSong;
			source.volume = defaultVolume;
			AudioVolume = defaultVolume;

			source.Play (0);
		}
	}

	public void MainSong(AudioClip main) {
		if (source.isPlaying && main != Main_Song) {
			// another song is playing, fade out and play new main song
			FadeOut = true;
			nextSong = main;
			Main_Song = main;
		} else {
			Main_Song = main; 
		}
		startPlaying = true;
	}
}
