using UnityEngine;
using System.Collections;
/*
namespace UnityStandardAssets.Characters.ThirdPerson {
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class CheckFallen : MonoBehaviour {

		public AudioClip LandingSound;
		private AudioSource source;
		private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object

		// Use this for initialization
		void Awake () {
			m_Character = GetComponent<ThirdPersonCharacter>();
			source = GetComponent<AudioSource>();
		}
		
		void OnCollisionEnter (Collision coll) {
			if (m_Character.getFallen() == false) {
				source.PlayOneShot(LandingSound, 0.5f);
				m_Character.setFallen(true);
			}
		}
	}
}
*/