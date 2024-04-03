/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TofAr.V0.Tof;
using TofAr.V0.Plane;
using TofArSettings;
using TofArSettings.Plane;

namespace TofArSamples.Plane
{
    public class PlaneModelController : ControllerBase
    {
        public PlaneArrangement planeArrangement;
        public int planeArrangementIndex = 0;

        public GameObject planeArrangementPrefab;

        public event ChangeToggleEvent OnChangeShow;
        public event ChangeValueEvent OnChangeInterval;
        public event ChangeValueEvent OnChangeKernelSize;
        public event ChangeValueEvent OnChangeMinimumSize;
        public event ChangeValueEvent OnChangeThreshold;

        public const int IntervalMin = 0;
        public const int IntervalMax = 100;
        public const int IntervalStep = 1;
        
        public const float ThresholdMin = 0.0f;
        public const float ThresholdMax = 100.0f;
        public const float ThresholdStep = 1f;
        
        public const float MinimumSizeMin = 0.0f;
        public const float MinimumSizeMax = 1000.0f;
        public const float MinimumSizeStep = 1f;

        public const int KernelSizeMin = 1;
        public const int KernelSizeMax = 21;
        public const int KernelSizeStep = 1;

        private bool isPlaneShown = true;
        public bool IsPlaneShown
        {
            get {return isPlaneShown;}
            set
            {
                this.isPlaneShown = value;
                TogglePlanes(value);
            }
        }

        [SerializeField]
        private int interval = 11;
        [SerializeField]
        private float planeThreshold = 10.0f;
        [SerializeField]
        private float minimumSize = 0.0f;
        [SerializeField]
        private int kernelSize = 5;

