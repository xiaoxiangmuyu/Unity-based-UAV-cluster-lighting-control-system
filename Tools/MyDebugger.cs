using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
public class MyDebugger : MonoBehaviour
{
    public static MyDebugger instance;
    Vector3 start, end;
    public  Material debugMat;

    bool isDebug;
    private void Awake() {
        instance=this;
        RenderPipelineManager.endCameraRendering+=OnPostRender;
    }
    public void DrawLine(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
    public void BeginDebug()
    {
        isDebug=true;
    }
    public void StopDebug()
    {
        isDebug=false;
    }
    /// <summary>
    /// 划线
    /// </summary>
    void OnPostRender(ScriptableRenderContext src, Camera camera)
    {
        if (!isDebug)
            return;
        // Debug.Log("ScreenWidth"+Screen.width);
        // Debug.Log("ScreenHeight"+Screen.height);
        Vector3 s=new Vector3(start.x / Screen.width, start.y / Screen.height, start.z);
        Vector3 e=new Vector3(end.x / Screen.width, end.y / Screen.height, end.z);
        GL.PushMatrix();
        debugMat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(s);
        GL.Vertex(e);
        //Debug.Log("start:"+s+"   end"+e);
        GL.End();
        GL.PopMatrix();

    }
}
