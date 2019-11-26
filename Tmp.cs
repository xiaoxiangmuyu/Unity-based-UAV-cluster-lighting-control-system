using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tmp : MonoBehaviour
{
    float interval = 0.5f;
    float timer = 0f;
    int lightingNum = 5;
    int index = 0;
    int minIndex = 0;
    int maxIndex = 0;
    int maxNum = 20;
    public bool isStop = false;

    void Awake()
    {
        float d = (float)((long)(2.3780803482 * 100)) / 100;
        //Debug.Log(d);
        //Debug.Log(d.ToString(".##"));
    }

    private void Update()
    {
        //if (isStop)
        //{
        //    return;
        //}
        //timer += Time.deltaTime;
        //if (timer >= interval)
        //{
        //    timer = 0f;
        //    minIndex = index % maxNum;
        //    maxIndex = (minIndex + lightingNum - 1) % maxNum;
        //    if (minIndex < maxIndex)
        //    {
        //        Debug.LogErrorFormat("minIndex: {0}, maxIndex: {1}", minIndex, maxIndex);
        //    }
        //    else
        //    {
        //        Debug.LogErrorFormat("minIndex: {0}, maxIndex: {1}", minIndex, maxNum - 1);
        //        Debug.LogErrorFormat("new minIndex: {0}, maxIndex: {1}", 0, maxIndex);
        //    }
        //    index += 5;
        //}
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
