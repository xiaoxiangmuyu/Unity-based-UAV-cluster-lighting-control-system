using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
public class OffsetMapping : ColorMapping
{
    public enum DirTypeH { Left_Right, Right_Left }
    public enum DirTypeV { Up_Down, Down_Up }
    public float deltaX;
    public float deltaY;
    public bool horizontal;
    [HideIf("hideDirTypeV")]
    public DirTypeV dirTypeV;
    [HideIf("hideDirTypeH")]
    public DirTypeH dirTypeH;
    public bool vertical;
    [SerializeField]
    private Dictionary<Transform,Vector2> Offsets;
    private float offsetX;
    private float offsetY;
    private bool hideDirTypeV { get { return vertical == false; } }
    private bool hideDirTypeH { get { return horizontal == false; } }
    private void Offset(Transform child)
    {
        var temp=Offsets[child];
        if (horizontal)
        {
            if(dirTypeH==DirTypeH.Left_Right)
            temp.x += deltaX;
            else
            temp.x -= deltaX;
        }

        if (vertical)
        {
            if(dirTypeV==DirTypeV.Up_Down)
            temp.y += deltaY;
            else
            temp.y -= deltaY;
        }
        Offsets[child]=temp;
    }
    public override Color GetMappingColor(Transform trans)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                Color targetColor = destTex.GetPixel(Mathf.FloorToInt(screenPositions[child].x + offsetX), Mathf.FloorToInt(screenPositions[child].y + offsetY));
                return targetColor;
            }
        }
        Debug.LogError("没有找到映射颜色__" + trans.name);
        return Color.white;
    }
    public Color GetFlowMappingColor(Transform trans)
    {
        if(Offsets==null)
        {
            Debug.LogError("Offset 没有初始化");
            return Color.white;
        }
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                if(!Offsets.ContainsKey(child))
                {
                    Offsets.Add(child,Vector2.zero);
                }
                else
                {
                    Offset(child);
                }
                //Debug.Log(Offsets[child]);
                Color targetColor = destTex.GetPixel(Mathf.FloorToInt(screenPositions[child].x + Offsets[child].x), Mathf.FloorToInt(screenPositions[child].y + Offsets[child].y));
                return targetColor;
            }
        }
        Debug.LogError("没有找到映射颜色__" + trans.name);
        return Color.white;
    }
    [Button(ButtonSizes.Large)]
    void InitOffsets()
    {
        Offsets=new Dictionary<Transform, Vector2>();
        AddChild(transform);
        Debug.Log(Offsets.Count);
    }
    void AddChild(Transform tra)
    {
        for (int i = 0; i < tra.childCount; i++)
        {
            if (tra.GetChild(i).childCount == 0)
            {
                int temp;
                if (!int.TryParse(tra.GetChild(i).name, out temp))
                continue;
                tra.GetChild(i).GetComponent<Renderer>().material=Resources.Load<Material>("bai");
                Offsets.Add(tra.GetChild(i),Vector2.zero);

            }
            else
                AddChild(tra.GetChild(i));
        }
    }
}
