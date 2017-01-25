using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.ThirdPerson;


[RequireComponent(typeof(ThirdPersonCharacter))]
public class Enemy : MonoBehaviour
{

    protected float healthBarLength;
    protected float maxHealth;
    protected float health;
    protected bool healthBarEnabled;
    protected bool enemyDead;

    protected float shootBulletTime;
    protected int moveDirection;
    protected float movementTime;
    protected float moveSpeed;

    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Vector3 m_Move;

    // Use this for initialization
    void Start()
    {
        healthBarLength = 50;
        maxHealth = 50;
        health = 50;
        healthBarEnabled = false;
        enemyDead = false;
        shootBulletTime = Random.Range(3.0f, 5.0f);

        moveDirection = 0;
        movementTime = 0.0f;
        moveSpeed = 0.5f;

        enabled = false;

        m_Character = GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            if (health <= 0 && !enemyDead)
            {
                enemyDead = true;
                EnableRagdoll();
                //healthBarEnabled = false;
                LevelManager.enemiesDefeated++;
                LevelManager.NextPositionCheck();
                LevelManager.Destroy(this.gameObject, 2.0f);
            }
            if (!enemyDead)
            {
                Vector3 originPosition = this.transform.position;
                shootBulletTime -= Time.deltaTime;
                if (shootBulletTime <= 0)
                {
                    Vector3 destinationPosition = LevelManager.GetPlayer().transform.position;
                    destinationPosition = new Vector3(destinationPosition.x, destinationPosition.y + 1.0f, destinationPosition.z);
                    this.transform.LookAt(destinationPosition);
                    Projectile projectile = new Projectile(this.transform.FindChild("Gunpoint").transform.position, true);
                    GameObject bullet = projectile.projectileObj;
                    ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
                    pm.SetDestination(destinationPosition);
                    pm.CalculateSpeed(1000.0f);

                    shootBulletTime = Random.Range(3.0f, 5.0f);

                    moveDirection = 0;
                }

                movementTime -= Time.deltaTime;
                if (movementTime <= 0)
                {
                    moveDirection = Random.Range(0, 5); // Int from 0-4 for rest, up/down/left/right
                    movementTime = Random.Range(1.0f, 5.0f);
                }

                originPosition = this.transform.position;
                // Looks like will need to update the functions for movement with the model
                if (moveDirection == 0)
                {
                    m_Move = new Vector3(0, 0, 0);
                    Vector3 destinationPosition = LevelManager.GetPlayer().transform.position;
                    this.transform.LookAt(destinationPosition);
                }
                else if (moveDirection == 1)
                {
                    //originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + 5);
                    //this.transform.LookAt(originPosition);
                    //this.GetComponent<Rigidbody>().MovePosition(originPosition);
                    m_Move = new Vector3(moveSpeed, 0, 0);
                }
                else if (moveDirection == 2)
                {
                    //originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z - 5);
                    //this.transform.LookAt(originPosition);
                    //this.GetComponent<Rigidbody>().MovePosition(originPosition);
                    m_Move = new Vector3(-moveSpeed, 0, 0);
                }
                else if (moveDirection == 3)
                {
                    //originPosition = new Vector3(originPosition.x + 5, originPosition.y, originPosition.z);
                    //this.transform.LookAt(originPosition);
                    //this.GetComponent<Rigidbody>().MovePosition(originPosition);
                    m_Move = new Vector3(0, 0, moveSpeed);
                }
                else if (moveDirection == 4)
                {
                    //originPosition = new Vector3(originPosition.x - 5, originPosition.y, originPosition.z);
                    //this.transform.LookAt(originPosition);
                    //this.GetComponent<Rigidbody>().MovePosition(originPosition);
                    m_Move = new Vector3(0, 0, -moveSpeed);
                }
                m_Character.Move(m_Move, false, false);
            }
        }
    }

    void OnGUI()
    {
        if (enabled)
        {
            if (healthBarEnabled && !enemyDead)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(screenPos.x - healthBarLength / 2, screenPos.y, healthBarLength, 10), tex);
                if (health > 0)
                {
                    GUI.color = Color.green;
                    GUI.DrawTexture(new Rect(screenPos.x - healthBarLength / 2 + 1, screenPos.y + 1, healthBarLength / maxHealth * health - 2, 8), tex);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        healthBarEnabled = true;
        if (health - damage > 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;
        }
    }

    protected void EnableRagdoll()
    {
        //this.gameObject.GetComponent<Animator>().enabled = false;
        this.gameObject.GetComponent<ThirdPersonCharacter>().enabled = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    }
    protected void DisableRagdoll()
    {
        this.gameObject.transform.GetComponent<Rigidbody>().isKinematic = true;
        //this.gameObject.transform.GetComponent<Rigidbody>().detectCollisions = false;
    }
}
