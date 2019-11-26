using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutDown : MonoBehaviour
{
    public float delayTime;
    public string[] names = { }; // 需要变黑的飞机的名字，不填表示全部子对象
    public bool resetColor = false; // 变黑后是否需恢复原颜色
    public float CDTime = 0.1f; // 复原等待时间
    public bool disableCollider = false; // 变黑后是否禁用碰撞器以防止其他Trigger碰到飞机再变色

    private float delayTimer = 0f;
    private bool isDone = false;

    // Update is called once per frame
    void Update()
    {
        if (isDone)
        {
            return;
        }

        delayTimer += Time.deltaTime;

        if (delayTimer < delayTime)
        {
            return;
        }

        if (names == null || names.Length < 1)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                TurnOff(transform.GetChild(i));
            }
        }
        else
        {
            for (int i = 0; i < names.Length; i++)
            {
                TurnOff(transform.Find(names[i]));
            }
        }

        isDone = true;
    }

    private void TurnOff(Transform child)
    {
        if (child)
        {
            ColorPoint changingColor = child.GetComponent<ColorPoint>();

            if (changingColor)
            {
                changingColor.SetColor(Color.black, resetColor, CDTime);
            }
            else
            {
                Debug.LogErrorFormat("There is NO 'ChangingColor' script attach to child, child name: {0}", child.name);
            }

            if (disableCollider)
            {
                BoxCollider boxCollider = child.GetComponent<BoxCollider>();

                if (boxCollider && boxCollider.enabled)
                {
                    boxCollider.enabled = false;
                }
            }
        }
        else
        {
            Debug.LogErrorFormat("Child is NOT found, name: {0}", child.name);
        }
    }
}
