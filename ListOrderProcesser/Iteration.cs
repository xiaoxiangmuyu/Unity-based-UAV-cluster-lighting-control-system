using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Iteration:IListOrderProcesser
{
        public void ProcessOrder(RecordData inputData,Ease ease)
    {
        var templist=new List<string>(inputData.objNames);
        int length=templist.Count;
        for(int i=length-1;i>=0;i-=10)
        {
            templist.AddRange(templist.GetRange(0,i));
        }
        float processPercent=0;
        List<float>times=new List<float>(inputData.times);
        for(int i=0;i<templist.Count;i++)
        {
            times.Add(times[i]+inputData.animTime);
        }
        inputData.objNames=new List<string>(templist);
        inputData.times=new List<float>(times);

    }

}
