using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
/// <summary>
///  用于几个部分交替加速闪动
/// </summary>
public class Twinkle : MonoBehaviour
{
    public List<ChangingColor>triggersLeft;
    public List<ChangingColor>triggersRight;
    public List<ChangingColor>CircleIn;
    public List<ChangingColor>CircleOut;
    public float delayTime;
    public float showTime;
    public Color color;

    float timer;
    bool flag;
    // Start is called before the first frame update
    void Start()
    {
        if(triggersLeft==null||triggersRight==null)
        Debug.LogError("Triggers is null");
    }

    // Update is called once per frame
    void Update()
    {
        if(flag)
            return;
        timer+=Time.deltaTime;
        if(timer<delayTime)
        {
            return;
        }
        else
        {
            flag=true;
            CircleOut.ForEach((a)=>a.originalColor=Color.black);
            CircleIn.ForEach((a)=>a.originalColor=Color.black);
            CircleOut.ForEach((a)=>a.TurnOff());
            StartCoroutine(TwinkleCoroutine());
        }
            

    }
    IEnumerator TwinkleCoroutine()
    {
        float showTimer=showTime;
        bool flag = true;
        float interval=0.25f;
        int stage1=4;
        int stage2=4;
        int stage3=8;
        while (true)
        {
            if(flag)
            {
            triggersLeft.ForEach((a)=>a.ShowColorMapping(showTimer));
            CircleIn.ForEach((a)=>a.ShowColorMapping(showTimer+interval));
            }
            else
            {
            triggersRight.ForEach((a)=>a.ShowColorMapping(showTimer));
            CircleOut.ForEach((a)=>a.ShowColorMapping(showTimer+interval));
            }

            yield return new WaitForSeconds(showTimer+interval);
            flag=!flag;
            
            if(stage1!=1)
            {
                stage1-=1;
                continue;
            }
            else if(stage2!=0)
            {
                showTimer=0.4f;
                interval=0.125f;
                stage2-=1;
                continue;
            }
            else if(stage3!=0)
            {
                showTimer=0.25f;
                interval=0.0625f;
                stage3-=1;
            }
            else 
            break;
            
        }
        TurnOffAll();
        yield return new WaitForSeconds(1f);
        OpenAll();
        yield return new WaitForSeconds(0.02f);
        TurnOffAll();
        yield return new WaitForSeconds(0.05f);
        OpenAll();
        yield return null;
    }
    void TurnOffAll()
    {
        triggersLeft.ForEach((a)=>a.SetColor(Color.black));
        triggersRight.ForEach((a)=>a.SetColor(Color.black));
        CircleOut.ForEach((a)=>a.SetColor(Color.black));
        CircleIn.ForEach((a)=>a.SetColor(Color.black));
    }
    void OpenAll()
    {
        triggersLeft.ForEach((a)=>a.ShowColorMapping());
        triggersRight.ForEach((a)=>a.ShowColorMapping());
        CircleOut.ForEach((a)=>a.ShowColorMapping());
        CircleIn.ForEach((a)=>a.ShowColorMapping());
    }
}
