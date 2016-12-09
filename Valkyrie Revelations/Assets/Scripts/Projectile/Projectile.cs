using UnityEngine;
using System.Collections;

public class Projectile {
    public GameObject projectileObj;
    public Projectile(Vector3 originPosition)
    {
        projectileObj = (GameObject)Resources.Load("Prefabs/Prefab-Bullet/Bullet", typeof(GameObject));
        projectileObj.transform.position = originPosition;
        projectileObj = LevelManager.InstantiatePrefab(projectileObj);
    }
}
