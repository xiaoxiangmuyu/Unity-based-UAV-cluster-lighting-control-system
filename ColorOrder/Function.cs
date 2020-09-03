using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using DG.Tweening;
using Sirenix.OdinInspector;
public abstract class Function : ColorOrderBase
{
    public virtual void GetFunction(ColorPoint point)
    {
        Action(point);
    }
    protected virtual void Action(ColorPoint point) { }

    public class GlobalGradientColor : Function
    {
        public float time;
        public float interval;
        public Gradient gradient;
        private ColorMapper mappingSource;
        public ColorMapper MappingSource
        {
            get
            {
                if (!mappingSource)
                {
                    GetMappingSource();
                }
                return mappingSource;
            }
        }
        [ValueDropdown("availableMappingSource")]
        [OnValueChanged("GetMappingSource")]
        public string mappingSourceName;
        IEnumerable availableMappingSource
        {
            get
            {
                var datalist = GameObject.FindObjectsOfType<ColorMapper>();
                List<string> dataName = new List<string>();
                foreach (var data in datalist)
                    dataName.Add(data.name);
                return dataName;
            }
        }
        void GetMappingSource()
        {
            foreach (var obj in GameObject.FindObjectsOfType<ColorMapper>())
            {
                if (obj.name == mappingSourceName || System.String.Equals(obj, mappingSourceName))
                {
                    mappingSource = obj;
                    return;
                }
            }
            Debug.LogError("没有找到MappingSource");
        }
        protected override void Action(ColorPoint point)
        {
            point.colorMapper=MappingSource;
            point.gradient=gradient;
            point.StartCoroutine(point.UpdateColorByPos(time,interval));
        }
    }

}


