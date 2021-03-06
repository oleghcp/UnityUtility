using System.Runtime.CompilerServices;
using UnityEngine;
using UnityUtility.MathExt;
using static System.MathF;

namespace UnityUtility
{
    public static class MathUtility
    {
        /// <summary>
        /// Rotates an array cell position.
        /// </summary>
        /// <param name="i">Row.</param>
        /// <param name="j">Column.</param>
        /// <param name="rotations">Defines a rotation angle by multiplying by 90 degrees. If the value is positive returns rotated clockwise.</param>
        public static (int i, int j) RotateCellPos(int i, int j, int rotations)
        {
            //Span<int> sinPtr = stackalloc[] { 0, 1, 0, -1 };
            //Span<int> cosPtr = stackalloc[] { 1, 0, -1, 0 };

            //rotations = rotations.Repeat(4);

            //int sin = sinPtr[rotations];
            //int cos = cosPtr[rotations];

            //int i1 = j * sin + i * cos;
            //int j1 = j * cos - i * sin;

            //return (i1, j1);

            switch (rotations.Repeat(4))
            {
                case 1: return (j, -i);
                case 2: return (-i, -j);
                case 3: return (-j, i);
                default: return (i, j);
            }
        }

        /// <summary>
        /// Rotates vector2.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector2 RotateVector(in Vector2 rotated, float angle)
        {
            angle = angle.ToRadians();
            float sin = Sin(angle);
            float cos = Cos(angle);

            return new Vector2(rotated.x * cos - rotated.y * sin, rotated.x * sin + rotated.y * cos);
        }

        /// <summary>
        /// Rotates vector3 around specified axis.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees.</param>
        public static Vector3 RotateVector(in Vector3 rotated, in Vector3 axis, float angle)
        {
            angle = angle.ToRadians();

            float sin = Sin(angle);
            float cos = Cos(angle);

            float oneMinusCos = 1f - cos;
            float oneMinusCosByXY = oneMinusCos * axis.x * axis.y;
            float oneMinusCosByYZ = oneMinusCos * axis.y * axis.z;
            float oneMinusCosByZX = oneMinusCos * axis.z * axis.x;
            float xSin = sin * axis.x;
            float ySin = sin * axis.y;
            float zSin = sin * axis.z;

            return new Vector3
            {
                x = rotated.x * (cos + oneMinusCos * axis.x * axis.x) + rotated.y * (oneMinusCosByXY - zSin) + rotated.z * (oneMinusCosByZX + ySin),
                y = rotated.x * (oneMinusCosByXY + zSin) + rotated.y * (cos + oneMinusCos * axis.y * axis.y) + rotated.z * (oneMinusCosByYZ - xSin),
                z = rotated.x * (oneMinusCosByZX - ySin) + rotated.y * (oneMinusCosByYZ + xSin) + rotated.z * (cos + oneMinusCos * axis.z * axis.z),
            };
        }

        /// <summary>
        /// Returns vector2 corresponding to the specified angle. Default is Vector2.Right.
        /// </summary>
        /// <param name="angle">Angle in degrees.</param>
        public static Vector2 AngleToVector2(float angle)
        {
            angle = angle.ToRadians();
            return new Vector2(Cos(angle), Sin(angle));
        }

        public static (float ch1, float ch2) LerpColorChannels(float ratio)
        {
            ratio = 1f - ratio.Clamp01();
            float ch1 = (ratio * 2f).Clamp01();
            float ch2 = (2f - ratio * 2f).Clamp01();
            return (ch1, ch2);
        }

        /// <summary>
        /// 1f - MathF.Exp(-value)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InvExp(float value)
        {
            return 1f - Exp(-value);
        }
    }
}
