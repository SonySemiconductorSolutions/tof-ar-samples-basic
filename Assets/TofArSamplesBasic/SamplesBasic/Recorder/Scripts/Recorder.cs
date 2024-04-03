/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using UnityEngine;

namespace TofArSamples.Recorder
{
    public class Recorder : MonoBehaviour
    {
        public bool Record
        {
            set
            {
                record = value;
                onChangeRecord?.Invoke(value);
                UpdateView(record);
            }
            get
            {
                return record;
            }
        }
        private bool record = true;
        protected bool recording = false;
        protected DateTime startTime;

        public delegate void OnChangeRecordDelegate(bool record);
        public OnChangeRecordDelegate onChangeRecord;

        public virtual DataType dataType
        {
            get
            {
                return DataType.Hand;
            }
        }

        public virtual void StartRectoring()
        {
            ClearData();
            recording = Record;
            startTime = DateTime.Now;
        }

        public virtual void StopRectoring()
        {
            recording = false;
        }

        public virtual void ClearData() { }

        protected virtual void UpdateView(bool show) { }
    }
}
