/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections.Generic;
using TofArSettings;
using UnityEngine;
using UI = TofArSettings.UI;

namespace TofArSamples
{
    public abstract class ImageViewController : ControllerBase
    {
        [SerializeField]
        float offset = 20;

        /// <summary>
        /// View mode type
        /// </summary>
        public enum Mode : int
        {
            FullScreen,
            PictureInPicture,
            Hide
        }

        /// <summary>
        /// Picture-in-picture view position type
        /// </summary>
        public enum Anchor : int
        {
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
            Center
        }

        int modeIndex;

        /// <summary>
        /// View mode index
        /// </summary>
        public int ModeIndex
        {
            get { return modeIndex; }
            set
            {
                if (value != modeIndex && 0 <= value &&
                    value < ModeList.Length)
                {
                    modeIndex = value;
                    ViewMode = ModeList[ModeIndex];

                    OnChangeMode?.Invoke(ModeIndex);
                }
            }
        }

        Mode mode = Mode.Hide;

        /// <summary>
        /// View mode
        /// </summary>
        public Mode ViewMode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    var lastViewMode = mode;

                    SetViewMode(value);

                    if (lastViewMode == Mode.PictureInPicture)
                    {
                        if (minimizedViewPerAnchor[anchor] > 0)
                        {
                            minimizedViewPerAnchor[anchor]--;
                        }
                    }
                    else if (mode == Mode.PictureInPicture)
                    {
                        minimizedViewPerAnchor[anchor]++;
                    }

