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
    public class MaterialTransparentController : ControllerBase
    {
        [SerializeField]
        private Material tramsparentMaterial;

        public const float transparentMin = 0.0f;

        public const float transparentMax = 1;

        public const float transparentStep = 0.1f;

        public const float transparentDefault = 1;

        private UnityEngine.Color defaultColor;

        float transparent = transparentDefault;

        /// <summary>
        /// Transparency
        /// </summary>
        public float Transparent
        {
            get { return transparent; }
            set
            {
                if (transparent != value && transparentMin <= value && value <= transparentMax)
                {
                    transparent = value;

                    UnityEngine.Color nowColor = MaterialColor;
                    nowColor.a = transparent;
                    MaterialColor = nowColor;

                    OnChange?.Invoke(Transparent);
                }
            }
        }

        private UnityEngine.Color MaterialColor
        {
            get { return tramsparentMaterial.GetColor("_MainColor"); }
            set { tramsparentMaterial.SetColor("_MainColor", value); }
        }

        private void OnEnable()
        {
            defaultColor = MaterialColor;
        }

        private void OnDisable()
        {
            MaterialColor = defaultColor;
        }

        public event ChangeValueEvent OnChange;

    }
}
