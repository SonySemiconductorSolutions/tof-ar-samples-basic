/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using UnityEngine;

namespace TofAr.AppLicense
{
    public static class TextImporter
    {
        public static string MergeImport(string fileName)
        {
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            if (asset == null)
            {
                return string.Empty;
            }
            string text = asset.text;

            Resources.UnloadAsset(asset);
            return text;
        }
    }
}