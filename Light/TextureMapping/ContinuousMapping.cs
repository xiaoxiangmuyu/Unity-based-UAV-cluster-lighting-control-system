using UnityEngine;

public class ContinuousMapping : ColorMapping
{
    public float duration = 0f;
    public float interval = 0.1f; // 间隔多少秒后再次上色

    private float durationTimer = 0f;

    protected override void MappingFunc()
    {
        durationTimer += Time.deltaTime;

        if (durationTimer < duration)
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                timer = 0f;
                HandleTexture();
            }
        }
        else
        {
            isFinished = true;
            ShutDown(); // 上色完毕后整体变黑，形成节奏感
        }
    }

    private void ShutDown()
    {
        if (screenPositions != null && screenPositions.Count > 0)
        {
            Renderer curRenderer;
            Material mat;

            foreach (var child in screenPositions.Keys)
            {
                if (child)
                {
                    curRenderer = child.GetComponent<Renderer>();

                    if (curRenderer)
                    {
                        mat = curRenderer.material;

                        if (mat)
                        {
                            mat.color = Color.black;
                        }
                    }
                }
            }
        }
    }

    protected virtual void HandleTexture() { }
}
