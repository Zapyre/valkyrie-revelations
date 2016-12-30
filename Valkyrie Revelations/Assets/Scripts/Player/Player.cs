using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class Player : MonoBehaviour
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

    // Player Stats
    private float healthBarLength;
    private float maxHealth;
    private float health;
    private bool playerDead;

    //Weapons Loadout
    private ArrayList weaponList;
    private Weapon equippedWeapon;

    private void Start()
    {
        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<ThirdPersonCharacter>();

        mainCamera = Camera.main;
        coverCamera = GameObject.Find("Cover Camera").GetComponent<Camera>();

        // Game Mechanisms Initialization
        shooting = false;
        cooldown = 0f;

        // Player Stats Initialization
        healthBarLength = 200;
        maxHealth = 100;
        health = 100;
        playerDead = false;

        mainCamera.enabled = true;
        coverCamera.enabled = false;

        // Weapons initialization
        weaponList = new ArrayList();
        Weapon pistol = new Pistol();
        Weapon machineGun = new MachineGun();
        weaponList.Add(pistol);
        weaponList.Add(machineGun);
        equippedWeapon = (Weapon)weaponList[0];
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
            //shooting = false;
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
                if (equippedWeapon.GetAmmoInClip() <= 0)
                {
                    Debug.Log("Reload Weapon!");
                }
                else { 
                    cooldown = equippedWeapon.ShootWeapon(hit, this.transform);
                }
            }
        }
        cooldown -= Time.deltaTime;

        Area area = LevelManager.GetCurrentArea();
        if (area != null && LevelManager.moveToNextArea) {
            if (transform.position == area.nextPosition) // Arrival at new stage
            {
                transform.LookAt(area.nextLookAt);
                LevelManager.SetupNextArea();
            }
            else
            {
                Vector3 knightPos = transform.position;
                Vector3 newKnightPos = area.nextPosition;

                if (knightPos != newKnightPos)
                {
                    float increment = 0.1f;
                    float x = knightPos.x - newKnightPos.x;
                    float y = knightPos.y - newKnightPos.y;
                    float z = knightPos.z - newKnightPos.z;

                    float newX = newKnightPos.x;
                    float newY = newKnightPos.y;
                    float newZ = newKnightPos.z;

                    if (x < -increment)
                    {
                        newX = knightPos.x + increment;
                    }
                    else if (x > increment)
                    {
                        newX = knightPos.x - increment;
                    }
                    if (y < -increment)
                    {
                        newY = knightPos.y + increment;
                    }
                    else if (y > increment)
                    {
                        newY = knightPos.y - increment;
                    }
                    if (z < -increment)
                    {
                        newZ = knightPos.z + increment;
                    }
                    else if (z > increment)
                    {
                        newZ = knightPos.z - increment;
                    }

                    // Need to redo this positional
                    Vector3 newPos = new Vector3(newX, newY, newZ);
                    transform.LookAt(newPos);
                    transform.position = newPos;
                }
            }
        }
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
            equippedWeapon.Reload();
            mainCamera.enabled = false;
            coverCamera.enabled = true;
        }
        else
        {
            mainCamera.enabled = true;
            coverCamera.enabled = false;
        }
    }

    void OnGUI()
    {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(10, Screen.height - 32, healthBarLength, 20), tex);
        if (crouch)
        {
            GUI.color = Color.blue;
        }
        else
        {
            GUI.color = Color.green;
        }

        GUIStyle leftStyle = GUI.skin.GetStyle("Label");
        leftStyle.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(10, Screen.height - 52, healthBarLength, 20), "Your Health", leftStyle);
        GUI.DrawTexture(new Rect(11, Screen.height - 31, healthBarLength / maxHealth * health - 2, 18), tex);

        GUI.color = Color.white;
        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        GUI.Box(new Rect(Screen.width - 100, Screen.height - 50, 100, 50), "Ammo");
        GUI.Label(new Rect(Screen.width - 100, Screen.height - 20, 100, 20), equippedWeapon.GetAmmoInClip() + "/" + equippedWeapon.GetMaxAmmoInClip(), centeredStyle);

        if (crouch) {
            int i = 0;
            foreach (Weapon weapon in weaponList) {
                if (equippedWeapon == weapon)
                {
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.color = Color.white;
                }
                if (GUI.Button(new Rect(Screen.width / 2 / weaponList.Count * i + Screen.width / 4, Screen.height - Screen.height / 4, Screen.width / 2 / weaponList.Count, Screen.height / 4), weapon.GetName()))
                {
                    equippedWeapon = weapon;
                }
                i++;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!crouch && !LevelManager.moveToNextArea)
        {
            if (health - damage > 0)
            {
                health -= damage;
            }
            else
            {
                health = 0;
            }
        }
    }
}
