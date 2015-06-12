using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;

		const float Fall_Trigger = 15; // Value of how far down you fall before sound plays
		float FallDistance; // Used to keep track of fall distance
		float footstepTimer = 0; // Timer used for footsteps
		float footstepSpeed; // Time before next footstep plays (lower => more frequent footsteps)

		public AudioClip CoughingSound; // Sound to play if in the poison gas
		public AudioClip FallSound; // Sound to play if fell from high altitude (can be changed)
		public AudioClip LandSound; // Sound to play when landing (not from high altitude)
		public AudioClip LandTerrainSound; // Sound to play when landing on terrain
		public AudioClip FootstepSound; // Sound to play during move animation for footsteps
		private AudioSource source;

		private bool LandPlatform = false;
		private bool LandTerrain = false;

		// Checkpoint stuff
		public Vector3 Checkpoint_Position;
		public int current_checkpoint;

		// Health System
		public float health = 100f;
		public float delayAfterDeath = 5f;
		public float deathTimer = 0;
		public AudioClip DeathSound;
		private bool playerDead = false;
		public Slider HealthSlider;
		private bool hurt = false;

		public Image DamageScreenFlash;
		public float flashSpeed = 0.5f;
		public Color flashColor_Red = new Color(1f, 0f, 0f, 0.5f);
		public Color flashColor_Green = new Color (0f, 1f, 0f, 0.5f);
		public Color checkpointEnterColor = new Color(0f, 0f, 1f, 0.5f);

		private bool inHealingPool = false;
		private bool InPoison = false;

		// MAGIC NUMBERS
		public readonly float FALL_DISTANCE_MULTIPLIER = 1.2f;
		public readonly float POISON_DMG = 0.8f;
		public readonly int POISON_DMG_TIME_DELAY = 1;
		public readonly float HEALTH_REGEN_RATE = 1.0f;
		public readonly float HEALING_POOL_DELAY = 0.2f;

		// End Game Stuff
		public bool EndGame = false;

		void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;

			// Setting up fall animation stuff
			source = GetComponent<AudioSource>();
			FallDistance = 0;
			Checkpoint_Position = transform.position;
			current_checkpoint = 1;

			// Regenerate health
			InvokeRepeating("Regenerate", 0f, 5.0f); // Every 5 seconds regenerate health
		}


		public void Move(Vector3 move, bool crouch, bool jump, bool sprint)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2(move.x, move.z);
			m_ForwardAmount = move.z;

			// check if sprinting
			if (sprint == true) {
				footstepSpeed = 0.25f;
			} else {
				footstepSpeed = 0.4f;
			}

			ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// Check player's health
			if (CheckHealth() == true) {
				// send input and other state parameters to the animator
				UpdateAnimator(move);
			}
			DamageScreenFlash.color = Color.Lerp (DamageScreenFlash.color, Color.clear, flashSpeed * Time.deltaTime);

		}

		// Trigger checks (used for lava)
		void OnTriggerEnter(Collider coll) {
			Debug.Log (coll.name);
			if (coll.tag == "Lava") {
				TakeDamage(100f, flashColor_Red);
				if (!source.isPlaying)
					source.PlayOneShot (FallSound, 0.25f);
			} // Looks for collision of next checkpoint
			else if (coll.tag == "Poison") {
				InPoison = true;
				StartCoroutine(PlayerInPoison ());
			}
			else if (coll.name == "Healing Pool Collider") {
				inHealingPool = true;
				StartCoroutine(HealThisAmount (HEALTH_REGEN_RATE));
			}
			else if (coll.tag == "Checkpoint") {
				if (coll.name == ("Checkpoint " + current_checkpoint.ToString ())) {
					//Debug.Log ("Found checkpoint");
					Checkpoint_Position = coll.gameObject.transform.position;
					Checkpoint_Position.x += 2.0f;
					Checkpoint_Position.y += 4.0f;
					current_checkpoint++; // Increment which checkpoint to look for next

					coll.GetComponentInChildren<Light>().color = checkpointEnterColor;
				}
			}
			else if (coll.name == "EndGameTrigger") {
				EndGame = true;
			}
		}

		
		void OnTriggerExit(Collider coll) {
			if (coll.tag == "Poison") {
				StopCoroutine(PlayerInPoison ());
				InPoison = false;
				Debug.Log ("Exit poison");
			}
			else if (coll.name == "Healing Pool Collider") {
				StopCoroutine(HealThisAmount (HEALTH_REGEN_RATE));
				inHealingPool = false;
			}
		}

		IEnumerator PlayerInPoison() {
			while (InPoison) {
				TakeDamage (POISON_DMG, flashColor_Green);
				// repeat the sound AFTER it fully finishes
				if (source.isPlaying == false) {
					PlaySoundAtVol(CoughingSound, 0.25f); 
				}
				yield return new WaitForSeconds (POISON_DMG_TIME_DELAY);	
			}
		}

		// Method replicating source.PlayOneShot BUT ensures source.isPlaying will return true
		void PlaySoundAtVol(AudioClip sound, float volume) {
			source.clip = sound;
			source.volume = volume;
			source.Play ();
		}

		// Player's collision detection
		void OnCollisionEnter(Collision coll) {
			//Debug.Log (checkpoint.gameObject.name);
			//Debug.Log ("Checkpoint " + current_checkpoint.ToString ());

			if (coll.gameObject.tag == "Enemy") { // Looks for collision with enemy (or anything that will hurt player)
				// Enemies/Obstacles can hurt player
				// Adjust health
				// Play sound
			}

			// CODE FOR DIFFERENT SOUNDS ON LANDING ON TERRAIN (BUGGY!)
			/*else if (FallDistance >= 1) {
				if (coll.gameObject.tag == "Platform") {
					source.PlayOneShot(LandSound, 0.25f);
					FallDistance = 0;
				}
				if (coll.gameObject.name == "Terrain") {
					source.PlayOneShot(LandTerrainSound, 0.25f);
					FallDistance = 0;
				}
			} */
			/*
			if (!m_IsGrounded) {
				if (coll.gameObject.tag == "Platform") {
					LandPlatform = true;
					LandTerrain = false;
				}
				if (coll.gameObject.name == "Terrain") {
					LandTerrain = true;
					LandPlatform = false;
				}
			}
			*/
		}

		// Checks if player is dead (return false), if not return true
		bool CheckHealth() {
			//Debug.Log("Health is " + health);
			HealthSlider.value = health;
			if (health <= 0) { // DEAD
				// Death sound/animation here
				if (playerDead == false) {
					// play dead animation
					// play dead sound
					playerDead = true;
					Debug.Log("DEAD");
				}
				else {
					DeathDelay();
				}
				return false;
			} else {
				// Still alive
				return true;
			}
		}

		void DeathDelay() {
			deathTimer += Time.deltaTime;

			if (deathTimer >= delayAfterDeath) {
				// Reset character to checkpoint
				playerDead = false;
				deathTimer = 0;
				health = 100f;
				HealthSlider.value = health;
				Checkpoint();
			}
		}

		void Regenerate() {
			Debug.Log ("Regenerating... Health is " + health);
			if (health < 100 && health > 0) {
				health += HEALTH_REGEN_RATE;
				HealthSlider.value = health;
			}
		}

		IEnumerator HealThisAmount(float healingRate) {
			while (inHealingPool) {
				if (health < 100 && health > 0) {
					health += healingRate;
					HealthSlider.value = health;
				}
				yield return new WaitForSeconds (HEALING_POOL_DELAY);
			}
		}

		void TakeDamage(float dmg, Color flashColor) {
			health -= dmg;
			DamageScreenFlash.color = flashColor;
		}

		public void Checkpoint() {
			transform.position = Checkpoint_Position;
		}

		public void Checkpoint(int checkpoint) {
			GameObject devCheckpoint = GameObject.Find("Checkpoint " + checkpoint);

			Vector3 vector3 = devCheckpoint.transform.position;
			vector3.x += 2.0f;
			vector3.y += 5.0f;
			transform.position = vector3;
		}

		void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);

			// Play footstep sounds if in motion
			footstepTimer += Time.deltaTime;
			// If moving and on ground, play footstep sound every so time, depending on if sprinting
			if ((m_ForwardAmount > 0 || m_TurnAmount > 0) && footstepTimer > footstepSpeed && m_IsGrounded && !m_Crouching &&
			    !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
				source.PlayOneShot(FootstepSound, 0.1f);
				footstepTimer = 0;
			}

			if (!m_IsGrounded) {
				m_Animator.SetFloat ("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
			Mathf.Repeat (
				m_Animator.GetCurrentAnimatorStateInfo (0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded) {
				m_Animator.SetFloat ("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0) {
				m_Animator.speed = m_AnimSpeedMultiplier;
			} else {
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;

				// Find fall distance from previous y distance
				FallDistance -= transform.position.y;
				//Debug.Log (FallDistance);
				if (FallDistance > Fall_Trigger) {
					// Apply fall damage/animation
					source.PlayOneShot(FallSound, 0.25f); // Playing sound when falling
					// Can add animation here
					hurt = true;
					m_Animator.Play ("Death");

					// Adjust health
					float healthAdjust = (FallDistance * FALL_DISTANCE_MULTIPLIER/Fall_Trigger);
					TakeDamage(10.0f * healthAdjust, flashColor_Red);
				}
				else if (FallDistance >= 1) {
					source.PlayOneShot(LandSound, 0.15f);
				}

				/*
				else if (FallDistance >= 1){
					if (LandPlatform == true) {
						source.PlayOneShot(LandSound, 0.25f); // Playing sound when falling
						LandPlatform = false;
					}
					else if (LandTerrain == true) {
						source.PlayOneShot(LandTerrainSound, 0.25f);
						LandTerrain = false;
					}
				}*/
				// Reset FallDistance since grounded.
				FallDistance = 0;
			}
			else
			{
				// No longer grounded, set initial fall distance at current y position
				if (FallDistance < transform.position.y)
					FallDistance = transform.position.y;
				//Debug.Log (FallDistance);
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
