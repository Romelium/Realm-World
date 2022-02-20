using System;
using UnityEngine;
using System.Linq;

namespace Array2DExtensions
{
    public static class Array2DExtensions
    {
        /// <summary>
        /// Warning: Will change the original array2D.
        /// </summary>
        public static T[,] Select<T>(this T[,] array2D, Func<T, T> selector)
        {
            for (int y = 0; y < array2D.GetLength(1); y++)
            {
                for (int x = 0; x < array2D.GetLength(0); x++)
                {
                    array2D[x, y] = selector(array2D[x, y]);
                }
            }
            return array2D;
        }
        public static int GetX(this float[,] array2D, float x_01)
        {
            return (int)(x_01 * (array2D.GetLength(0) - 1));
        }
        public static int GetY(this float[,] array2D, float y_01)
        {
            return (int)(y_01 * (array2D.GetLength(1) - 1));
        }
        public static float GetValue(this float[,] array2D, float x_01, float y_01)
        {
            return array2D[array2D.GetX(x_01), array2D.GetY(y_01)];
        }
        public static float GetSteepness(this float[,] array2D, int x, int y, float scaler = 1f)
        {
            var p1 = array2D[x, y] * scaler;
            var p2 = array2D[x + 1, y] * scaler;
            var p3 = array2D[x, y + 1] * scaler;
            return GetSteepness(p1, p2, p3);
        }
        public static float GetSteepness(this float[,] array2D, float x_01, float y_01, float scaler = 1f)
        {
            return array2D.GetSteepness(array2D.GetX(x_01), array2D.GetY(y_01), scaler);
        }
        // From https://gamedev.stackexchange.com/a/89826
        public static float GetSteepness(float p1, float p2, float p3)
        {
            // simplified https://math.stackexchange.com/q/305914
            var nx = p2 - p1;
            var nz = p3 - p1;
            var ay = 1 / ((nx * nx) + 1 + (nz * nz));
            return ay;
        }
        public static float GetNormal(this float[,] array2D, int x, int y, float scaler = 1f)
        {
            var p1 = array2D[x, y] * scaler;
            var p2 = array2D[x + 1, y] * scaler;
            var p3 = array2D[x, y + 1] * scaler;
            return GetSteepness(p1, p2, p3);
        }
        public static float GetNormal(this float[,] array2D, float x_01, float y_01, float scaler = 1f)
        {
            return array2D.GetNormal(array2D.GetX(x_01), array2D.GetY(y_01), scaler);
        }
        public static Vector3 GetNormal(float p1, float p2, float p3)
        {
            // simplified https://math.stackexchange.com/q/305914
            var vy = p2 - p1;
            var wy = p3 - p1;

            var nx = vy;
            var nz = wy;
            var nxP2 = nx * nx;
            var nzP2 = nz * nz;

            var ax = nx / (nxP2 + 1 + nzP2);
            var ay = 1 / (nxP2 + 1 + nzP2);
            var az = nz / (nxP2 + 1 + nzP2);

            return new Vector3(ax, ay, az);
        }
    }
}