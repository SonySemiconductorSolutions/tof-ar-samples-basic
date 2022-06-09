/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using UnityEngine;
using TofArSettings;
using System.Linq;

namespace TofArSamples
{
    public class MaterialColorController : ControllerBase
    {
        [Serializable]
        private struct NamedColor
        {
            public string name;
            public UnityEngine.Color color;
        }

        [SerializeField]
        private NamedColor[] colors;

        [SerializeField]
        private Material colorMaterial;

        private UnityEngine.Color defaultColor;

        int index;

        /// <summary>
        /// Process Level index
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index && 0 <= value &&
                    value < colors.Length)
                {
                    index = value;

                    UnityEngine.Color nowColor = MaterialColor;
                    nowColor.r = colors[index].color.r;
                    nowColor.g = colors[index].color.g;
                    nowColor.b = colors[index].color.b;
                    MaterialColor = nowColor;

                    OnChange?.Invoke(Index);
                }
            }
        }

        /// <summary>
        /// Color
        /// </summary>
        public UnityEngine.Color Color
        {
            get
            {
                return colors[index].color;
            }
        }

        /// <summary>
        /// Color list as strings
        /// </summary>
        public string[] ColorNames
        {
            get { return colors.Select((nc) => nc.name).ToArray(); }
        }

        private UnityEngine.Color MaterialColor
        {
            get { return colorMaterial.GetColor("_MainColor"); }
            set { colorMaterial.SetColor("_MainColor", value); }
        }

        private void OnEnable()
        {
            defaultColor = MaterialColor;
        }

        private void OnDisable()
        {
            MaterialColor = defaultColor;
        }

        /// <summary>
        /// Event that is called when Process level is changed
        /// </summary>
        public event ChangeIndexEvent OnChange;

        /// <summary>
        /// Event that is called when Process Level list is updated
        /// </summary>
        //public event UpdateArrayEvent OnUpdateList;
    }
}
