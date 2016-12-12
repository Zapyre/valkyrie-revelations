using UnityEngine;
using System.Collections;

public class ProjectileMovement : MonoBehaviour
{
    GameObject projectile;
    Rigidbody rbody;
    Vector3 destination;
    Vector3 speed;
    bool forceAdded;
    // Use this for initialization
    void Awake()
    {
        projectile = this.gameObject;
        rbody = this.GetComponent<Rigidbody>();
        destination = this.gameObject.transform.position;
        forceAdded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!forceAdded)
        {
            rbody.AddForce(speed);
            forceAdded = true;
        }
        if (Mathf.Abs(projectile.transform.position.x) > Mathf.Abs(destination.x) && Mathf.Abs(projectile.transform.position.y) > Mathf.Abs(destination.y) && Mathf.Abs(projectile.transform.position.z) > Mathf.Abs(destination.z))
        {
            //LevelManager.Destroy(projectile, 0.1f);
        }
    }

    public void SetDestination(Vector3 d)
    {
        destination = d;
    }

    public void CalculateSpeed(float multiplier)
    {        
        float x, y, z, d;
        Vector3 projpos = this.gameObject.transform.position;
        x = Mathf.Abs(destination.x - projpos.x);
        y = Mathf.Abs(destination.y - projpos.y);
        z = Mathf.Abs(destination.z - projpos.z);
        d = x + y + z;
        float inverter = 1.0f;
        if (projpos.x > destination.x)
        {
            inverter = -1.0f;
        }
        x = x / d * multiplier * inverter;
        inverter = 1.0f;
        if (projpos.y > destination.y)
        {
            inverter = -1.0f;
        }
        y = y / d * multiplier * inverter;
        inverter = 1.0f;
        if (projpos.z > destination.z)
        {
            inverter = -1.0f;
        }
        z = z / d * multiplier * inverter;
        speed = new Vector3(x, y, z);
        projectile.transform.LookAt(destination);
        rbody.isKinematic = false;
        forceAdded = false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Your bullet hit " + collision.gameObject.name);
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        /*if (collision.gameObject.transform.parent != null)
        {
            JetFighterEnemy jfenemy = collision.gameObject.transform.parent.GetComponent<JetFighterEnemy>();
            Player player = collision.gameObject.transform.parent.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(10);
            }
            else if (jfenemy != null)
            {
                if (collision.gameObject.tag == "left")
                {
                    jfenemy.takeDamage(10, 1);
                }
                else if (collision.gameObject.tag == "right")
                {
                    jfenemy.takeDamage(10, 2);
                }
                else
                {
                    jfenemy.takeDamage(10, 0);
                }
            }
        }
        else */
        if (enemy != null)
        {
            enemy.takeDamage(10);
        }

        rbody.isKinematic = true;
        LevelManager.Destroy(projectile, 0.0001f);
    }
}
