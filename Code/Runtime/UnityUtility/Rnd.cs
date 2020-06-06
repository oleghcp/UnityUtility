﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Tools;
using UnityEngine;
using UnityUtility.BitMasks;
using UnityUtility.Collections;
using UnityUtility.Collections.Unsafe;
using UnityUtility.MathExt;
using UnityUtility.Rng;

namespace UnityUtility
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

    /// <summary>
    /// Class for generating random data.
    /// </summary>
    public class Rnd
    {
        private IRng m_rng;

        public IRng Generator
        {
            get { return m_rng; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                m_rng = value;
            }
        }

        public Rnd(IRng randomizer)
        {
            Generator = randomizer;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public int Random(int min, int max)
        {
            return m_rng.Next(min, max);
        }

        /// <summary>
        /// Returns a random integer number between zero [inclusive] and max [exclusive].
        /// </summary>
        public int Random(int max)
        {
            return m_rng.Next(max);
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive].
        /// </summary>
        public float Random(float min, float max)
        {
            return m_rng.NextFloat(min, max);
        }

        /// <summary>
        /// Returns a random float number between zero [inclusive] and max [inclusive].
        /// </summary>
        public float Random(float max)
        {
            return m_rng.NextFloat(0f, max);
        }

        /// <summary>
        /// Returns a random float number between -range [inclusive] and range [inclusive].
        /// </summary>
        public float Range(float range)
        {
            return m_rng.NextFloat(-range, range);
        }

        /// <summary>
        /// Returns true with chance from 0f to 1f.
        /// </summary>
        public bool Chance(float chance)
        {
            return chance > m_rng.NextDouble();
        }

        /// <summary>
        /// Returns true with chance represented by Percent from 0 to 100.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Chance(Percent chance)
        {
            return Chance(chance.ToRatio());
        }

        //public  int RandomTemp(float[] weights)
        //{
        //    float sum = weights.Sum();
        //    float factor = 1f / sum;

        //    for (int i = 0; i < weights.Length; i++)
        //    {
        //        if (Chance(weights[i] * factor))
        //            return i;

        //        sum -= weights[i];
        //        factor = 1f / sum;
        //    }

        //    return -1;
        //}

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public int Random(float[] weights)
        {
            double rnd = m_rng.NextDouble() * weights.Sum();
            double sum = 0d;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public unsafe int Random(float* weights, int length)
        {
            double rnd = m_rng.NextDouble() * ArrayUtility.Sum(weights, length);
            double sum = 0d;

            for (int i = 0; i < length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public int Random(int[] weights)
        {
            int rnd = m_rng.Next(weights.Sum());
            int sum = 0;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 for none of the elements (if all weights are zero). Each element cannot be less than 0f.
        /// </summary>
        public unsafe int Random(int* weights, int length)
        {
            int rnd = m_rng.Next(ArrayUtility.Sum(weights, length));
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                if (weights[i] + sum > rnd)
                    return i;

                sum += weights[i];
            }

            return -1;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        /// <param name="length">How many flags of 32bit mask should be considered.</param>
        public int RandomFlag(int mask, int length)
        {
            int rn = Random(mask.GetCount(length));

            for (int i = 0; i < length; i++)
            {
                if (mask.ContainsFlag(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw Errors.EmptyMask();
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        public int RandomFlag(BitArrayMask mask)
        {
            int rn = Random(mask.GetCount());

            for (int i = 0; i < mask.Length; i++)
            {
                if (mask.Get(i))
                {
                    if (rn-- == 0)
                        return i;
                }
            }

            throw Errors.EmptyMask();
        }

        #region randoms by condition
        //TODO: need check of impossible condition

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is not equal to exclusiveValue.
        /// </summary>
        public int Random(int min, int max, int exclusiveValue)
        {
            int value;
            do { value = m_rng.Next(min, max); } while (value == exclusiveValue);
            return value;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is satisfies the specified condition.
        /// </summary>
        public int Random(int min, int max, Func<int, bool> condition)
        {
            int value;
            do { value = m_rng.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] and which is satisfies the specified condition.
        /// </summary>
        public float Random(float min, float max, Func<float, bool> condition)
        {
            float value;
            do { value = m_rng.NextFloat(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public int RandomFlag(int mask, int length, Func<int, bool> condition)
        {
            int value;
            do { value = RandomFlag(mask, length); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public int RandomFlag(BitArrayMask mask, Func<int, bool> condition)
        {
            int value;
            do { value = RandomFlag(mask); } while (!condition(value));
            return value;
        }
        #endregion

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values.
        /// </summary>
        public float Descending(float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = m_rng.NextFloat(0f, 1f);
            return rnd * rnd * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to max values.
        /// </summary>
        public float Ascending(float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = m_rng.NextFloat(0f, 1f);
            return rnd.Sqrt() * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values if curvature &gt; 1f or to max values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public float Side(float min, float max, float curvature)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            if (curvature < 0f)
                throw Errors.NegativeParameter(nameof(curvature));

            float range = max - min;
            float rnd = m_rng.NextFloat(0f, 1f);
            return rnd.Pow(curvature) * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min and max values if curvature &gt; 1f or to average values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public float Symmetric(float min, float max, float curvature)
        {
            float average = (max + min) * 0.5f;

            if (m_rng.Next(0, 2) == 0)
                return Side(min, average, curvature);
            else
                return Side(average, max, 1f / curvature);
        }

        /// <summary>
        /// Returns a random even integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public int RandomEven(int min, int max)
        {
            if (!min.IsEven())
            {
                if (max - min < 2)
                    throw Errors.RangeDoesNotContain("even");

                min++;
            }

            return m_rng.Next(min, max) & -2;
        }

        /// <summary>
        /// Returns a random odd integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public int RandomOdd(int min, int max)
        {
            if (max.IsEven())
            {
                if (min == max)
                    throw Errors.RangeDoesNotContain("odd");
            }
            else if (max - min < 2)
            {
                throw Errors.RangeDoesNotContain("odd");
            }

            return m_rng.Next(min, max) | 1;
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        public void RandomByteArray(byte[] buffer)
        {
            m_rng.NextBytes(buffer);
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        public unsafe void RandomByteArray(byte* arrayPtr, int length)
        {
            m_rng.NextBytes(arrayPtr, length);
        }

        /// <summary>
        /// Returns a random point inside a circle with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 GetInsideUnitCircle()
        {
            return UnityRng.GetInsideUnitCircle();
        }

        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public Vector2 GetOnUnitCircle()
        {
            double angle = m_rng.NextDouble() * Math.PI * 2d;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 GetInsideUnitSphere()
        {
            return UnityRng.GetInsideUnitSphere();
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 GetOnUnitSphere()
        {
            return UnityRng.GetOnUnitSphere();
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion GetRandomRot(bool uniformDistribution = false)
        {
            return UnityRng.GetRandomRot(uniformDistribution);
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public Color32 GetRandomColor(byte alfa)
        {
            Bytes bytes = default;
            int channel1 = m_rng.Next(3);
            int channel2 = Random(0, 3, channel1);
            bytes[channel1] = byte.MaxValue;
            bytes[channel2] = m_rng.NextByte();
            bytes[3] = alfa;
            return (Color32)bytes;
        }

        /// <summary>
        /// Returns a random color32 with random alfa.
        /// </summary>
        public Color32 GetRandomColor()
        {
            return GetRandomColor(m_rng.NextByte());
        }
    }
}
