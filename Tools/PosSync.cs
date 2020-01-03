using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosSync : MonoBehaviour
{
    public Transform target;
    Vector3 lastPos;
    Vector3 moveDir;
    // Start is called before the first frame update
    void Start()
    {
        lastPos=target.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //moveDir=target.localPosition-lastPos;
        //transform.Translate(moveDir);
        transform.position=target.position;
        //transform.eulerAngles=target.eulerAngles;
    }
}
