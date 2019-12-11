using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class ColorMapping : ColorParent
{
    //渐变的方向类型
    public enum DirType
    {
        Up_Down,//上下
        Down_UP,//下上
        Left_Right,//左右
        Right_Left,//右左
        In_Out,//内外
        Out_In,//外内
        Custom
    }
    [Tooltip("是否Log中心点位置")]
    public bool logCenterPoint;
    [Tooltip("映射的中心点位置")]
    [ShowIf("IsCircleInOut")]
    public Vector2 centerPoint;
    [ShowIf("IsCustomDir")]
    public Vector2 customDirection;
    public DirType dirType;
    [Tooltip("颜色是否循环")]
    public bool isColorLoop;
    [Tooltip("显示多少次更换下一个颜色")]
    public int ColorChangeCount;
    public List<Gradient> colors = new List<Gradient>();


    private float maxDistance;
    private float maxValue { get { return Mathf.Sqrt(Mathf.Pow(customDirection.x, 2) + Mathf.Pow(customDirection.y, 2)); } }
    private float maxX = 0f;
    private float maxY = 0f;
    private float minX = 0f;
    private float minY = 0f;
    private bool IsCircleInOut { get { return dirType == DirType.In_Out || dirType == DirType.Out_In; } }
    private bool IsCustomDir { get { return dirType == DirType.Custom; } }
    protected int intMaxX;
    protected int intMaxY;
    public string destTagName = ""; // 需要上色的飞机的标签名，为空表示都上色

    protected Dictionary<Transform, Vector2> screenPositions = new Dictionary<Transform, Vector2>();

    // Start is called before the first frame update
    void Awake()
    {
        CoordinateTransformation();
        if (IsCircleInOut)
            InitCircleDistance();
        Debug.Log("intMaxX:"+intMaxX+"   intMaxY"+intMaxY);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void InitCircleDistance()
    {
        if (logCenterPoint)
            Debug.Log(gameObject.name + "的渐变中心点为" + new Vector2(intMaxX / 2, intMaxY / 2));
        foreach (var obj in screenPositions.Keys)
        {
            if (Vector2.Distance(screenPositions[obj], centerPoint) > maxDistance)
                maxDistance = Vector2.Distance(screenPositions[obj], centerPoint);
        }
    }
    /// <summary>
    /// 将飞机的世界坐标转为屏幕坐标，并计算最大宽度和高度
    /// </summary>
    private void CoordinateTransformation()
    {
        if (transform.childCount > 0)
        {
            Vector3 axis = Vector3.zero;
            screenPositions.Clear();
            Transform[] children = GetComponentsInChildren<Transform>();
            List<Transform> filterChildren = new List<Transform>();

            // 记录需要上色的飞机
            foreach (Transform child in children)
            {
                if (child && child.childCount > 0) // 排除有子对象的，只保留最末级的对象
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(destTagName) && !child.CompareTag(destTagName)) // 只对特定飞机上色
                {
                    continue;
                }

                filterChildren.Add(child);
            }

            if (filterChildren != null && filterChildren.Count > 0)
            {
                // 初始化值域。注意不能设为“0”，因为“0”可能不在值域中，造成后面计算值域时出错。
                axis = Camera.main.WorldToScreenPoint(filterChildren[0].position);
                maxX = axis.x;
                minX = axis.x;
                maxY = axis.y;
                minY = axis.y;

                // 计算值域
                for (int i = 0; i < filterChildren.Count; i++)
                {
                    // 解决图案朝向问题，以摄像机画面为准
                    axis = Camera.main.WorldToScreenPoint(filterChildren[i].position);

                    if (axis.x > maxX)
                    {
                        maxX = axis.x;
                    }

                    if (axis.x < minX)
                    {
                        minX = axis.x;
                    }

                    if (axis.y > maxY)
                    {
                        maxY = axis.y;
                    }

                    if (axis.y < minY)
                    {
                        minY = axis.y;
                    }
                }
                // 记录飞机二维坐标
                for (int i = 0; i < filterChildren.Count; i++)
                {
                    axis = Camera.main.WorldToScreenPoint(filterChildren[i].position);

                    // 坐标当前值减去最小值，使得左下角作为原点，与Texture2D.GetPixel的坐标系保持一致
                    screenPositions[filterChildren[i]] = new Vector2(axis.x - minX, axis.y - minY);
                }
            }

            // 解决边缘一列取色取到另一侧的问题，所以向上取整
            intMaxX = Mathf.CeilToInt(maxX - minX);
            intMaxY = Mathf.CeilToInt(maxY - minY);
            //Debug.LogErrorFormat("maxX: {0}, minX: {1}, intMaxX: {2}, maxY: {3}, minY: {4}, intMaxY: {5}", maxX, minX, intMaxX, maxY, minY, intMaxY);
        }
    }
    Color targetColor;
    public override Color GetMappingColor(Transform trans, int texIndex)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                switch (dirType)
                {
                    case DirType.Up_Down:
                        targetColor = colors[texIndex].Evaluate(1 - screenPositions[child].y / intMaxY);
                        break;
                    case DirType.Down_UP:
                        targetColor = colors[texIndex].Evaluate(screenPositions[child].y / intMaxY);
                        break;
                    case DirType.Left_Right:
                        targetColor = colors[texIndex].Evaluate(screenPositions[child].x / intMaxX);
                        break;
                    case DirType.Right_Left:
                        targetColor = colors[texIndex].Evaluate(1 - screenPositions[child].x / intMaxX);
                        break;
                    case DirType.In_Out:
                        float value = Vector2.Distance(screenPositions[child], centerPoint);
                        targetColor = colors[texIndex].Evaluate(value / maxDistance);
                        break;
                    case DirType.Out_In:
                        float value2 = Vector2.Distance(screenPositions[child], centerPoint);
                        targetColor = colors[texIndex].Evaluate(1 - value2 / maxDistance);
                        break;
                    case DirType.Custom:
                        float dirAngle = Mathf.Atan2(customDirection.y, customDirection.x);
                        float angelOrigin = Mathf.Atan2(screenPositions[child].y, screenPositions[child].x);
                        float angleDiff = Mathf.Abs(dirAngle - angelOrigin);
                        float xieBian = Mathf.Sqrt(Mathf.Pow(screenPositions[child].x, 2) + Mathf.Pow(screenPositions[child].y, 2));
                        float _value = xieBian * Mathf.Cos(angleDiff);
                        targetColor = colors[texIndex].Evaluate(_value / maxValue);
                        break;
                }
                return targetColor;
            }
        }
        Debug.LogError("没有找到映射颜色__" + trans.name);
        return Color.white;
    }

}
