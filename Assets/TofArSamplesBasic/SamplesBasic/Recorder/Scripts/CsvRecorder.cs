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
    public class CsvRecorder : Recorder
    {
        [SerializeField]
        private string fileName = "Data.csv";
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        private string header = "";
        public string Header
        {
            get
            {
                if (header.Length == 0)
                {
                    header = CreateHeader();
                }
                return header;
            }
        }


        protected Dictionary<DateTime, string> csvData = new Dictionary<DateTime, string>();


        protected DateTime currntDataTime;
        protected string currntData;

        protected int dataLength = 0;

        public override void ClearData()
        {
            csvData.Clear();
        }

        protected virtual string CreateHeader()
        {
            var result = "Timestamp";
            for (int i = 0; i < dataLength; i++)
            {
                result += ",Value_" + i;
            }
            return result;
        }

        protected void SetData(string data)
        {
            currntDataTime = DateTime.Now;
            currntData = data;
            if (!recording) { return; }
            csvData[DateTime.Now] = data;
        }

        public Dictionary<DateTime, string> GetData(RecordMode recordMode)
        {
            if (recordMode == RecordMode.Single)
            {
                return new Dictionary<DateTime, string> { { currntDataTime, currntData } };
            }
            else
            {
                return csvData;
            }
        }
    }
}


