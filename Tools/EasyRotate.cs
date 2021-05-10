using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EasyRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Camera mainCamera;
    Vector2 beginPos;
    Vector2 endPos;
    Vector3 temp;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        beginPos = new Vector2(Screen.width / 2, Screen.height / 2);
        beginPos = Input.mousePosition;
        temp = mainCamera.transform.eulerAngles;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        endPos = Input.mousePosition;
        Vector2 dir = (endPos - beginPos).normalized;
        float angle = Vector2.Angle(dir, Vector2.down);
        if (endPos.x < beginPos.x)
            angle = -angle;

        mainCamera.transform.eulerAngles = (new Vector3(temp.x, temp.y, angle));
        //ConsoleProDebug.Watch("angle", angle.ToString());
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("方向调整完成");
    }

}
