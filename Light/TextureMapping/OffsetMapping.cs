public class OffsetMapping : ContinuousMapping
{
    public float deltaX;
    public float deltaY;
    public bool horizontal;
    public bool vertical;

    private float offsetX;
    private float offsetY;

    protected override void HandleTexture()
    {
        if (horizontal)
        {
            offsetX += deltaX;
        }

        if (vertical)
        {
            offsetY += deltaY;
        }

        SetColor(destTex, offsetX, offsetY);
    }
}
