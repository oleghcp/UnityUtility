using System;
using System.Collections.Generic;
using UnityUtility;

namespace UnityUtilityTools
{
    public static class Helper
    {
        internal static float GetRatio(float numerator, float denominator)
        {
            if (denominator == 0f)
                return 1f;

            return numerator / denominator;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static void Swap<T>(IList<T> list, int i, int j)
        {
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        //The copy of internal System.Numerics.Hashing.HashHelpers
        public static int Combine(int hc1, int hc2)
        {
            uint rol5 = ((uint)hc1 << 5) | ((uint)hc1 >> 27);
            return ((int)rol5 + hc1) ^ hc2;
        }

        public static int GetHashCode(int hc0, int hc1)
        {
            return hc0 ^ hc1 << 2;
        }

        public static int GetHashCode(int hc0, int hc1, int hc2)
        {
            return hc0 ^ hc1 << 2 ^ hc2 >> 2;
        }

        public static int GetHashCode(int hc0, int hc1, int hc2, int hc3)
        {
            return hc0 ^ hc1 << 2 ^ hc2 >> 2 ^ hc3 >> 1;
        }

        internal static string CutAssemblyQualifiedName(string assemblyQualifiedName)
        {
            const char DEVIDER = ',';

            bool first = false;

            for (int i = 0; i < assemblyQualifiedName.Length; i++)
            {
                if (assemblyQualifiedName[i] != DEVIDER) { continue; }
                if (!first) { first = true; }
                else { return assemblyQualifiedName.Substring(0, i); }
            }

            return null;
        }

        public static object GetDefaultValue(Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean: return default(bool);
                case TypeCode.Byte: return default(byte);
                case TypeCode.Char: return default(char);
                case TypeCode.DateTime: return default(DateTime);
                case TypeCode.Decimal: return default(decimal);
                case TypeCode.Double: return default(double);
                case TypeCode.Int16: return default(short);
                case TypeCode.Int32: return default(int);
                case TypeCode.Int64: return default(long);
                case TypeCode.SByte: return default(sbyte);
                case TypeCode.Single: return default(float);
                case TypeCode.UInt16: return default(ushort);
                case TypeCode.UInt32: return default(uint);
                case TypeCode.UInt64: return default(ulong);

                case TypeCode.Object:
                    if (type.IsValueType)
                        return Activator.CreateInstance(type);
                    return null;

                case TypeCode.Empty:
                case TypeCode.DBNull:
                case TypeCode.String:
                    return null;

                default: throw new UnsupportedValueException(type.GetTypeCode());
            }
        }
    }
}
