using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RenderControl : MonoBehaviour
{
    public CinemachineVirtualCamera virtialCamera;
    private void Start() {
        var t =GetComponent<CinemachineTargetGroup>();
        var camera= GameObject.Find("PointsPreview").AddComponent<CinemachineBrain>();
        camera.GetComponent<Camera>().orthographic=false;
        foreach(var point in GameObject.Find("Main").transform.GetComponentsInChildren<ColorPoint>())
        {
            t.AddMember(point.transform,1,1);
        }
        Debug.Log("点击摄像机Solo模式才能正常工作！");
    }
}
