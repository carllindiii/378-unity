using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	public class PoisonCollision : MonoBehaviour {

		//private GameObject ThePlayer;

		// Use this for initialization
		void Awake () {
			//ThePlayer = GameObject.FindGameObjectWithTag("Player");
		}

		// Update is called once per frame

		void OnParticleCollision(GameObject p) {
			if (p.transform.tag == "Player") {
				p.GetComponent<ThirdPersonCharacter>().PlayerInPoison();
			}
		}
	}
}
