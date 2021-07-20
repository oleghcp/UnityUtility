﻿using System;
using System.Runtime.CompilerServices;
using UnityUtilityTools;

namespace UnityUtility.Rng
{
    [Serializable]
    public class TimeBytesBasedRng : IRng
    {
        private readonly byte MULT;

        private uint _seed;
        private uint _ticks;

        public TimeBytesBasedRng()
        {
            _seed = (uint)Environment.TickCount;
            MULT = (byte)(_seed % 8 + 2);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return NextInternal(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            if (maxValue < 0)
                throw Errors.NegativeParameter(nameof(maxValue));

            return NextInternal(0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            if (minValue > maxValue)
                throw Errors.MinMax(nameof(minValue), nameof(maxValue));

            return (float)(NextDouble() * ((double)maxValue - minValue) + minValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Next(float maxValue)
        {
            return Next(0f, maxValue);
        }

        public unsafe double NextDouble()
        {
            Span<byte> bytes = stackalloc byte[8];
            NextBytes(bytes);
            ulong rn = *(ulong*)bytes.Ptr;
            rn %= 1000000000000000ul;
            return rn * 0.000000000000001d;
        }

        public byte NextByte()
        {
            return RandomByte();
        }

        public void NextBytes(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = RandomByte();
            }
        }

        public void NextBytes(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = RandomByte();
            }
        }

        private unsafe int NextInternal(int minValue, int maxValue)
        {
            long length = (long)maxValue - minValue;

            if (length <= 256L)
            {
                byte rn = RandomByte();
                return rn % (int)length + minValue;
            }
            else if (length <= 65536L)
            {
                Span<byte> bytes = stackalloc byte[2];
                NextBytes(bytes);
                ushort rn = *(ushort*)bytes.Ptr;
                return rn % (int)length + minValue;
            }
            else
            {
                Span<byte> bytes = stackalloc byte[4];
                NextBytes(bytes);
                uint rn = *(uint*)bytes.Ptr;
                return (int)(rn % length + minValue);
            }
        }

        private byte RandomByte()
        {
            uint newTicks = (uint)Environment.TickCount;
            _ticks = _ticks < newTicks ? newTicks : _ticks + 1;
            _seed = MULT * _seed + _ticks;
            return (byte)(_seed % 256);
        }
    }
}
