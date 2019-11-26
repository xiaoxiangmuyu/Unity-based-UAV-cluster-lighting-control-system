using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBase : MonoBehaviour
{
    public float Speed = 1f; // 半径变化速度
    public float startTime = 0.5f; // 从开始表演到显示流光时的时间
    public float waitInterval = 1f; // 流光播放时间间隔，必须大于ChangingColor.countDownTime，确保颜色复原后再播放下次流光
    public int playingCount = 2; // 流光播放次数
    public float changeMargin = 0.2f;
    public bool resetColor = false; // 流光过后是否恢复颜色
    //public float CDTime = 0.1f; // 复原等待时间
    public float colorChangingTime;
    public Color targetColor;
    public string filterTag = ""; // 影响的飞机的标签
    public LightType lightType;

    protected float timer = 0f;
    protected float waitTimer=0f;
    protected float playingTimer = 0f; // 流光播放时间间隔计时器
    protected Collider curCollider;
    protected Vector3 originalPos;
    protected bool isWait;

    protected virtual void Awake()
    {
        curCollider = GetComponent<Collider>();
        //curCollider.enabled = false;
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (playingCount <= 0)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= startTime)
        {
            //curCollider.enabled = true;

            if (playingCount > 0)
            {
                Play(Speed);
            }
        }
    }

    protected virtual void Play(float speed)
    {

    }
}
