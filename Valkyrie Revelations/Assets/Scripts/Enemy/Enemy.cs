﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    protected float healthBarLength;
    protected float maxHealth;
    protected float health;
    protected bool healthBarEnabled;
    protected bool enemyDead;

    protected float shootBulletTime;
    protected int moveDirection;
    protected float movementTime;
    protected float moveSpeed;

	// Use this for initialization
	void Start () {
        healthBarLength = 50;
        maxHealth = 50;
        health = 50;
        healthBarEnabled = false;
        enemyDead = false;
        shootBulletTime = 0.0f;

        moveDirection = 0;
        movementTime = 0.0f;
        moveSpeed = 0.05f;

        enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
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
                    destinationPosition = new Vector3(destinationPosition.x, destinationPosition.y + 1.5f, destinationPosition.z);
                    this.transform.LookAt(destinationPosition);
                    Projectile projectile = new Projectile(this.transform.FindChild("Gunpoint").transform.position);
                    GameObject bullet = projectile.projectileObj;
                    ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
                    pm.SetDestination(destinationPosition);
                    pm.CalculateSpeed(500.0f);

                    shootBulletTime = Random.Range(0, 5.0f);

                    moveDirection = 0;
                }

                movementTime -= Time.deltaTime;
                if (movementTime <= 0)
                {
                    moveDirection = Random.Range(0, 5); // Int from 0-4 for rest, up/down/left/right
                    movementTime = Random.Range(0, 5.0f);
                }

                originPosition = this.transform.position;
                Debug.Log(originPosition);
                // Looks like will need to update the functions for movement with the model
                if (moveDirection == 0)
                {
                    Vector3 destinationPosition = LevelManager.GetPlayer().transform.position;
                    this.transform.LookAt(destinationPosition);
                }
                else if (moveDirection == 1)
                {
                    originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + moveSpeed);
                    this.transform.LookAt(originPosition);
                    this.GetComponent<Rigidbody>().MovePosition(originPosition);
                }
                else if (moveDirection == 2)
                {
                    originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z - moveSpeed);
                    this.transform.LookAt(originPosition);
                    this.GetComponent<Rigidbody>().MovePosition(originPosition);
                }
                else if (moveDirection == 3)
                {
                    originPosition = new Vector3(originPosition.x + moveSpeed, originPosition.y, originPosition.z);
                    this.transform.LookAt(originPosition);
                    this.GetComponent<Rigidbody>().MovePosition(originPosition);
                }
                else if (moveDirection == 4)
                {
                    originPosition = new Vector3(originPosition.x - moveSpeed, originPosition.y, originPosition.z);
                    this.transform.LookAt(originPosition);
                    this.GetComponent<Rigidbody>().MovePosition(originPosition);
                }
            }
        }
    }

    void OnGUI()
    {
        if (enabled) { 
            if (healthBarEnabled)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(screenPos.x - healthBarLength / 2, screenPos.y, healthBarLength, 10), tex);
                if (health > 0) { 
                    GUI.color = Color.green;
                    GUI.DrawTexture(new Rect(screenPos.x - healthBarLength / 2 + 1, screenPos.y + 1, healthBarLength/maxHealth * health - 2, 8), tex);
                }
            }
        }
    }

    public void takeDamage (int damage)
    {
        healthBarEnabled = true;
        if (health - damage > 0) { 
            health -= damage;
        }
        else {
            health = 0;
        }
    }

    protected void EnableRagdoll()
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    }
    protected void DisableRagdoll()
    {
        this.gameObject.transform.GetComponent<Rigidbody>().isKinematic = true;
        //this.gameObject.transform.GetComponent<Rigidbody>().detectCollisions = false;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        CollisionResult();
    }

    protected void CollisionResult()
    {
        //Debug.Log("This object has hit " + collider.gameObject.name);
        Vector3 originPosition = this.transform.position;
        if (moveDirection == 1)
        {
            originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z - moveSpeed * 2);
        }
        else if (moveDirection == 2)
        {
            originPosition = new Vector3(originPosition.x, originPosition.y, originPosition.z + moveSpeed * 2);
        }
        else if (moveDirection == 3)
        {
            originPosition = new Vector3(originPosition.x - moveSpeed * 2, originPosition.y, originPosition.z);
        }
        else if (moveDirection == 4)
        {
            originPosition = new Vector3(originPosition.x + moveSpeed * 2, originPosition.y, originPosition.z);
        }
        this.transform.position = originPosition;
        moveDirection = 0;
    }
}
