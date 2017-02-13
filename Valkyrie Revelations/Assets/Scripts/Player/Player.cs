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

    //Weapon Animation Controls
    protected Transform weapon;

    //Camera Rotation Speed
    private float camRotSpeed;
    private float gyroSensitivityUp;
    private float gyroSensitivityDown;
    private float shakeCamera;
    private bool shakeLeft;

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
        Weapon beamCannon = new BeamCannon();
        weaponList.Add(pistol);
        weaponList.Add(machineGun);
        weaponList.Add(beamCannon);
        equippedWeapon = (Weapon)weaponList[0];

        // Enable phone view rotation
        Input.gyro.enabled = true;
        camRotSpeed = 0.5f;
        gyroSensitivityUp = 0.35f;
        gyroSensitivityDown = 0.40f;
        shakeCamera = 0.0f;
        shakeLeft = true;

        weapon = this.transform.FindChild("Gunpoint");
    }


    private void Update()
    {
        if (!LevelManager.pause)
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (Input.touchCount > 0)
            {
                Touch myTouch = Input.touches[0];

                //Check if the phase of that touch equals Began
                if (myTouch.phase == TouchPhase.Began && !crouch)
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

            if (Input.GetMouseButtonDown(0) && !crouch)
            {
                shooting = true;
            }

            if (shooting && cooldown < 0 && !crouch)
            {
                if (equippedWeapon.GetWeaponType() == WeaponType.SINGLESHOT || equippedWeapon.GetWeaponType() == WeaponType.RAPIDFIRE)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        if (equippedWeapon.GetAmmoInClip() <= 0)
                        {
                            Debug.Log("Reload Weapon!");
                        }
                        else
                        {
                            weapon.LookAt(hit.point);
                            cooldown = equippedWeapon.ShootWeapon(hit, this.transform);
                        }
                    }

                    if (equippedWeapon.GetWeaponType() == WeaponType.SINGLESHOT)
                    {
                        shooting = false;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        shooting = false;
                    }
                }

                if (equippedWeapon.GetWeaponType() == WeaponType.CHARGINGCANNON)
                {
                    ChargeCannon cc = (ChargeCannon)equippedWeapon;
                    if (cc.GetChargeParticle().isStopped) {
                        cc.GetChargeParticle().Play();
                        cc.GetChargeParticle().enableEmission = true;
                    }
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        weapon.LookAt(hit.point);
                        if (Input.GetMouseButtonUp(0))
                        {
                            if (equippedWeapon.GetAmmoInClip() <= 0)
                            {
                                Debug.Log("Reload Weapon!");
                            }
                            else
                            {
                                cooldown = equippedWeapon.ShootWeapon(hit, this.transform);
                            }
                            shooting = false;
                        }
                    }
                    cc.AddChargeTime(Time.deltaTime, transform);
                }
            }
            cooldown -= Time.deltaTime;

            Area area = LevelManager.GetCurrentArea();
            if (area != null && LevelManager.moveToNextArea)
            {
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

            if (shakeCamera > 0)
            {
                CameraShake();
            }
        }
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (!LevelManager.pause)
        {
            // read inputs
            //float h = CrossPlatformInputManager.GetAxis("Horizontal");
            //float v = CrossPlatformInputManager.GetAxis("Vertical");
            this.gameObject.transform.Rotate(0, -Input.gyro.rotationRateUnbiased.y * camRotSpeed, 0);
#if !MOBILE_INPUT
            crouch = Input.GetKey(KeyCode.C);
            if (Input.GetKey(KeyCode.A))
            {
                this.gameObject.transform.Rotate(0, -0.5f, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.gameObject.transform.Rotate(0, 0.5f, 0);
            }
#else
            // Gyro Controls
            if (Mathf.Abs(Input.gyro.attitude.y) < gyroSensitivityUp)
            {
                crouch = false;
            }
            else if (Mathf.Abs(Input.gyro.attitude.y) > gyroSensitivityDown)
            {
                crouch = true;
            }
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
    }

    void OnGUI()
    {
        // Debugging section
        GUI.color = Color.white;
        GUI.Box(new Rect(0, 0, 200, 50), "Tilt : " + Input.gyro.attitude);

        // Actual GUI
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
        String totalAmmo = equippedWeapon.GetTotalAmmo() + "";
        if (equippedWeapon.GetTotalAmmo() == -1)
        {
            totalAmmo = "Infinite";
        }
        else if (equippedWeapon.GetTotalAmmo() == 0)
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(Screen.width - 200, Screen.height - 80, 200, 40), "No Ammo Remaining");
        }
        else
        {
            GUI.Box(new Rect(Screen.width - 200, Screen.height - 80, 200, 40), "Total Ammo Remaining");
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 60, 200, 20), totalAmmo, centeredStyle);
        }

        if (equippedWeapon.GetAmmoInClip() == 0)
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Reload!");
        }
        else
        {
            GUI.color = Color.white;
            GUI.Box(new Rect(Screen.width - 200, Screen.height - 40, 200, 40), "Ammo");
            GUI.Label(new Rect(Screen.width - 200, Screen.height - 20, 200, 20), equippedWeapon.GetAmmoInClip() + "/" + equippedWeapon.GetMaxAmmoInClip(), centeredStyle);
        }

        if (crouch)
        {
            int i = 0;
            foreach (Weapon weapon in weaponList)
            {
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

        GUI.color = Color.white;
        if (LevelManager.pause)
        {
            if (GUI.Button(new Rect(Screen.width / 4 * 3, 0, Screen.width / 4, Screen.height / 4), "Resume Game"))
            {
                LevelManager.pause = false;
                Time.timeScale = 1;
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width / 4 * 3, 0, Screen.width / 4, Screen.height / 4), "Pause Game"))
            {
                LevelManager.pause = true;
                Time.timeScale = 0;
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
                shakeCamera = 0.5f;
            }
            else
            {
                health = 0;
            }
        }
    }

    private void CameraShake()
    {
        if (shakeLeft) { 
            mainCamera.transform.Rotate(new Vector3(1, 0, 0));
            shakeLeft = false;
        }
        else
        {
            mainCamera.transform.Rotate(new Vector3(-1, 0, 0));
            shakeLeft = true;
        }
        shakeCamera -= Time.deltaTime;
        if (shakeCamera <= 0)
        {
            shakeCamera = 0;
            if (!shakeLeft)
            {
                mainCamera.transform.Rotate(new Vector3(-1, 0, 0));
                shakeLeft = true;
            }
        }
    }
}
