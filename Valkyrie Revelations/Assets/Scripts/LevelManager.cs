using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
    public static Light mainLight;

    public static ArrayList areaList;
    public static int areaAt;

    private static bool enemiesAreaActivated;
    public static int enemiesDefeated;
    public static bool moveToNextArea;

    private static GameObject player;
    private static int enemiesActivated;

    private float textAnimTime;

    public static bool pause;

    // Use this for initialization
    void Start () {
        //mainLight = (Light)GameObject.Find("Main Light").GetComponent("Light");

        enemiesDefeated = 0;
        player = GameObject.Find("Player");

        // Area initialization
        areaList = new ArrayList();
        Area a = new Area(new Vector3(0, 0, 2), new Vector3(3, 0, 0), 3, new Vector3(0, 0, 40), new Vector3(0,0,41));
        areaList.Add(a);
        Area b = new Area(new Vector3(0, 0, 2), new Vector3(3, 0, 0), 3, new Vector3(0, 0, 40), new Vector3(1, 0, 28));
        areaList.Add(b);
        /*Area b = new Area(new Vector3(2, 0, 0), new Vector3(0, 0, 3), 6, new Vector3(28, 0, 28), new Vector3(28, 0, 29));
        areaList.Add(b);
        Area c = new Area(new Vector3(0, 0, 2), new Vector3(-3, 0, 0), 9, new Vector3(-35, 0, 28), new Vector3(-35, 0, 27));
        areaList.Add(c);
        Area d = new Area(new Vector3(0, 0, -2), new Vector3(3, 0, 0), 12, new Vector3(0, 0, 28), new Vector3(0, 0, 29));
        areaList.Add(d);
        Area e = new Area(new Vector3(0, 0, 2), new Vector3(3, 0, 0), 12, new Vector3(0, 0, 44), new Vector3(0, 0, 45)); // Checkpoint area (no new enemies)
        areaList.Add(e);
        Area f = new Area(new Vector3(0, 0, 2), new Vector3(0, -1, 0), 17, new Vector3(0, 0, 44), new Vector3(0, 0, 45), true, true, "3-Screen Skirmish");
        areaList.Add(f);
        Area g = new Area(new Vector3(0, 0, 2), new Vector3(0, -1, 0), 18, new Vector3(0, 0, 0), new Vector3(0, 0, -1), true, true, "Boss Battle!", true);
        areaList.Add(g);*/

        areaAt = 0;

        enemiesActivated = 0;
        for (int i = enemiesActivated; i < GetCurrentArea().enemyBreakPoint; i++)
        {
            GameObject.Find("Enemy " + enemiesActivated).GetComponent<Enemy>().enabled = true;
            enemiesActivated++;
        }

        moveToNextArea = false;

        textAnimTime = 6.0f;

        pause = true;
        Time.timeScale = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (moveToNextArea)
        {
            if (GetCurrentArea() == null)
            {
                Debug.Log("You have completed the level!");
            }
            else
            {
                if (!enemiesAreaActivated)
                {
                    for (int i = enemiesActivated; i < GetCurrentArea().enemyBreakPoint; i++)
                    {
                        GameObject.Find("Enemy " + enemiesActivated).GetComponent<Enemy>().enabled = true;
                        enemiesActivated++;
                        enemiesAreaActivated = true;
                    }
                }
                NextPositionCheck();

                textAnimTime = 6.0f;
                if (areaList.Count > areaAt)
                {
                    if (GetCurrentArea().areaAnim)
                    {
                        GetCurrentArea().Animate();
                    }
                }
            }
        }
    }

    void OnGUI() {
        if (textAnimTime > 0) // Arrival at new stage
        {
            if (GetCurrentArea() == null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, 0, 100, 20), "Level Complete!");
            }
            else if (GetCurrentArea().areaNotes != null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 50, 0, 100, 20), GetCurrentArea().areaNotes);
            }
        }
    }

    public static GameObject InstantiatePrefab(GameObject go)
    {
        GameObject g = Instantiate(go, go.transform.position, Quaternion.identity) as GameObject;
        g.name = g.name.Replace("(Clone)", "");
        return g;
    }

    public static void Destroy (GameObject go)
    {
        Destroy(go);
    }

    public static void NextPositionCheck()
    {
        //Debug.Log("Enemies Defeated : " + enemiesDefeated);
        Area area = GetCurrentArea();
        if (area.CheckAreaPassed(enemiesDefeated, 1.0f))
        {
            moveToNextArea = true;
        }
    }

    public static GameObject GetPlayer()
    {
        return player;
    }

    public static Area GetCurrentArea()
    {
        if (areaList.Count <= areaAt)
        {
            return null;
        }
        return (Area)areaList[areaAt];
    }

    public static void SetupNextArea()
    {
        areaAt++;
        moveToNextArea = false;
        enemiesAreaActivated = false;
    }
}
