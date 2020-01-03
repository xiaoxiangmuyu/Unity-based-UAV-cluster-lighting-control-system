using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DotweenUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DOTween.ManualUpdate(0.04f, 0.04f);
    }
}
