using Sirenix.OdinInspector;
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

        SetColor(destTex, offsetX, offsetY);
    }
}
