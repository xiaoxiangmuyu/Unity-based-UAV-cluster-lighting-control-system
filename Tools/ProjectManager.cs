using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectManager : MonoBehaviour
{
    public  Quaternion RotationInfo;
    public  Vector3 PosInfo;
    public  int ChildCount;
    public static ProjectManager instance;
    public void Init()
    {
        instance=this;
    }
    private void Awake() {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
