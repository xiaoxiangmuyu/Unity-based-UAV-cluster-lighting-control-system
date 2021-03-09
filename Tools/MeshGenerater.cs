using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class MeshGenerater : MonoBehaviour
{
    public GameObject pointPrefab;
    public Texture2D texture { get { return ProjectManager.Instance.texture; } }
    public int offsetSpeed;
    [SerializeField]
    List<Material> points;
    [Button("生成", ButtonSizes.Gigantic)]
    void Generate()
    {
        points = new List<Material>();
        var meshPos = GetComponent<MeshFilter>().mesh.vertices;
        foreach (var pos in meshPos)
        {
            var instance = Instantiate(pointPrefab, transform);
            instance.transform.localScale *= 0.05f;
            instance.transform.localPosition = transform.TransformPoint(pos);
            points.Add(instance.GetComponent<MeshRenderer>().material);

        }

    }
    private void Start()
    {

    }
    int add = 0;
    private void Update()
    {
        if (points == null)
            return;
        int x, y;
        var uv = GetComponent<MeshFilter>().mesh.uv;
        for (int i = 0; i < points.Count; i++)
        {
            if (uv[i].x > 1 || uv[i].y > 1)
            {
                uv[i].x -= (int)uv[i].x;
                uv[i].y -= (int)uv[i].y;
            }
            x = Mathf.FloorToInt(uv[i].x * texture.width);
            y = Mathf.FloorToInt(uv[i].y * texture.height);
            points[i].color = texture.GetPixel(x + add, y);
        }
        add += offsetSpeed;
    }
}
