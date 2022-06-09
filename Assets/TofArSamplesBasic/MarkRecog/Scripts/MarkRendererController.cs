/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.MarkRecog;
using TofArSettings;
using UnityEngine;
using System.Linq;

namespace TofArSamples.MarkRecog
{
    public class MarkRendererController : ControllerBase
    {
        [SerializeField]
        private HandBrush brush;

        [SerializeField]
        private GameObject[] brushRenderers;

        [SerializeField]
        private GameObject markDisplay;

        public event ChangeToggleEvent OnMarkDisplayChanged;

        public bool MarkDisplayVisible
        {
            get => markDisplay.activeSelf;
            set
            {
                if(markDisplay.activeSelf != value)
                {
                    markDisplay.SetActive(value);
                    OnMarkDisplayChanged?.Invoke(value);
                }
            }
        }

        public event ChangeIndexEvent OnChangeIndex;

        public delegate void ChangeRendererEvent(GameObject renderer);

        public event ChangeRendererEvent OnChangeRenderer;

        private void Awake()
        {
            RendererNames = brushRenderers.Select((m) => m.name).ToArray();
        }

        [HideInInspector]
        public string[] RendererNames { get; private set; }

        private int index = 0;

        public int Index
        {
            get => index;
            set
            {
                if(value != index &&  value >=0 && value < brushRenderers.Length)
                {
                    index = value;
                GameObject brushRenderer = brushRenderers[index];
                this.brush.SetRenderer(brushRenderer);
                    OnChangeIndex?.Invoke(value);
                    OnChangeRenderer?.Invoke(brushRenderer);
                }
            }
        }
    }
}
