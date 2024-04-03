/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023 Sony Semiconductor Solutions Corporation.
 *
 */


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TofAr.V0.Color;

namespace TofArSamples.Recorder
{
    public class ColorRecorder : BinaryRecorder
    {
        public override DataType dataType
        {
            get
            {
                return DataType.Color;
            }
        }

        private SynchronizationContext context;

        void OnEnable()
        {
            context = SynchronizationContext.Current;
            TofArColorManager.OnFrameArrived += OnFrameArrived;
        }

        private void OnFrameArrived(object sender)
        {
            if (TofArColorManager.Instance.ColorData == null) { return; }
            var datetime = DateTime.Now;
            var data = TofArColorManager.Instance.ColorData;
            var texture = TofArColorManager.Instance.ColorTexture;

            context.Post(_ =>
            {
                resizedWidth = texture.width / scale;
                resizedHeight = texture.height / scale;
                if (resizedTexture == null || resizedTexture.width != resizedWidth || resizedTexture.height != resizedHeight)
                {
                    print("Change resized texture");
                    resizedTexture = new Texture2D(resizedWidth, resizedHeight, texture.format, false);
                }
                count++;
                if (count > skip)
                {
                    count = 0;
                    SetData(ResizeDataFrom(data.Data), datetime);
                }
            }, null);
        }


        public int skip = 0;
        private int count = 0;

        public int scale = 1;
        private int resizedWidth;
        private int resizedHeight;
        private Texture2D resizedTexture;
        private int channel = 3; // RGB

        private byte[] ResizeDataFrom(byte[] data)
        {
            var resize = new byte[resizedWidth * resizedHeight * channel];
            for (int y = 0; y < resizedHeight; y++)
            {
                for (int x = 0; x < resizedWidth; x++)
                {
                    resize[channel * (y * resizedWidth + x)] = data[channel * scale * (scale * (resizedHeight - y - 1) * resizedWidth + x)];
                    resize[channel * (y * resizedWidth + x) + 1] = data[channel * scale * (scale * (resizedHeight - y - 1) * resizedWidth + x) + 1];
                    resize[channel * (y * resizedWidth + x) + 2] = data[channel * scale * (scale * (resizedHeight - y - 1) * resizedWidth + x) + 2];
                }
            }
            return resize;
        }


        protected override Dictionary<DateTime, byte[]> CreateMultipleData()
        {
            var data = new Dictionary<DateTime, byte[]>();
            foreach (var item in binaryData)
            {
                resizedTexture.LoadRawTextureData(item.Value);
                data[item.Key] = resizedTexture.EncodeToPNG();
            }
            return data;
        }

        protected override Dictionary<DateTime, byte[]> CreateSingleData()
        {
            var data = new Dictionary<DateTime, byte[]>();
            resizedTexture.LoadRawTextureData(currntData);
            data[currntDataTime] = resizedTexture.EncodeToPNG();
            return data;
        }
    }
}
