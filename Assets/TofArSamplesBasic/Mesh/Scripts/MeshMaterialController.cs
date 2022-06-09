/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using System.Text;
using TofArSettings;
using UnityEngine;
using System.Linq;
using TofArSettings.Mesh;

namespace TofArSamples.Mesh
{
    public class MeshMaterialController : ControllerBase
    {
        MeshRenderer dynamicMesh;

        /// <summary>
        /// Materials for the dynamic mesh to use
        /// </summary>
        [SerializeField]
        private Material[] materials;

        /// <summary>
        /// Material index change event
        /// </summary>
        public event ChangeIndexEvent OnChangeIndex;

        public delegate void ChangeMaterialEvent(Material mat);
        /// <summary>
        /// Material value change event
        /// </summary>
        public event ChangeMaterialEvent OnChangeMaterial;

        private void Awake()
        {
            MaterialNames = materials.Select((m) => m.name).ToArray();
            var dynamicMeshObj = FindObjectOfType<TofAr.V0.Mesh.DynamicMesh>();
            dynamicMesh = dynamicMeshObj.GetComponent<MeshRenderer>();
            dynamicMesh.material = materials[index];
            var meshManagerCtrl = FindObjectOfType<MeshManagerController>();
            meshManagerCtrl.OnStreamStartStatusChanged += OnMeshStreamStartStatusChanged;
        }

        /// <summary>
        /// Names of the materials
        /// </summary>
        [HideInInspector]
        public string[] MaterialNames { get; private set; }

        private int index = 0;

        /// <summary>
        /// Index
        /// </summary>
        public int Index
        {
            get => index;
            set
            {
                if (value != index && value >= 0 && value < materials.Length)
                {
                    index = value;
                    dynamicMesh.material = materials[index];
                    OnChangeIndex?.Invoke(value);
                    OnChangeMaterial?.Invoke(materials[index]);
                }
            }
        }

        private void OnMeshStreamStartStatusChanged(bool value)
        {
            dynamicMesh.enabled = value;
        }

    }
}
