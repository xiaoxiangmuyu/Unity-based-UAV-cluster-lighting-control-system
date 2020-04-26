using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataProcesser 
{
    void Process(ref RecordData data,float animTime);
}
