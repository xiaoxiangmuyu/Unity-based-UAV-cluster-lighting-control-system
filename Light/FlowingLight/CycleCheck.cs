using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class CycleCheck : MonoBehaviour
{
    public float radius = 1f;
    public float interval = 0.5f;
    public float angle = 90f; // 旋转角度
    public float delayTime = 0f;
    public bool resetColor = false;
    public float CDTime = 0.1f;
    public float duration = 2f;

    private float timer = 0f;
    private bool isDone = false;
    private float delayTimer = 0f;
    private float durationTimer = 0f;
    private TextureMapping colorMapping;

    private void Awake()
    {
        colorMapping = GetComponentInParent<TextureMapping>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Check();
    }

    private void Check()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward);

        if (hits != null && hits.Length > 0)
        {
            ColorPoint changingColor;

            for (int i = 0; i < hits.Length; ++i)
            {
                //if (!hits[i].collider.CompareTag("CycleCheck"))
                //{
                //    Debug.LogErrorFormat("Collider's tag is NOT 'CycleCheck', collider name: {0}", hits[i].collider.name);
                //    continue;
                //}
                Debug.LogFormat("hit!{0}",hits[i].collider.name);
                changingColor = hits[i].collider.GetComponent<ColorPoint>();

                if (changingColor)
                {
                    if (colorMapping)
                    {
                        Color color = colorMapping.GetColor(hits[i].collider.transform);

                        if (color == Color.clear)
                        {
                            continue;
                        }

                        changingColor.SetColor(color, resetColor, CDTime);
                    }
                    else
                    {
                        Debug.LogError("ColorMapping script is NOT found in parent");
                    }
                }
                else
                {
                    Debug.LogErrorFormat("There is NO ChangingColor script on the collider, name: {0}", hits[i].collider.name);
                }
            }
        }
        else
        {
            Debug.LogError("no hit");
        }
    }

    private void Update()
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

        durationTimer += Time.deltaTime;

        if (durationTimer >= duration)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            transform.Rotate(0, -angle, 0);
            if (transform.rotation.y == 0f)
            {
                isDone = true;
            }
            Check();
        }
    }
    
   

}
