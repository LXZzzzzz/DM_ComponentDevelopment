using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    public class DynamicPropertyUnitLimitItem : DynamicPropertyItem
    {
        [HideInInspector]
        public Text m_textLimit;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_textLimit = transform.Find("TextLimit").GetComponent<Text>();
        }
        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}
