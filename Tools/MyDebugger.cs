using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MyDebugger : MonoBehaviour
{
    public bool isDebug;
    Vector3 corner1;
    Vector3 corner2;
    Vector3 corner3;
    Vector3 corner4;
    public void DrawRect(Vector3 corner1,Vector3 corner2,Vector3 corner3,Vector3 corner4)
    {
        this.corner1=corner1;
        this.corner2=corner2;
        this.corner3=corner3;
        this.corner4=corner4;
        isDebug=true;
    }
    void DrawRect()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawLine(corner1,corner2);
        Gizmos.DrawLine(corner2,corner3);
        Gizmos.DrawLine(corner3,corner4);
        Gizmos.DrawLine(corner4,corner1);
    }
    void OnDrawGizmos()
    {
        if(!isDebug)
        return;
        DrawRect();
    }
}
