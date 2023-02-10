using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PRS
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}

public class Utils 
{
    public static Quaternion QI => Quaternion.identity;

    public static Vector3 MousePos
    {
        get
        {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 result2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 result3 = Input.mousePosition;
            Vector3 result4 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Vector3 mousPos = hit.point;
            //RaycastHit hit = Physics.Raycast(ray);
            //Vector3 rayVector;
            result.x = Input.mousePosition.x;
            result.y = 0.5f;
            result.z = Input.mousePosition.z;
            result3.y = 0.5f;
            return result3;
        }
    }
}
