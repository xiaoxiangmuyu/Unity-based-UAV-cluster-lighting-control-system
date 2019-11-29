using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosSync : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position=target.position;
        transform.eulerAngles=target.eulerAngles;
    }
}
