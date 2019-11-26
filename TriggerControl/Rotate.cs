using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;
public class Rotate : MonoBehaviour
{
    public enum RotateType { Clockwise,AntiClockwise }
    // Start is called before the first frame update
    public float timeDelay;
    public float duringTime;
    [Min(0)]
    public float angle;
    public RotateType rotateType;
    public bool isFinish;
    public UnityEvent OnFinish;

    private float movedAngle;
    private float delayTimer;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(delayTimer<timeDelay)
        {
            delayTimer+=Time.deltaTime;
            return;
        }
        if(isFinish)
            return;
        if(rotateType==RotateType.AntiClockwise)
        {
            transform.Rotate(new Vector3(0,0,angle/25/duringTime));
        }
        else
        {
            transform.Rotate(new Vector3(0,0,-angle / 25 / duringTime));
        }
        movedAngle+= angle / 25 / duringTime;
        if (movedAngle >= angle)
        {
            isFinish=true;
            if(OnFinish!=null)
            OnFinish.Invoke();
        }
    }
    public void Again()
    {
        isFinish=false;
        movedAngle=0;
        GetComponent<BoxCollider>().size=new Vector3(1,12,22);
        Debug.Log("trigger");
    }
}
