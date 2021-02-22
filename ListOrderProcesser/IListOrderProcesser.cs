using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public interface IListOrderProcesser
{
    void ProcessOrder(RecordData inputData,Ease ease);
}
