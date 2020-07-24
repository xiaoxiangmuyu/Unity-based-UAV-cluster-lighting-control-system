using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
public class MyDebugger : MonoBehaviour
{
    public static MyDebugger instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MyDebugger>();
            return _instance;
        }
    }
    static MyDebugger _instance;
    Vector3 start, end;
    public Material debugMat;

    bool isDebug;
    public bool IsDebugMode { get { return isDebug; } }
    private void Awake()
    {
        _instance = this;
    }
    public void DrawLine(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
    public void BeginDebug()
    {
        isDebug = true;
    }
    public void StopDebug()
    {
        isDebug = false;
    }
    private void OnDrawGizmos()
    {
        if (isDebug)
            Debug.DrawLine(start, end, Color.red);
    }
}
