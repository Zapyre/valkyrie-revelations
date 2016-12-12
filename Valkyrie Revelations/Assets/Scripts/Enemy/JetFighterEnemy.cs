using UnityEngine;
using System.Collections;

public class JetFighterEnemy : Enemy {

    private enum Direction { Left, Center, Right };
    private float maxMoveTime = 5.0f;
    Direction dir;

    private ArrayList healthPart;

    // Use this for initialization
    void Start () {
        healthBarLength = 50;
        maxHealth = 500;
        health = 500;
        healthBarEnabled = false;
        enemyDead = false;
        shootBulletTime = 0.0f;

        moveDirection = 0;
        movementTime = 0.0f;
        maxMoveTime = 5.0f;
        moveSpeed = 90f;

        enabled = false;
        dir = Direction.Center;

        healthPart = new ArrayList();
        healthPart.Add(500); // Center
        healthPart.Add(250); // Left
        healthPart.Add(250); // Right
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
                Vector3 destinationPosition = LevelManager.GetPlayer().transform.position;
                shootBulletTime -= Time.deltaTime;
                if (shootBulletTime <= 0)
                {
                    foreach (Transform weapon in this.transform) { 
                        if (weapon.name == "Gunpoint") { 
                            Projectile projectile = new Projectile(weapon.position);
                            GameObject bullet = projectile.projectileObj;
                            ProjectileMovement pm = (ProjectileMovement)bullet.GetComponent(typeof(ProjectileMovement));
                            destinationPosition = new Vector3(destinationPosition.x, destinationPosition.y + 1.0f, destinationPosition.z);
                            pm.SetDestination(destinationPosition);
                            pm.CalculateSpeed(500.0f);
                        }
                    }
                    shootBulletTime = Random.Range(0, 5.0f);
                }

                movementTime -= Time.deltaTime;
                if (movementTime <= 0)
                {
                    if (moveDirection == 1)
                    {
                        if (dir == Direction.Center)
                        {
                            dir = Direction.Left;
                        }
                        else if (dir == Direction.Right)
                        {
                            dir = Direction.Center;
                        }
                    }
                    else if (moveDirection == 2)
                    {
                        if (dir == Direction.Center)
                        {
                            dir = Direction.Right;
                        }
                        else if (dir == Direction.Left)
                        {
                            dir = Direction.Center;
                        }
                    }
                    moveDirection = Random.Range(0, 3); // Int from 0-2 for rest, left/right
                    movementTime = maxMoveTime;
                }

                if (moveDirection == 0)
                {
                    this.transform.LookAt(destinationPosition);
                }
                else if (moveDirection == 1 && dir != Direction.Left)
                {
                    this.transform.RotateAround(destinationPosition, Vector3.up, moveSpeed / maxMoveTime * Time.deltaTime);
                    this.transform.LookAt(destinationPosition);
                }
                else if (moveDirection == 2 && dir != Direction.Right)
                {
                    this.transform.RotateAround(destinationPosition, Vector3.up, -moveSpeed / maxMoveTime * Time.deltaTime);
                    this.transform.LookAt(destinationPosition);
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

    public void takeDamage(int damage, int dir)
    {
        Debug.Log(dir);
        int hp = (int)healthPart[dir];
        if (dir != 0 || (dir == 0 && (int)healthPart[1] <= 0 && (int)healthPart[2] <= 0))
        {
            hp -= damage;
            if (hp < 0)
            {
                hp = 0;
            }
            healthPart[dir] = hp;
        }
        

        healthBarEnabled = true;
        hp = 0;
        for (int i = 0; i < 3; i++)
        {
            hp += (int)healthPart[i];
        }
        health = hp;
    }

    public override void OnCollisionEnter(Collision collision)
    {
    }
}