        /// <summary>
        /// TBD
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set
            {
                this.interval = value;
                this.planeArrangement.Interval = value;
                OnChangeInterval?.Invoke(value);
            }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public float PlaneThreshold
        {
            get { return planeThreshold; }
            set
            {
                this.planeThreshold = value;
                this.planeArrangement.PlaneThreshold = value;
                OnChangeThreshold?.Invoke(value);
            }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public float MinimumSize
        {
            get { return minimumSize; }
            set
            {
                this.minimumSize = value;
                this.planeArrangement.MinSize = value;
                OnChangeMinimumSize?.Invoke(value);
            }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public int KernelSize
        {
            get { return kernelSize; }
            set
            {
                this.kernelSize = value;
                this.planeArrangement.KSize = value;
                OnChangeKernelSize?.Invoke(value);
            }
        }

        protected override void Start()
        {
            base.Start();

            TofArPlaneManager.OnPlaneAdded += OnPlaneAdded;
            TofArPlaneManager.OnPlaneRemoved += OnPlaneRemoved;

            if (planeArrangement != null)
            {
                UpdateSettings();
                planeArrangement.Select();     
            }       

            previousProcessTargets = TofArTofManager.Instance.ProcessTargets;
            TofArTofManager.Instance.SetProcessTargets(true, true, true);
        }

        private void OnDestroy()
        {
            if (previousProcessTargets != null && TofArTofManager.Instance != null)
            {
                TofArTofManager.Instance.SetProcessTargets(previousProcessTargets.processDepth, previousProcessTargets.processConfidence, previousProcessTargets.processPointCloud);
            }
            TofArPlaneManager.OnPlaneAdded -= OnPlaneAdded;
            TofArPlaneManager.OnPlaneRemoved -= OnPlaneRemoved;
        }

        private void UpdateSettings()
        {
            if (planeArrangement != null)
            {
                MinimumSize = planeArrangement.MinSize * 1000f;
                Interval = planeArrangement.Interval;
                KernelSize = planeArrangement.KSize;
                PlaneThreshold = planeArrangement.PlaneThreshold;

                UpdatePlaneIndex();

                FindObjectOfType<PlaneInfoSettings>().ChangeSelectedPlane(planeArrangement);
            }

        }

        public void OnDropdownDepthSelected(CameraConfigurationProperty conf, bool isProcessTexture)
        {
            TofArPlaneManager.Instance.StopStream();
            TofArTofManager.Instance.StopStream();
            if (conf != null)
            {
                TofArTofManager.Instance.StartStream(conf, isProcessTexture);
                TofArPlaneManager.Instance.StartStream();
            }
        }

        private void TogglePlanes(bool showPlanes)
        {
            var planeArray = Resources.FindObjectsOfTypeAll(typeof(PlaneArrangement)) as PlaneArrangement[];
            if (planeArray != null)
            {
                foreach (var plane in planeArray)
                {
                    plane.gameObject.SetActive(showPlanes);
                }
            }
        }

        #region MULTI_PLANE

        public GameObject dragObject;
        public RemoveButtonHandler btnRemove;

        private bool isSelecting = false;
        private bool isDragging = false;
        private bool hoverOverRemoveButton = false;
        private ProcessTargetsProperty previousProcessTargets;

        public void AddPlane()
        {
            TofArPlaneManager.Instance.AddPlane();
        }

        public void RemovePlane()
        {
            if (planeArrangement != null)
            {
                // remove configuration
                int idx = planeArrangement.transform.GetSiblingIndex();

                TofArPlaneManager.Instance.RemovePlane(idx);
            }
        }

        public void OnPlaneAdded(int idx)
        {
            GameObject newPlane = GameObject.Instantiate(planeArrangementPrefab, new Vector3(0, 0, 2), Quaternion.identity);

            PlaneArrangement newPlaneArrangement = newPlane.GetComponent<PlaneArrangement>();

            if (newPlaneArrangement != null)
            {
                newPlaneArrangement.CenterPosition = new Vector2(0.5f, 0.5f);

                if (planeArrangement != null)
                    planeArrangement.Deselect();

                planeArrangement = newPlaneArrangement;

                planeArrangement.Select();

                UpdateSettings();
            }
        }

        public void OnPlaneRemoved(int idx)
        {
            var planeRoot = planeArrangement.transform.parent;

            planeArrangement.transform.SetParent(null);
            Destroy(planeArrangement.gameObject);

            if (planeRoot.childCount > 0)
            {
                planeArrangement = planeRoot.GetChild(0).GetComponent<PlaneArrangement>();
                planeArrangement.Select();
            }
            else
            {
                planeArrangement = null;
            }

            UpdateSettings();
        }

        public void BeginDrag()
        {
            if (planeArrangement != null && isSelecting)
            {
                var plane = planeArrangement.transform;

                isDragging = true;

                dragObject.SetActive(true);

                SelectPlane(plane);

                btnRemove.gameObject.GetComponent<Image>().enabled = true;
            }
        }

        public void Dragging(Vector2 position)
        {
            // update icon position
            dragObject.GetComponent<RectTransform>().position = position;
        }

        public void EndDrag(Vector2 position)
        {
            // if above delete icon, call RemovePlane
            dragObject.SetActive(false);

            if (isDragging)
            {
                if (hoverOverRemoveButton)
                    RemovePlane();
                else
                {
                    if (position.x >= 0 && 1 >= position.x
                        && position.y >= 0 && 1 >= position.y)
                    {
                        planeArrangement.CenterPosition = position;
                    }

                }
            }

            btnRemove.gameObject.GetComponent<Image>().enabled = false;

            isDragging = false;
            isSelecting = false;
        }


        public void PointerDown(Vector2 point)
        {
            isSelecting = false;

            PlaneArrangement[] planeObjects = FindObjectsOfType<PlaneArrangement>();

            foreach (PlaneArrangement plane in planeObjects)
            {
                var centerPos = plane.CenterPosition;

                float distance = Vector2.Distance(point, centerPos);

                if (distance < 0.1f)
                {
                    isSelecting = true;

                    SelectPlane(plane.transform);

                    return;
                }
            }
        }

        public void StartHoverRemoveButton()
        {
            Debug.Log("HOVER start");
            hoverOverRemoveButton = true;
        }

        public void FinishHoverRemoveButton()
        {
            Debug.Log("Hover end");
            hoverOverRemoveButton = false;
        }

        private void SelectPlane(Transform plane)
        {
            if (planeArrangement != null)
                planeArrangement.Deselect();

            planeArrangement = plane.gameObject.GetComponent<PlaneArrangement>();

            planeArrangement.Select();

            // update settings
            UpdateSettings();
        }

        private void UpdatePlaneIndex()
        {
            planeArrangementIndex = planeArrangement.transform.GetSiblingIndex();
        }

        #endregion
    }
}
