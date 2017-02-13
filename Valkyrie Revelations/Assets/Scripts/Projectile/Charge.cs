using UnityEngine;
using System.Collections;

public class Charge
{
    public GameObject projectileObj;
    public Charge(Vector3 originPosition)
    {
        projectileObj = (GameObject)Resources.Load("Prefabs/Prefab-Charge/Charge", typeof(GameObject));
        projectileObj.transform.position = originPosition;
        projectileObj = LevelManager.InstantiatePrefab(projectileObj);
    }
}