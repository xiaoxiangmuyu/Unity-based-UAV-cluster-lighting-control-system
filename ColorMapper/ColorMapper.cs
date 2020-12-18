using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GGType//GlobalGradient Type
{
    Vertical,
    Horizontal,
    Distances,
}
public class ColorMapper : MonoBehaviour
{
    private void OnEnable()
    {
        //ProjectManager.Instance.RecordProject.AddMapper(this);
    }
    public GGType gGType;
    public Color GetColor(Gradient gradient, Vector3 pos)
    {
        switch (gGType)
        {
            case GGType.Vertical:
                {
                    float maxValue = transform.localScale.y;
                    return gradient.Evaluate((pos.y - (transform.position.y - transform.localScale.y / 2)) / maxValue);
                }
            case GGType.Horizontal:
                {
                    float maxValue = transform.lossyScale.x;
                    return gradient.Evaluate((pos.x - (transform.position.x - transform.localScale.x / 2)) / maxValue);
                }
            case GGType.Distances:
                {
                    float maxValue = GetComponent<SphereCollider>().radius;
                    float distance = Vector2.Distance(pos, transform.position);
                    return gradient.Evaluate(distance / maxValue);
                }
        }
        Debug.LogError("映射信息有问题,返回红色");
        return Color.red;
    }
}
