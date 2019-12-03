using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CallBack : ColorOrderBase
{
    public abstract void GetCallBack(ColorPoint point);

    public class EnableCol:CallBack
    {
        Collider collider;
        public override void GetCallBack(ColorPoint point)
        {
            point.GetComponent<Collider>().enabled=true;
            Debug.Log("!");
        }
    }
}
