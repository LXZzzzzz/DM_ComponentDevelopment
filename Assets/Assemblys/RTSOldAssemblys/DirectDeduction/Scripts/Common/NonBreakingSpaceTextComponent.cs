using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [RequireComponent(typeof(Text))]
    public class NonBreakingSpaceTextComponent : DMonoBehaviour
    {
        public static readonly string no_breaking_space = "\u00A0";
        protected Text text;
        void Awake()
        {
            text = this.GetComponent<Text>();
            text.RegisterDirtyVerticesCallback(OnTextChange);
        }

        public void OnTextChange()
        {
            if (text.text.Contains(" "))
            {
                text.text = text.text.Replace(" ", no_breaking_space);
            }
        }
    }
}
