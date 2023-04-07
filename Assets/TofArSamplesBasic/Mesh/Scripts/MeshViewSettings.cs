/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022,2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections.Generic;
using TofArSettings;
using TofArSettings.UI;
using UnityEngine;
using UnityEngine.Events;
using UI = TofArSettings.UI;

namespace TofArSamples.Mesh
{
    public class MeshViewSettings : ImageViewSettings
    {
        MeshMaterialController meshMaterialController;
        OcclusionObjectController occlusionObjectController;

        [SerializeField]
        bool material = true;

        /// <summary>
        /// Toggle occlusion
        /// </summary>
        [SerializeField]
        bool occlusion = true;

        UI.ItemToggle itemCube, itemPlane, itemHandOcclusion;
        UI.ItemDropdown itemMaterials;

        List<UnityAction> list;

        [System.Serializable]
        public class AddControllersEvent : UnityEvent<SettingsPanel> { }
        AddControllersEvent addControllersEvent = new AddControllersEvent();

        protected override void Awake()
        {
            base.Awake();

            meshMaterialController = GetComponent<MeshMaterialController>();
            meshMaterialController.enabled = material;
            occlusionObjectController = GetComponent<OcclusionObjectController>();
            occlusionObjectController.enabled = occlusion;
        }

        protected override void PrepareUI()
        {
            base.PrepareUI();

            list = new List<UnityAction>(uiOrder);
            if (material)
            {
                list.Add(MakeUIMaterial);
                controllers.Add(meshMaterialController);
            }

            if (occlusion)
            {
                list.Add(MakeUIOcclusion);
                controllers.Add(occlusionObjectController);
            }

            addControllersEvent.Invoke(settings);

            // Set UI order
            uiOrder = list.ToArray();
        }

        /// <summary>
        /// Make MeshMaterial UI
        /// </summary>
        void MakeUIMaterial()
        {
            itemMaterials = settings.AddItem("Mesh material", meshMaterialController.MaterialNames,
                meshMaterialController.Index, ChangeMeshMaterial);

            meshMaterialController.OnChangeIndex += (index) =>
            {
                itemMaterials.Index = index;
            };
        }

        /// <summary>
        /// Change material of mesh
        /// </summary>
        /// <param name="index"></param>
        void ChangeMeshMaterial(int index)
        {
            meshMaterialController.Index = index;
        }

        /// <summary>
        /// Make UI for occlusion object
        /// </summary>
        void MakeUIOcclusion()
        {
            itemCube = settings.AddItem("Cube", occlusionObjectController.IsCube,
                ChangeCube);

            occlusionObjectController.OnChangeCube += (onOff) =>
            {
                itemCube.OnOff = onOff;
            };

            ChangeCube(occlusionObjectController.IsCube);

            itemPlane = settings.AddItem("Plane", occlusionObjectController.IsPlane,
                ChangePlane);

            occlusionObjectController.OnChangePlane += (onOff) =>
            {
                itemPlane.OnOff = onOff;
            };
        }

        /// <summary>
        /// Toggle cube display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangeCube(bool onOff)
        {
            occlusionObjectController.IsCube = onOff;
        }

        /// <summary>
        /// Toggle plane display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        void ChangePlane(bool onOff)
        {
            occlusionObjectController.IsPlane = onOff;
        }

        /// <summary>
        /// Controller registration event
        /// </summary>
        /// <param name="unityAction">Argumentless delegate</param>
        public void AddListenerAddControllersEvent(UnityAction<SettingsPanel> unityAction)
        {
            addControllersEvent.AddListener(unityAction);
        }

        /// <summary>
        /// Register to UIOrderList
        /// </summary>
        /// <param name="controller">RecordController</param>
        public void SetUIOrderList(UnityAction unityAction)
        {
            list.Add(unityAction);
        }

        /// <summary>
        /// Register to Controller
        /// </summary>
        /// <param name="controller">ControllerBase</param>
        public void SetController(ControllerBase controller)
        {
            controllers.Add(controller);
        }

        /// <summary>
        /// Get settings
        /// </summary>
        /// <returns></returns>
        public SettingsPanel GetSettings()
        {
            return settings;
        }
    }
}
