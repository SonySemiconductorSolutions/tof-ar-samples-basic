/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2023,2024 Sony Semiconductor Solutions Corporation.
 *
 */

namespace TofArSamples.Recorder
{
    public enum DataType : int
    {
        Hand,
        Body,
        Face,
        BlendShape,
        Depth,
        Color,
        Slam,
    }

    public enum RecordMode : int
    {
        Single = 0,
        Multiple,
    }

    public enum SaveType: int
    {
        Component = 0,
        Timestamp,
    }


    public enum HandType : int
    {
        Left = 0,
        Right
    }
}

