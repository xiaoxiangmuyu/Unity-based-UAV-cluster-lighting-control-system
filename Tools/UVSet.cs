using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class UVSet : SerializedMonoBehaviour
{
    public Camera mainCamera;
    public List<string> X;
    public List<string> Y;
    public List<ColorPoint> points;
    [ShowInInspector]
    public Dictionary<string, Vector2> nameToUV;
    Texture2D texture
    {
        get
        {
            return ProjectManager.Instance.texture;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nameToUV == null)
            return;
        for (int i = 0; i < points.Count; i++)
        {
            points[i].mat.color = texture.GetPixel((int)(nameToUV[points[i].name].x * texture.width), (int)(nameToUV[points[i].name].y * texture.height));
        }
    }
    [Button(ButtonSizes.Gigantic)]
    [LabelText("曲面UV展开")]
    void Calculate()
    {
        var temp = new List<GameObject>(MyTools.FindObjs(X));
        points = new List<ColorPoint>();
        nameToUV = new Dictionary<string, Vector2>();
        for (int i = 0; i < temp.Count; i++)
        {
            points.Add(temp[i].GetComponent<ColorPoint>());
        }
        foreach (var point in X)
        {
            float x = (float)X.IndexOf(point) / (float)(X.Count - 1);
            float y = (float)Y.IndexOf(point) / (float)(Y.Count - 1);
            nameToUV.Add(point, new Vector2(x, y));
        }


    }
    public List<string> XCords;
    public List<string> YCords;
    [ShowInInspector]
    Dictionary<float, float> XPosToUV;
    [ShowInInspector]
    Dictionary<float, float> YPosToUV;
    [Button]
    void SetX()
    {
        XPosToUV = new Dictionary<float, float>();
        var objs = MyTools.FindObjs(XCords);
        for (int i = 0; i < XCords.Count; i++)
        {
            XPosToUV.Add(mainCamera.WorldToScreenPoint(objs[i].transform.position).x, (float)i / (float)(XCords.Count));
        }
    }
    [Button]
    void SetY()
    {
        YPosToUV = new Dictionary<float, float>();
        var objs = MyTools.FindObjs(YCords);
        for (int i = 0; i < YCords.Count; i++)
        {
            YPosToUV.Add(mainCamera.WorldToScreenPoint(objs[i].transform.position).y, (float)i / (float)(YCords.Count));
        }
    }
    [Button(ButtonSizes.Gigantic)]
    [LabelText("平整屏幕UV展开")]
    void ScreenInit()
    {
        points = new List<ColorPoint>();
        nameToUV = new Dictionary<string, Vector2>();
        foreach (var point in UnityEditor.Selection.gameObjects)
        {
            Vector2 uv = new Vector2();
            foreach (var pair in XPosToUV)
            {
                if (mainCamera.WorldToScreenPoint(point.transform.position).x - (pair.Key) <= 1)
                {
                    uv.x = pair.Value;
                    Debug.Log("Find X");
                    break;
                }
            }
            foreach (var pair in YPosToUV)
            {
                if (mainCamera.WorldToScreenPoint(point.transform.position).y - (pair.Key) <= 1)
                {
                    uv.y = pair.Value;
                    Debug.Log("Find Y");
                    break;
                }
            }
            nameToUV.Add(point.name, uv);
            points.Add(point.GetComponent<ColorPoint>());
        }

    }
}
