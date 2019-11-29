using Sirenix.OdinInspector;
using UnityEngine;
public class OffsetMapping : ContinuousMapping
{
    public enum DirTypeH{Left_Right,Right_Left}
    public enum DirTypeV{Up_Down,Down_Up}
    public float deltaX;
    public float deltaY;
    public bool horizontal;
    [HideIf("hideDirTypeV")]
    public DirTypeV dirTypeV;
    [HideIf("hideDirTypeH")]
    public DirTypeH dirTypeH;
    public bool vertical;


    private float offsetX;
    private float offsetY;
    private bool hideDirTypeV{get{return vertical==false;}}
    private bool hideDirTypeH{get{return horizontal==false;}}

    protected override void HandleTexture()
    {
        if (horizontal)
        {
            if(dirTypeH==DirTypeH.Left_Right)
            offsetX += deltaX;
            else
            offsetX -= deltaX;
        }

        if (vertical)
        {
            if(dirTypeV==DirTypeV.Up_Down)
            offsetY += deltaY;
            else
            offsetX -= deltaY;
        }
        //SetColor(offsetX, offsetY);
    }
    public override Color GetMappingColor(Transform trans)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                Color targetColor = destTex.GetPixel(Mathf.FloorToInt(screenPositions[child].x+offsetX), Mathf.FloorToInt(screenPositions[child].y+offsetY));
                return targetColor;
            }
        }
        Debug.LogError("没有找到映射颜色__" + trans.name);
        return Color.white;
    }
}
