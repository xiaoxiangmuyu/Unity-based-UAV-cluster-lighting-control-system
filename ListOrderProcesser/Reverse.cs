using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Reverse : IListOrderProcesser
{
    public void ProcessOrder(RecordData inputData,Ease ease)
    {
        var temp = new List<string>(inputData.objNames);
        temp.Reverse();
        inputData.objNames.AddRange(temp);

        List<float>times=new List<float>(inputData.times);
        for(int i=inputData.times.Count-1;i>=0;i--)
        {
            times.Add(2*inputData.animTime-inputData.times[i]);
        }

        
        inputData.times=new List<float>(times);


    }
}