                    OnUpdateView?.Invoke();
                    //UpdateView();
                    ModeIndex = Utils.Find(mode, ModeList);
                }
            }
        }

        private void SetViewMode(Mode newViewMode)
        {
            mode = newViewMode;
        }

        int anchorIndex;

        /// <summary>
        /// View anchor index
        /// </summary>
        public int AnchorIndex
        {
            get { return anchorIndex; }
            set
            {
                var anchorList = (Anchor[])Enum.GetValues(typeof(Anchor));
                if (value != anchorIndex && 0 <= value &&
                    value < anchorList.Length)
                {
                    anchorIndex = value;
                    ViewAnchor = anchorList[anchorIndex];

                    OnChangeAnchor?.Invoke(AnchorIndex);
                }
            }
        }

        Anchor anchor = Anchor.RightBottom;

        /// <summary>
        /// Picture-in-picture view position
        /// </summary>
        public Anchor ViewAnchor
        {
            get { return anchor; }
            set
            {
                if (anchor != value)
                {
                    var lastAnchor = anchor;
                    anchor = value;

                    if (mode == Mode.PictureInPicture)
                    {
                        if (minimizedViewPerAnchor[lastAnchor] > 0)
                        {
                            minimizedViewPerAnchor[lastAnchor]--;
                        }
                        minimizedViewPerAnchor[anchor]++;
                    }

                    OnUpdateView?.Invoke();

                    //UpdateView();
                    AnchorIndex = Utils.Find(anchor, AnchorList);
                }
            }
        }

        /// <summary>
        /// View mode list
        /// </summary>
        public Mode[] ModeList { get; private set; }

        string[] modeNames;

        /// <summary>
        /// View mode list as strings
        /// </summary>
        public string[] ModeNames
        {
            get { return modeNames; }
        }

        /// <summary>
        /// Picture-in-picture view position list
        /// </summary>
        public Anchor[] AnchorList
        {
            get { return (Anchor[])Enum.GetValues(typeof(Anchor)); }
        }

        /// <summary>
        /// Picture-in-picture display position list as strings
        /// </summary>
        public string[] AnchorNames
        {
            get { return Enum.GetNames(typeof(Anchor)); }
        }

        /// <summary>
        /// Event that is called when view mode is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeMode;

        /// <summary>
        /// Event that is called when picture-in-picture view mode position is changed
        /// </summary>
        public event ChangeIndexEvent OnChangeAnchor;

        private static Dictionary<Anchor, int> minimizedViewPerAnchor = new Dictionary<Anchor, int>()
        {
            {Anchor.Center, 0 },
            {Anchor.LeftBottom, 0 },
            {Anchor.LeftTop, 0 },
            {Anchor.RightBottom, 0 },
            {Anchor.RightTop, 0 }
        };
        protected int minimizePositionIndex = 0;

        private delegate void ViewUpdateEvent();
        private static event ViewUpdateEvent OnUpdateView;

        [SerializeField]
        protected QuadFitter fitter;
        protected CameraManagerController mgrCtrl;
        ScreenRotateController scRotCtrl;
        UI.Toolbar toolbar;
        RectTransform rawImgRt;
        Vector2 defaultImgSize;

        protected virtual void Awake()
        {
            // mgrCtrl is obtained in a child class
            scRotCtrl = FindObjectOfType<ScreenRotateController>();
            minimizePositionIndex = GetPositionIndex();
        }

        protected virtual void OnEnable()
        {
            scRotCtrl.OnRotateScreen += OnRotateScreen;
            if (mgrCtrl)
            {
                mgrCtrl.OnChangeAfter += AdjustAspect;
                AdjustAspect();
                mgrCtrl.OnChangeAfter += ChangeViewMode;
                ChangeViewMode();
            }

            ImageViewController.OnUpdateView += UpdateView;

        }

        void OnDisable()
        {
            scRotCtrl.OnRotateScreen -= OnRotateScreen;
            if (mgrCtrl)
            {
                mgrCtrl.OnChangeAfter -= AdjustAspect;
                mgrCtrl.OnChangeAfter -= ChangeViewMode;
            }

            ImageViewController.OnUpdateView -= UpdateView;
        }

        protected override void Start()
        {
            toolbar = FindObjectOfType<UI.Toolbar>();

            if (ExistRawImage())
            {
                rawImgRt = GetRawImageRt();
                defaultImgSize = rawImgRt.sizeDelta;
            }

            // Create view mode list
            var listMode = new List<Mode>();
            if (ExistQuad())
            {
                // Enable fullscreen if Quad exists
                listMode.Add(Mode.FullScreen);
            }

            if (ExistRawImage())
            {
                // Enable fullscreen and picture-in-picture display if RawImage exists
                if (!listMode.Contains(Mode.FullScreen))
                {
                    listMode.Add(Mode.FullScreen);
                }

                listMode.Add(Mode.PictureInPicture);
            }

            listMode.Add(Mode.Hide);
            ModeList = listMode.ToArray();

            // Make view mode name list
            modeNames = new string[ModeList.Length];
            for (int i = 0; i < ModeList.Length; i++)
            {
                modeNames[i] = ModeList[i].ToString();
            }

            // Get initial values
            modeIndex = Utils.Find(ViewMode, ModeList);
            if (modeIndex < 0)
            {
                modeIndex = 0;
            }

            anchorIndex = Utils.Find(ViewAnchor, AnchorList);

            // Switch view
            UpdateView();

            base.Start();
        }

        /// <summary>
        /// Switch view according to the camera setting values
        /// </summary>
        /// <param name="index">Index of camera setting values</param>
        public void ChangeViewMode(int index = 0)
        {
            if (ModeList != null)
            {
                if (index == 0)
                {
                    ViewMode = Mode.Hide;
                }
                else
                {
                    ViewMode = Mode.FullScreen;
                }
            }
        }

        /// <summary>
        /// Adjust aspect ratio of view
        /// </summary>
        /// <param name="index">Index of camera setting values (not used)</param>
        protected virtual void AdjustAspect(int index = 0)
        {
        }

        /// <summary>
        /// Check if ViewQuad exists in scene
        /// </summary>
        /// <returns>Exists/Does not exist</returns>
        protected virtual bool ExistQuad()
        {
            return false;
        }

        /// <summary>
        /// Check if ViewRawImage exists in scene
        /// </summary>
        /// <returns>Exists/Does not exist</returns>
        protected virtual bool ExistRawImage()
        {
            return false;
        }

        /// <summary>
        /// Get RectTransform of ViewRawImage
        /// </summary>
        /// <returns>RectTransform of ViewRawImage</returns>
        protected virtual RectTransform GetRawImageRt()
        {
            return null;
        }

        /// <summary>
        /// Change maximize setting of RawImage
        /// </summary>
        /// <param name="onOff">Maximize/Do not maximize</param>
        protected virtual void ChangeMaximize(bool onOff)
        {
        }

        /// <summary>
        /// Toggle Quad display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        protected virtual void ShowQuad(bool onOff)
        {
            // Toggle operation of ViewFitter as well
            if (fitter)
            {
                fitter.enabled = onOff;
            }
        }

        /// <summary>
        /// Toggle RawImage display
        /// </summary>
        /// <param name="onOff">Show/Hide</param>
        protected virtual void ShowRawImage(bool onOff)
        {
        }

        /// <summary>
        /// Event that is called when the screen is rotated
        /// </summary>
        /// <param name="ori"></param>
        void OnRotateScreen(ScreenOrientation ori)
        {
            UpdateView();
        }

        /// <summary>
        /// Update view
        /// </summary>
        void UpdateView()
        {
            if (ViewMode == Mode.FullScreen)
            {
                ShowQuad(true);
                if (ExistQuad())
                {
                    // Show Quad and hide RawImage
                    ShowRawImage(false);
                }
                else if (ExistRawImage())
                {
                    // If there is no Quad, display RawImage in fullscreen
                    ShowRawImage(true);
                    ChangeMaximize(true);
                    ApplyAnchor(Anchor.Center);
                }
            }
            else if (ViewMode == Mode.PictureInPicture)
            {
                // Hide Quad, disable RawImage fullscreen and display RawImage
                ShowQuad(false);
                ShowRawImage(true);
                if (ExistRawImage())
                {
                    ChangeMaximize(false);
                    rawImgRt.sizeDelta = defaultImgSize;
                }

                ApplyAnchor(ViewAnchor);
            }
            else if (ViewMode == Mode.Hide)
            {
                // Hide
                ShowQuad(false);
                ShowRawImage(false);
            }
        }

        /// <summary>
        /// Change picture-in-picture display position
        /// </summary>
        /// <param name="anchor">Picture-in-picture display position</param>
        void ApplyAnchor(Anchor anchor)
        {
            if (!rawImgRt)
            {
                return;
            }

            Vector2 anchorMinMax = new Vector2(0.5f, 0.5f);
            Vector2 pos = Vector2.zero;
            Vector2 posOffset = rawImgRt.sizeDelta / 2;

            posOffset.x += offset;
            posOffset.y += offset;



            // Adjust according to screen orientation and screen size
            if ((posOffset.x > posOffset.y && scRotCtrl.IsPortrait) ||
                (posOffset.x < posOffset.y && !scRotCtrl.IsPortrait))
            {
                float tmp = posOffset.x;
                posOffset.x = posOffset.y;
                posOffset.y = tmp;
            }

            int posIndex = 0;
            if (ViewMode == Mode.PictureInPicture)
            {
                posIndex = minimizePositionIndex;
                if (posIndex >= minimizedViewPerAnchor[anchor])
                {
                    posIndex = (minimizedViewPerAnchor[anchor] - 1);
                }
            }

            // Move while accounting for the toolbar position
            switch (anchor)
            {
                case Anchor.Center:
                    if (ViewMode == Mode.PictureInPicture && minimizedViewPerAnchor[anchor] > 0)
                    {
                        float a = (minimizedViewPerAnchor[anchor]) - 1;
                        float b = a / 2f;
                        float offs = (float)posIndex - b;
                        if (scRotCtrl.IsPortrait)
                        {
                            pos.x += (rawImgRt.sizeDelta.y + offset) * offs;
                        }
                        else
                        {
                            pos.x += (rawImgRt.sizeDelta.x + offset) * offs;
                        }
                    }
                    break;
                case Anchor.LeftTop:
                    anchorMinMax = new Vector2(0, 1);
                    pos.x = posOffset.x;
                    pos.y = -posOffset.y;
                    if (scRotCtrl.IsPortrait)
                    {
                        pos.x += (rawImgRt.sizeDelta.y + offset) * posIndex;
                    }
                    else
                    {
                        pos.x += (rawImgRt.sizeDelta.x + offset) * posIndex;
                    }
                    break;

                case Anchor.LeftBottom:
                    anchorMinMax = new Vector2(0, 0);
                    pos.x = posOffset.x;
                    pos.y = posOffset.y;
                    if (scRotCtrl.IsPortrait)
                    {
                        pos.y += toolbar.BarWidth;
                        pos.x += (rawImgRt.sizeDelta.y + offset) * posIndex;
                    }
                    else
                    {
                        pos.x += (rawImgRt.sizeDelta.x + offset) * posIndex;
                    }
                    break;

                case Anchor.RightTop:
                    anchorMinMax = new Vector2(1, 1);
                    pos.x = -posOffset.x;
                    pos.y = -posOffset.y;
                    if (scRotCtrl.IsPortrait)
                    {
                        pos.x -= (rawImgRt.sizeDelta.y + offset) * posIndex;
                    }
                    else
                    {
                        pos.x -= toolbar.BarWidth;
                        pos.x -= (rawImgRt.sizeDelta.x + offset) * posIndex;
                    }

                    break;

                case Anchor.RightBottom:
                    anchorMinMax = new Vector2(1, 0);
                    pos.x = -posOffset.x;
                    pos.y = posOffset.y;
                    if (scRotCtrl.IsPortrait)
                    {
                        pos.y += toolbar.BarWidth;
                        pos.x -= (rawImgRt.sizeDelta.y + offset) * posIndex;
                    }
                    else
                    {
                        pos.x -= toolbar.BarWidth;
                        pos.x -= (rawImgRt.sizeDelta.x + offset) * posIndex;
                    }
                    break;
            }

            rawImgRt.anchorMin = anchorMinMax;
            rawImgRt.anchorMax = anchorMinMax;
            rawImgRt.pivot = new Vector2(0.5f, 0.5f);
            rawImgRt.anchoredPosition = pos;


        }

        protected abstract int GetPositionIndex();
    }
}
