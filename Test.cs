using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    
    public Gradient coloring;
    public SkinnedMeshRenderer meshRenderer;
    public float top = 10;
    public float buttom = 0;
    void OnEnable()
    {
      Mesh mesh=  meshRenderer.sharedMesh;
      Vector3[] v3=  mesh.vertices;
        List<Color> lc = new List<Color>();
        for (int i = 0; i < v3.Length; i++)
        {
            Color c= coloring.Evaluate(v3[i].y / (top - buttom));
            Debug.Log(c);
            lc.Add(new Color(c.r, c.g, c.b));
        }
        mesh.colors = lc.ToArray();
    }

}
