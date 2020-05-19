﻿namespace UnityUtility.Rng
{
    public interface IRng
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        float NextFloat(float minValue, float maxValue);
        double NextDouble();
        byte NextByte();
        void NextBytes(byte[] buffer);
        unsafe void NextBytes(byte* arrayPtr, int length);
    }
}