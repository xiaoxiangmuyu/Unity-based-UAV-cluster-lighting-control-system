using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ChangeMat : MonoBehaviour
{


    [MenuItem("GameObject/Tool/ChangeMat", priority = 0)]
    static void ChangenewMat()
    {
        GameObject obj = Selection.activeGameObject;
        foreach (Renderer it in obj.transform.GetComponentsInChildren<MeshRenderer>())
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/utilityMaterial.mat");
            Material[] bufMat = new Material[it.sharedMaterials.Length];
            for (int i = 0; i < it.sharedMaterials.Length; i++)
            {
                bufMat[i] = mat;
            }
            it.sharedMaterials = bufMat;
        }
    }
}
