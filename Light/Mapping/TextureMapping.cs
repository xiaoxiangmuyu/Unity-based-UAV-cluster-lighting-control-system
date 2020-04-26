﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
public class TextureMapping : ColorParent
{

    [BoxGroup("TextureMapping")]
    public bool isTexLoop;
    [BoxGroup("TextureMapping")]
    public int texChangeCount;
    [BoxGroup("TextureMapping")]
    public List<Texture2D> scrTexs = new List<Texture2D>();


    public string destTagName = ""; // 需要上色的飞机的标签名，为空表示都上色
    public float delayTime = 0f; // 开始上色前的时间
    public bool mappingOnAwake = false;
    

    protected float timer = 0f;
    protected int intMaxX;
    protected int intMaxY;
    [SerializeField][ShowInInspector]
    protected Dictionary<Transform, Vector2> screenPositions = new Dictionary<Transform, Vector2>();
    protected bool isFinished = false;
    [SerializeField][ShowInInspector]
    protected List<Texture2D> destTexs = new List<Texture2D>();


    private float maxX = 0f;
    private float maxY = 0f;
    private float minX = 0f;
    private float minY = 0f;
    private float delayTimer = 0f; // 开始上色前的时间计时器
    private int counter;


    protected virtual void Awake()
    {
        // 飞机的世界坐标转屏幕坐标
        if(screenPositions==null||screenPositions.Count==0)
        CoordinateTransformation();

        // 生成可容纳所有飞机显示的图片
        if(destTexs==null||destTexs.Count==0)
        InitTex();

        // 飞机上色。注意：在delayTime=0时上色会有1帧的延迟，办法是在Awake中上色
        if (mappingOnAwake)
        {
            SetColor();
        }
    }
    [Button]
    protected void InitTex()
    {
        if (scrTexs.Count == 0)
        {
            Debug.LogError("没有设置贴图" + gameObject.name);
            return;
        }
        
        scrTexs.Sort(Sort);

        for (int i = 0; i < scrTexs.Count; i++)
        {
            if (scrTexs[i].isReadable == false)
            {
                Debug.LogError("图片不可读!    " + scrTexs[i].name);
                return;
            }
            destTexs.Add(ScaleTexture(scrTexs[i], intMaxX, intMaxY));
        }
    }
    /// <summary>
    /// 根据目标图片为飞机上色
    /// </summary>
    /// <param name="destTex"></param>
    protected void SetColor(float offsetX = 0f, float offsetY = 0f)
    {
        if (destTexs == null)
        {
            Debug.LogError("贴图为空！");
            return;
        }

        if (screenPositions != null && screenPositions.Count > 0)
        {
            Renderer curRenderer;
            Material mat;

            foreach (var child in screenPositions.Keys)
            {
                if (child)
                {
                    if (child.GetComponent<ColorPoint>().State==PointState.Busy)
                        return;
                    curRenderer = child.GetComponent<Renderer>();

                    if (curRenderer)
                    {
                        mat = curRenderer.material;

                        if (mat)
                        {
                            // 飞机的屏幕坐标映射到图片上，取那一点的颜色作为飞机的颜色。
                            // 向上取整会造成边界点的颜色取到对面边界的颜色，所以改为向下取整。
                            Color color = destTexs[0].GetPixel(Mathf.FloorToInt(screenPositions[child].x + offsetX), Mathf.FloorToInt(screenPositions[child].y + offsetY));
                            child.GetComponent<ColorPoint>().originalColor = color;
                            mat.color = color;
                        }
                    }
                }
            }
        }
    }
    protected void SetColor(Color color)
    {

        Renderer curRenderer;
        Material mat;

        foreach (var child in screenPositions.Keys)
        {
            if (child)
            {
                if (child.GetComponent<ColorPoint>().State==PointState.Busy)
                    return;
                curRenderer = child.GetComponent<Renderer>();

                if (curRenderer)
                {
                    mat = curRenderer.material;
                    if (mat)
                    {
                        mat.color = color;
                    }
                }
            }
        }

    }
    [Button]
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

    /// <summary>
    /// 根据源图片生成指定大小的新图片
    /// </summary>
    /// <param name="source"></param>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <returns></returns>
    protected Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        if (source == null)
        {
            return null;
        }

        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                // 根据原图片颜色为新图片上色
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();

        // 用于调试查看生成的图片是否正确，需要时再打开
        //GameObject go = GameObject.Find("Quad");
        //if (go)
        //{
        //    go.transform.localScale = new Vector3(targetWidth, targetHeight, 0);
        //    go.GetComponent<Renderer>().material.mainTexture = result;
        //}

        return result;
    }

    private void Update()
    {
        if (isFinished) // 上色已完成，直接返回
        {
            return;
        }
        delayTimer += Time.deltaTime;

        if (delayTimer < delayTime) // 未到上色时间
        {
            return;
        }

        MappingFunc();
    }
    public void SetColor(Transform trans, float duringTime)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                Color targetColor = destTexs[0].GetPixel(Mathf.FloorToInt(screenPositions[child].x), Mathf.FloorToInt(screenPositions[child].y));
                child.GetComponent<Renderer>().material.DOColor(targetColor, duringTime);
            }
        }
    }
    public void SetColor(Transform trans)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                Color targetColor = destTexs[0].GetPixel(Mathf.FloorToInt(screenPositions[child].x), Mathf.FloorToInt(screenPositions[child].y));
                child.GetComponent<Renderer>().material.color = targetColor;
            }
        }
    }
    public override Color GetMappingColor(Transform trans, int texIndex)
    {
        foreach (var child in screenPositions.Keys)
        {
            if (child == trans)
            {
                Color targetColor = destTexs[texIndex].GetPixel(Mathf.FloorToInt(screenPositions[child].x), Mathf.FloorToInt(screenPositions[child].y));
                return targetColor;
            }
        }
        Debug.LogError("没有找到映射颜色__" + trans.name);
        return Color.white;
    }
    // public virtual Color GetMappingColor(Transform trans)
    // {

    // }
    protected virtual void MappingFunc()
    {
        isFinished = true;
    }
    public Color GetColor(Transform trans)
    {
        Color color = Color.clear;

        if (screenPositions.ContainsKey(trans))
        {
            color = destTexs[0].GetPixel(Mathf.FloorToInt(screenPositions[trans].x), Mathf.FloorToInt(screenPositions[trans].y));
        }
        else
        {
            Debug.LogErrorFormat("screenPositions does NOT contain key: {0}", trans.name);
        }

        return color;
    }
    int Sort(Texture2D a, Texture2D b)
    {
        return int.Parse(a.name) - int.Parse(b.name);
    }
}