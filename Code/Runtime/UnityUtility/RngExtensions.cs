﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.Collections;
using UnityUtility.MathExt;
using UnityUtilityTools;

namespace UnityUtility
{
    public interface IRng
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        float Next(float minValue, float maxValue);
        float Next(float maxValue);
        double NextDouble();
        byte NextByte();
        void NextBytes(byte[] buffer);
        unsafe void NextBytes(byte* arrayPtr, int length);
    }

    /// <summary>
    /// Class for generating random data.
    /// </summary>
    public static class RngExtensions
    {
        /// <summary>
        /// Returns a random float number between -range [inclusive] and range [inclusive].
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Range(this IRng self, float range)
        {
            return self.Next(-range, range);
        }

        /// <summary>
        /// Returns true with chance from 0f to 1f.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Chance(this IRng self, float chance)
        {
            return chance > self.NextDouble();
        }

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<float> weights, float weightOfNone = 0f)
        {
            float target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Count);
            int count = weights.Count + startIndex;
            float sum = 0f;

            for (int i = startIndex; i < count; i++)
            {
                int index = i % weights.Count;

                if (weights[index] + sum >= target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }

#if UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Returns random index of an array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static unsafe int Random(this IRng self, Span<float> weights, float weightOfNone = 0f)
        {
            float target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Length);
            int count = weights.Length + startIndex;
            float sum = 0f;

            for (int i = 0; i < count; i++)
            {
                int index = i % weights.Length;

                if (weights[index] + sum >= target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }
#endif

        /// <summary>
        /// Returns random index of an array contains chance weights or -1 if none of the elements (if <paramref name="weightOfNone"/> more than zero).
        /// </summary>
        public static int Random(this IRng self, IList<int> weights, int weightOfNone = 0)
        {
            int target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Count);
            int count = weights.Count + startIndex;
            int sum = 0;

            for (int i = startIndex; i < count; i++)
            {
                int index = i % weights.Count;

                if (weights[index] + sum > target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }

#if UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Returns random index of an array contains chance weights.
        /// </summary>
        public static unsafe int Random(this IRng self, Span<int> weights, int weightOfNone = 0)
        {
            int target = self.Next(weights.Sum() + weightOfNone);
            int startIndex = self.Next(weights.Length);
            int count = weights.Length + startIndex;
            int sum = 0;

            for (int i = startIndex; i < count; i++)
            {
                int index = i % weights.Length;

                if (weights[index] + sum > target)
                    return index;

                sum += weights[index];
            }

            return -1;
        }
#endif

        /// <summary>
        /// Returns a random flag contains in the specified mask.
        /// </summary>
        /// <param name="length">How many flags of 32bit mask should be considered.</param>
        public static int RandomFlag(this IRng self, int mask, int length)
        {
            int rn = self.Next(BitMask.GetCount(mask, length));

            for (int i = 0; i < length; i++)
            {
                if (BitMask.HasFlag(mask, i))
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
        public static int RandomFlag(this IRng self, BitArrayMask mask)
        {
            int rn = self.Next(mask.GetCount());

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
        public static int Random(this IRng self, int min, int max, int exclusiveValue)
        {
            int value;
            do { value = self.Next(min, max); } while (value == exclusiveValue);
            return value;
        }

        /// <summary>
        /// Returns a random integer number between min [inclusive] and max [exclusive] and which is satisfies the specified condition.
        /// </summary>
        public static int Random(this IRng self, int min, int max, Func<int, bool> condition)
        {
            int value;
            do { value = self.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] and which is satisfies the specified condition.
        /// </summary>
        public static float Random(this IRng self, float min, float max, Func<float, bool> condition)
        {
            float value;
            do { value = self.Next(min, max); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(this IRng self, int mask, int length, Func<int, bool> condition)
        {
            int value;
            do { value = self.RandomFlag(mask, length); } while (!condition(value));
            return value;
        }

        /// <summary>
        /// Returns a random flag contains in the specified mask and which is satisfies the specified condition.
        /// </summary>
        public static int RandomFlag(this IRng self, BitArrayMask mask, Func<int, bool> condition)
        {
            int value;
            do { value = self.RandomFlag(mask); } while (!condition(value));
            return value;
        }
        #endregion

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values.
        /// </summary>
        public static float Descending(this IRng self, float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd * rnd * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to max values.
        /// </summary>
        public static float Ascending(this IRng self, float min, float max)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd.Sqrt() * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min values if curvature &gt; 1f or to max values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Side(this IRng self, float min, float max, float curvature)
        {
            if (min > max)
                throw Errors.MinMax(nameof(min), nameof(max));

            if (curvature < 0f)
                throw Errors.NegativeParameter(nameof(curvature));

            float range = max - min;
            float rnd = self.Next(0f, 1f);
            return rnd.Pow(curvature) * range + min;
        }

        /// <summary>
        /// Returns a random float number between min [inclusive] and max [inclusive] with chance offset to min and max values if curvature &gt; 1f or to average values if curvature &lt; 1f.
        /// </summary>
        /// <param name="curvature">Power of the offset dependency (cannot be negative). If the value is 1f the function has no chance offset because of linear dependency.</param>
        public static float Symmetric(this IRng self, float min, float max, float curvature)
        {
            float average = (max + min) * 0.5f;

            if (self.Next(0, 2) == 0)
                return self.Side(min, average, curvature);
            else
                return self.Side(average, max, 1f / curvature);
        }

        /// <summary>
        /// Returns a random even integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomEven(this IRng self, int min, int max)
        {
            if (!min.IsEven())
            {
                if (max - min < 2)
                    throw Errors.RangeDoesNotContain("even");

                min++;
            }

            return self.Next(min, max) & -2;
        }

        /// <summary>
        /// Returns a random odd integer number between min [inclusive] and max [exclusive].
        /// </summary>
        public static int RandomOdd(this IRng self, int min, int max)
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

            return self.Next(min, max) | 1;
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomByteArray(this IRng self, byte[] buffer)
        {
            self.NextBytes(buffer);
        }

        /// <summary>
        /// Fills a byte array with random values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void RandomByteArray(this IRng self, byte* arrayPtr, int length)
        {
            self.NextBytes(arrayPtr, length);
        }

        /// <summary>
        /// Returns a random point inside a circle with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetInsideUnitCircle(this IRng _)
        {
            return UnityEngine.Random.insideUnitCircle;
        }

        /// <summary>
        /// Returns a random point on the circle line with radius 1.
        /// </summary>
        public static Vector2 GetOnUnitCircle(this IRng self)
        {
            double angle = self.NextDouble() * Math.PI * 2d;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Returns a random point inside a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetInsideUnitSphere(this IRng _)
        {
            return UnityEngine.Random.insideUnitSphere;
        }

        /// <summary>
        /// Returns a random point on the surface of a sphere with radius 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetOnUnitSphere(this IRng _)
        {
            return UnityEngine.Random.onUnitSphere;
        }

        /// <summary>
        /// Returns a random rotation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion GetRandomRot(this IRng _, bool uniformDistribution = false)
        {
            return uniformDistribution ? UnityEngine.Random.rotationUniform : UnityEngine.Random.rotation;
        }

        /// <summary>
        /// Returns a random color32 with the specified alfa.
        /// </summary>
        public static Color32 GetRandomColor(this IRng self, byte alfa)
        {
            Bytes bytes = default;
            int channel1 = self.Next(0, 3);
            int channel2 = self.Random(0, 3, channel1);
            bytes[channel1] = byte.MaxValue;
            bytes[channel2] = self.NextByte();
            bytes[3] = alfa;
            return (Color32)bytes;
        }

        /// <summary>
        /// Returns a random color32 with random alfa.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 GetRandomColor(this IRng self)
        {
            return self.GetRandomColor(self.NextByte());
        }

        /// <summary>
        /// Shuffles the elements of an entire collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IRng self, IList<T> collection)
        {
            CollectionUtility.Shuffle(collection, self);
        }
    }
}
