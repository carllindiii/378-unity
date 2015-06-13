using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityStandardAssets;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

		private Vector3 EndGameVector = new Vector3(0f,-10f,0f);
		private Animator finalShipAnimator;
		public Image EndGameFade;
		private float FadeSpeed = 0.5f;
		private float FadeDelay = 10f;
		private float FadeTimer = 0;
        
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
			finalShipAnimator = GameObject.Find ("SciFi_Fighter_AK5 Blue").GetComponent<Animator>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT

			m_Move *= 0.75f;
			// sprint speed multiplier
			bool sprint = false;
	        if (Input.GetKey(KeyCode.LeftShift)) {
				m_Move *= 1.5f;
				sprint = true;
			}

#endif
			if (m_Character.EndGame == false) {
				if (Input.GetKey (KeyCode.Escape))
					m_Character.Checkpoint ();

				if (Input.GetKey (KeyCode.Alpha1))
					m_Character.Checkpoint (1);
				if (Input.GetKey (KeyCode.Alpha2))
					m_Character.Checkpoint (2);
				if (Input.GetKey (KeyCode.Alpha3))
					m_Character.Checkpoint (3);
				if (Input.GetKey (KeyCode.Alpha4))
					m_Character.Checkpoint (4);
				if (Input.GetKey (KeyCode.Alpha5))
					m_Character.Checkpoint (5);
				if (Input.GetKey (KeyCode.Alpha6))
					m_Character.Checkpoint (6);
				if (Input.GetKey (KeyCode.Alpha7))
					m_Character.Checkpoint (7);

				// pass all parameters to the character control script
				m_Character.Move (m_Move, crouch, m_Jump, sprint);
				m_Jump = false;
			} else {
				m_Character.Move (EndGameVector, false, false, false); // make sure character does not move.
				GameObject.Find("EthanBody").GetComponent<Renderer>().enabled = false;
				GameObject.Find("EthanGlasses").GetComponent<Renderer>().enabled = false;

				// if you can see the ship, look at it. 
				if(GameObject.Find ("SciFi_Fighter_AK5 Blue").GetComponent<Renderer>().isVisible) {
					m_Cam.transform.LookAt(GameObject.Find ("SciFi_Fighter_AK5 Blue").transform);
				}
				else {
				}
				Camera.main.fieldOfView = 100;

				if(finalShipAnimator.GetCurrentAnimatorStateInfo(0).IsName("None")) {
					finalShipAnimator.Play ("FlyAway");
				}

//				Transform cameraView = m_Cam.transform;
//				cameraView.transform.tra.position.x -= 100;
//				m_Cam.position = cameraView.position;
//				
				// Fade screen
				//EndGameFade.color = Color.Lerp (EndGameFade.color, Color.black, FadeSpeed * Time.deltaTime);
//				FadeTimer += Time.deltaTime;
//				if (FadeTimer >= FadeDelay) {
//					Debug.Log ("CHANGE SCENES NOW");
//					Application.LoadLevel ("TestNextScene");
//				}
			}
        }
    }
}
