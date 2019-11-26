using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
[RequireComponent(typeof(BoxCollider))]
public class ColorPoint : MonoBehaviour
{
    private Renderer curRenderer;
    protected Material mat;
    protected ColorMapping colorMapping;
    public Color originalColor;
    public List<string> filterTags = new List<string>();



    protected virtual void Awake()
    {
        curRenderer = GetComponent<Renderer>();

        if (curRenderer)
        {
            mat = curRenderer.material;

            if (!mat)
            {
                Debug.LogError("Material is null");
                return;
            }
        }
        else
        {
            Debug.LogErrorFormat("Renderer is null, name: {0}", name);
            return;
        }

        colorMapping = GetComponentInParent<ColorMapping>();
    }

    private void OnTriggerEnter(Collider other)
    {
        originalColor = mat.color;
        TriggerBase TriggerBase = other.GetComponent<TriggerBase>();
        if (TriggerBase)
        {
            if ((TriggerBase.filterTags.Count != 0) && !FilterCompare(TriggerBase))
            {
                return;
            }
        }
        else
        {
            Debug.LogError("碰撞体没有TriggerBase组件");
            return;
        }
        switch (TriggerBase.lightType)
        {
            case LightType.SingleColor:
                {
                    mat.DOColor(TriggerBase.targetColor,TriggerBase.duringTime);
                    break;
                }
            case LightType.LowBrightness:
                {
                    float h, s, v;
                    Color.RGBToHSV(originalColor, out h, out s, out v);
                    mat.color = Color.HSVToRGB(h, s, v * 0.5f);
                    break;
                }
            case LightType.TextureMapping:
                {
                    if (colorMapping)
                    {
                        colorMapping.SetColor(transform,TriggerBase.duringTime);
                    }
                    break;
                }
            case LightType.ColorAndReset:
                {
                    OnColorAndReset(TriggerBase.targetColor, TriggerBase.duringTime);
                    //StartCoroutine(Change(Color.white));
                    //Sequence sequence=DOTween.Sequence();
                    //sequence.Append(mat.DOColor(TriggerBase.targetColor,TriggerBase.colorChangingTime));
                    //sequence.Append(mat.DOColor(originalColor,TriggerBase.colorChangingTime));
                    //sequence.AppendCallback(delegate{mat.color=originalColor;});
                    break;
                }
            case LightType.None:
                {
                    Debug.LogError("未选择灯光效果,检查碰撞体!");
                    break;
                }
        }
        // if (TriggerBase.resetColor)
        // {
        //     StartCoroutine(DelayFunc(TriggerBase.CDTime, delegate { mat.color = originalColor; }));
        // }
    }
    public void OnColorAndReset(Color color, float ShowColorTime)
    {
        // mat.color=color;
        // StartCoroutine(DelayFunc(ShowColorTime,()=>mat.color=originalColor));
        originalColor=mat.color;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(mat.DOColor(color, ShowColorTime/2));
        sequence.Append(mat.DOColor(originalColor, ShowColorTime/2));
        //sequence.AppendCallback(delegate { mat.color = originalColor; });
    }
    public void ShowColorMapping(float during = 0)
    {
        colorMapping.SetColor(transform,0.5f);
        if (during == 0)
            return;
        StartCoroutine(DelayFunc(during, delegate { mat.color = originalColor; }));
    }
    IEnumerator Change(Color color)
    {
        float ind = 0.95f;
        while (ind >= 0.3f)
        {
            SetColor(Color.Lerp(color, originalColor, ind));
            ind -= 0.1f;
            yield return new WaitForSeconds(0.04f);
        }
        while (ind <= 0.9f)
        {
            SetColor(Color.Lerp(color, originalColor, ind));
            ind += 0.1f;
            yield return new WaitForSeconds(0.04f);
        }
        SetColor(originalColor);
        StopAllCoroutines();
    }

    private IEnumerator DelayFunc(float delayTime, System.Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        callback();
    }

    private void OnEnable()
    {

    }

    public void SetColor(Color targetColor, bool needResetColor = false, float CDTime = 0.1f)
    {
        if (mat)
        {
            if (mat.color != targetColor)
            {
                originalColor = mat.color;
                mat.color = targetColor;

                if (needResetColor)
                {
                    StartCoroutine(DelayFunc(CDTime, delegate { mat.color = originalColor; }));
                }
            }
        }
        else
        {
            Debug.LogError("Material is null" + gameObject.name);
        }
    }

    //private void Update()
    //{
    //timer += Time.deltaTime;

    //if (timer >= interval)
    //{
    //    timer = 0f;
    //    //SetColorByHue();
    //}
    //}

    private void SetColorByHue()
    {
        float h, s, v;
        Color.RGBToHSV(mat.color, out h, out s, out v);
        float newHue = ((h * 360 + 10) % 360) / 360; // hue范围是[0,360]/360，这里每次累加10
        mat.color = Color.HSVToRGB(newHue, s, v);
    }

    public Color GetOriginalColor()
    {
        return originalColor;
    }
    public void TurnOff()
    {
        mat.color = Color.black;
    }
    private bool FilterCompare(TriggerBase tb)
    {
        foreach (var tag in tb.filterTags)
        {
            foreach (var thisTag in this.filterTags)
            {
                if (string.Equals(thisTag, tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

}
