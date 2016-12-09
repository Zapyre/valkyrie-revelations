using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        // Camera controls
        private Camera mainCamera;
        private Camera coverCamera;

        // Game Mechanisms
        private bool shooting;
        private float cooldown;
        private bool crouch;


        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();

            mainCamera = Camera.main;
            coverCamera = GameObject.Find("Cover Camera").GetComponent<Camera>();

            // Game Mechanisms Initialization
            shooting = false;
            cooldown = 0f;
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (Input.touchCount > 0)
            {
                Touch myTouch = Input.touches[0];

                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began)
                {
                    //If so, set touchOrigin to the position of that touch
                    Vector2 touchOrigin = myTouch.position;
                    shooting = true;
                }
            }
            else
            {
                shooting = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                shooting = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                shooting = false;
            }

            if (shooting && cooldown < 0 && !crouch)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debug.Log(hit.point + " | You shot at " + hit.transform.gameObject.name);
                    // Create bullet
                    Projectile projectile = new Projectile(this.transform.FindChild("Gunpoint").position);
                    GameObject bullet = projectile.projectileObj;
                    ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
                    if (hit.point != null)
                    {
                        pm.SetDestination(hit.point);
                        pm.CalculateSpeed(2000.0f);
                    }
                    //((ParticleSystem)bullet.GetComponentInChildren(typeof(ParticleSystem))).Play();
                    //GameObject bullet = GameObject.CreatePrimative(PrimativeType.Sphere);
                }
                cooldown = 0.1f;
            }
            cooldown -= Time.deltaTime;
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            crouch = Input.GetKey(KeyCode.C);

            // Removing this for on rails movement
            // calculate move direction to pass to character
            /*if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }*/
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;

            if (crouch)
            {
                mainCamera.enabled = false;
                coverCamera.enabled = true;
            }
            else
            {
                mainCamera.enabled = true;
                coverCamera.enabled = false;
            }
        }
    }
}
