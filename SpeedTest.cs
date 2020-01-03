using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTest : MonoBehaviour
{
    static float maxdis;
    public Vector3 lastPos;
    // Start is called before the first frame update
    void Awake()
    {
        lastPos=transform.position;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float value=Vector3.Distance(transform.position,lastPos);
        if(maxdis<value)
        maxdis=value;
        Debug.Log(maxdis);
        // float value=Mathf.Abs(transform.position.x-lastPos.x);
        // if(value>=0.12f) Debug.Log(value);
        lastPos=transform.position;
    }
}
