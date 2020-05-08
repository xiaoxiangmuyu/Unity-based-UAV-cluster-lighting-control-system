// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UI;
// using System;

// public class SaveDance : EditorWindow
// {
//     [MenuItem("GameObject/More/CrFolder", priority = 0)]
//     static void Folder()
//     {
//         AssetDatabase.CreateFolder("Assets", "Animation");
//         AssetDatabase.CreateFolder("Assets", "Materials");
//         AssetDatabase.CreateFolder("Assets", "Models");
//         AssetDatabase.CreateFolder("Assets", "Resources");
//         AssetDatabase.CreateFolder("Assets", "Scenes");
//         AssetDatabase.CreateFolder("Assets", "Scripts");
//         Material mat = new Material(Shader.Find("UI/Default"));
//         AssetDatabase.CreateAsset(mat, "Assets/" + "Resources" + "/bai" + ".mat");
//     }
//     //创建初始文件夹及材质球

//     [MenuItem("GameObject/More/ReMaterial", priority = 0)]
//     static void Init()
//     {
//         GameObject[] b = Selection.gameObjects;


//         for (int i = 0; i < b.Length; i++)
//         {
//             MeshRenderer[] lll = b[i].GetComponentsInChildren<MeshRenderer>();
//             foreach (var item in lll)
//             {
//                 item.material = Resources.Load<Material>("bai");
//             }
//         }
//     }
//     //批量更换材质球

//     [MenuItem("GameObject/More/ReName", priority = 0)]
//     static void Dian()
//     {
//         GameObject[] b = Selection.gameObjects;

//         MeshRenderer[] lll = b[0].GetComponentsInChildren<MeshRenderer>();
//         for (int i = 0; i < lll.Length; i++)
//         {
//             string n = i + 1 + "";
//             lll[i].gameObject.name = n;

//         }
//         int a = lll.Length;
//         Debug.Log(a);

//     }
//     //重命名并输出总数

//     [MenuItem("GameObject/More/NoMeshrender", priority = 0)]
//     static void Fei()
//     {

//         ArrayList all = new ArrayList();


//         GameObject[] b = Selection.gameObjects;
//         Transform[] lll = b[0].GetComponentsInChildren<Transform>();

//         for (int i = 0; i < lll.Length; i++)
//         {
//             if (!lll[i].gameObject.GetComponent<MeshRenderer>())
//             {
//                 Debug.Log(lll[i].name);
//             }
//         }

//     }
//     //查找不含Meshrender的物体

//     [MenuItem("GameObject/More/CreateP", priority = 0)]
//     public static void CreatPrefab()
//     {

//         Transform[] aaaa = Selection.transforms;    //   只能在 Hierarchy 面板下多选，在其他面板 下 只能 选一个


//         if (aaaa.Length == 0)
//         {
//             return;
//         }

//         for (int i = 0; i < aaaa.Length; i++)
//         {


//             if (PrefabUtility.GetOutermostPrefabInstanceRoot(aaaa[i]))
//             {


//                 PrefabUtility.UnpackPrefabInstance(aaaa[i].gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
//             }

//             PrefabUtility.SaveAsPrefabAssetAndConnect(aaaa[i].gameObject, "Assets/Prefab/" + aaaa[i].name + ".prefab" + "", InteractionMode.UserAction);
//         }

//     }
//     //批量保存预制体

//     [MenuItem("GameObject/More/Save Dance", false, -1)]
//     public static void SaveDanceFile(MenuCommand menuCommand)
//     {
//         string path = "";
//         if (menuCommand.context == null || menuCommand.context.GetType() != typeof(GameObject))
//         {
//             EditorUtility.DisplayDialog("提示", "你必须选择一个item", "好的");
//             return;
//         }

//         GameObject parentObject = (GameObject)menuCommand.context;

//         if (parentObject.GetComponentInChildren<Image>())
//         {
//             EditorUtility.DisplayDialog("Error", "You are trying to sort a GUI element. This will screw up EVERYTHING, do not do", "Okay");
//             return;
//         }

//         path = @"D:\A";

//         if (path == "")
//         {
//             EditorUtility.DisplayDialog("提示", "你必须选择一个文件夹", "Okay");
//             return;
//         }


//         MeshRenderer[] objectTransforms = parentObject.transform.GetComponentsInChildren<MeshRenderer>();
//         int count = objectTransforms.Length;
//         List<string> resultList = new List<string>();
//         string[] resultArry = new string[count];
//         Debug.Log(count);
//         foreach (MeshRenderer child in objectTransforms)
//         {

//             string result = child.transform.name + " " + child.transform.position.x.ToString("F2") + " " + (-child.transform.position.z).ToString("F2") + " " + child.transform.position.y.ToString("F2");
//             Debug.Log(child.transform.name);
//             resultArry[int.Parse(child.transform.name) - 1] = result;
//             resultList.Add(result);


//         }
//         FileStream aFile = new FileStream(@path + @"\" + parentObject.transform.name + ".txt", FileMode.Create);
//         StreamWriter sw = new StreamWriter(aFile, Encoding.ASCII);
//         sw.Write(string.Join("\r\n", resultList.ToArray()));
//         sw.Write(string.Join("\r\n", resultArry));
//         sw.Close();
//         sw.Dispose();
//         EditorUtility.DisplayDialog("提示", "优秀", "Okay！");
//         return;
//     }
//     //导出静态

// }
// public class Bianjie : EditorWindow
// {
//     float high;
//     float low;
//     int shili;
//     GameObject mubiao;

//     [MenuItem("GameObject/Range", priority = 0)]
//     static void Chuangjian()
//     {
//         EditorWindow.GetWindow(typeof(Bianjie));
//     }
//     void OnGUI()
//     {


//         //在弹出窗口中控制变量
//         high = EditorGUILayout.FloatField("high", high);
//         low = EditorGUILayout.FloatField("low", low);
//         mubiao = (GameObject)EditorGUILayout.ObjectField("目标物体", mubiao, typeof(GameObject), true);
//         shili = EditorGUILayout.IntField("个数", shili);
//         //创建一个按钮
//         if (GUI.Button(new Rect(100, 120, 100, 30), "执行"))
//         {
//             GameObject fangkai = Resources.Load<GameObject>("Cube");
//             GameObject fangkai1 = Instantiate(fangkai);
//             GameObject fangkai2 = Instantiate(fangkai);
//             GameObject fangkai3 = Instantiate(fangkai);
//             GameObject fangkai4 = Instantiate(fangkai);
//             GameObject bianjie = new GameObject("bianjie");

//             fangkai1.name = "fangkuai1";
//             fangkai2.name = "fangkuai2";
//             fangkai3.name = "fangkuai3";
//             fangkai4.name = "fangkuai4";


//             bianjie.transform.position = mubiao.transform.position;
//             fangkai1.transform.parent = bianjie.transform;
//             fangkai2.transform.parent = bianjie.transform;
//             fangkai3.transform.parent = bianjie.transform;
//             fangkai4.transform.parent = bianjie.transform;
//             fangkai1.transform.localPosition = new Vector3(0, 0, 0);
//             fangkai2.transform.localPosition = new Vector3(1, 1, 1);
//             fangkai3.transform.localPosition = new Vector3(2, 2, 2);
//             fangkai4.transform.localPosition = new Vector3(3, 3, 3);
//         }
//     }
// }