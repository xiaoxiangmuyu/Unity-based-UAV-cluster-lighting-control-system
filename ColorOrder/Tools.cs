using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public TextureMapping colorMapping;
    public Texture2D texture;
    void Start()
    {
        //StartCoroutine(DelayFunc(20,delegate{ChangeTex();}));
    }
    public void ChangeTex()
    {
        //colorMapping.ChangeTex(texture);
    }
    private IEnumerator DelayFunc(float delayTime, System.Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        callback();
    }
}
