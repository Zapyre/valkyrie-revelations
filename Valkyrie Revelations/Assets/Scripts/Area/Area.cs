using UnityEngine;
using System.Collections;

public class Area {
    public Vector3 currentLookAt;
    public Vector3 currentShootAt;
    public Vector3 currentMoveAway;
    public int enemyBreakPoint;
    public Vector3 nextPosition;
    public Vector3 nextLookAt;
    public bool lookLeft;
    public bool lookRight;
    public string areaNotes;
    public bool areaAnim;

    public Area(int eBP, Vector3 nextPos)
    {
        enemyBreakPoint = eBP;
        nextPosition = nextPos;
    }

    public Area(Vector3 curLookAt, Vector3 curMoveAway, int eBP, Vector3 nextPos, Vector3 lookAt)
    {
        currentLookAt = curLookAt;
        currentShootAt = new Vector3(curLookAt.x, curLookAt.y + 1.5f, curLookAt.z);
        currentMoveAway = curMoveAway;
        enemyBreakPoint = eBP;
        nextPosition = nextPos;
        nextLookAt = lookAt;
        lookLeft = false;
        lookRight = false;
    }

    public Area(Vector3 curLookAt, Vector3 curMoveAway, int eBP, Vector3 nextPos, Vector3 lookAt, bool ll, bool lr, string an)
    {
        currentLookAt = curLookAt;
        currentShootAt = new Vector3(curLookAt.x, curLookAt.y + 1.5f, curLookAt.z);
        currentMoveAway = curMoveAway;
        enemyBreakPoint = eBP;
        nextPosition = nextPos;
        nextLookAt = lookAt;
        lookLeft = ll;
        lookRight = lr;
        areaNotes = an;
    }

    public Area(Vector3 curLookAt, Vector3 curMoveAway, int eBP, Vector3 nextPos, Vector3 lookAt, bool ll, bool lr, string an, bool aAnim)
    {
        currentLookAt = curLookAt;
        currentShootAt = new Vector3(curLookAt.x, curLookAt.y + 1.5f, curLookAt.z);
        currentMoveAway = curMoveAway;
        enemyBreakPoint = eBP;
        nextPosition = nextPos;
        nextLookAt = lookAt;
        lookLeft = ll;
        lookRight = lr;
        areaNotes = an;
        areaAnim = aAnim;
    }

    public void Animate ()
    {
        GameObject doorwayL = GameObject.Find("Cafe-Dropoff-L");
        GameObject doorwayR = GameObject.Find("Cafe-Dropoff-R");
        GameObject doorwayC = GameObject.Find("Cafe-Door-Dropoff");

        GameObject.Destroy(doorwayL.GetComponent<MeshCollider>());
        GameObject.Destroy(doorwayR.GetComponent<MeshCollider>());
        GameObject.Destroy(doorwayC.GetComponent<MeshCollider>());

        doorwayL.AddComponent<Rigidbody>();
        doorwayR.AddComponent<Rigidbody>();
        doorwayC.AddComponent<Rigidbody>();

        GameObject.Find("Cafe-Dropoff-L").transform.GetComponent<Rigidbody>().useGravity = true;
        GameObject.Find("Cafe-Dropoff-R").transform.GetComponent<Rigidbody>().useGravity = true;
        GameObject.Find("Cafe-Door-Dropoff").transform.GetComponent<Rigidbody>().useGravity = true;
    }

    public bool CheckAreaPassed(int eBP, float timer)
    {
        bool passed = false;
        if (enemyBreakPoint == 0 && timer == 0)
        {
            passed = true;
        }
        else if (enemyBreakPoint <= eBP && timer > 0)
        {
            passed = true;
        }
        return passed;
    }
}
