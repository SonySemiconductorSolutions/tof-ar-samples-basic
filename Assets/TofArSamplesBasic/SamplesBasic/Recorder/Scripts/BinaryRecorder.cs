/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TofArSamples.Recorder
{
    public class BinaryRecorder : Recorder
    {
        [SerializeField]
        private string folderName = "Data";
        public string FolderName
        {
            get
            {
                return folderName;
            }
        }

        [SerializeField]
        private string extension = "bin";
        public string Extension
        {
            get
            {
                return extension;
            }
        }

        protected Dictionary<DateTime, byte[]> binaryData = new Dictionary<DateTime, byte[]>();

        protected DateTime currntDataTime;
        protected byte[] currntData;

        public delegate void OnDataStoreDelegate(int size);
        public OnDataStoreDelegate onDataStored;

        public override void ClearData()
        {
            ClearBinaryData();
        }

        protected virtual void ClearBinaryData()
        {
            binaryData.Clear();
        }


        protected void SetData(byte[] bytes, DateTime dateTime)
        {
            currntDataTime = dateTime;
            currntData = bytes;
            if (!recording) { return; }
            binaryData[dateTime] = bytes;
            onDataStored?.Invoke(bytes.Length);
        }

        public Dictionary<DateTime, byte[]> GetData(RecordMode recordMode)
        {
            if (recordMode == RecordMode.Single)
            {
                return CreateSingleData();
            }
            else if (recordMode == RecordMode.Multiple)
            {
                return CreateMultipleData();
            }
            return null;
        }

        protected virtual Dictionary<DateTime, byte[]> CreateMultipleData()
        {
            return binaryData;
        }

        protected virtual Dictionary<DateTime, byte[]> CreateSingleData()
        {
            return new Dictionary<DateTime, byte[]> { { currntDataTime, currntData } };
        }
    }
}
