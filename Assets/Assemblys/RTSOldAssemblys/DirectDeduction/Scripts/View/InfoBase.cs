using System;
using System.Collections.Generic;
using UnityEngine;

namespace 指挥端
{
    public class InfoBase
    {
        public GameObject gameObject => transform.gameObject;
        public Transform transform { get; private set; }

        public void InitUI(Transform trans)
        {
            transform = trans;
            this.OnInitUI();
        }

        protected virtual void OnInitUI() { }
    }
}
