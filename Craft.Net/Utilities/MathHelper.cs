﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Craft.Net.Utilities
{
    public class MathHelper
    {
        /// <summary>
        /// A global <see cref="System.Random"/> instance.
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// Maps a float from 0...360 to 0...255
        /// </summary>
        /// <param name="value"></param>
        public static byte CreateRotationByte(float value)
        {
            return (byte)(((value % 360) / 360) * 256);
        }

        public static int CreateAbsoluteInt(double value)
        {
            return (int)(value * 32);
        }

        public static double Distance2D(double a1, double a2, double b1, double b2)
        {
            return Math.Sqrt(Math.Pow(b1 - a1, 2) + Math.Pow(b2 - a2, 2));
        }

        public static Vector3 RotateX(Vector3 vector, double rotation) // TODO: Matrix
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.X,
                vector.Y * Math.Cos(rotation) - vector.Z * Math.Sin(rotation),
                vector.Y * Math.Sin(rotation) + vector.Z * Math.Cos(rotation));
        }

        public static Vector3 RotateY(Vector3 vector, double rotation)
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.Z * Math.Sin(rotation) + vector.X * Math.Cos(rotation),
                vector.Y,
                vector.Z * Math.Cos(rotation) - vector.X * Math.Sin(rotation));
        }

        public static Vector3 RotateZ(Vector3 vector, double rotation)
        {
            rotation = -rotation; // the algorithms I found were left-handed
            return new Vector3(
                vector.X * Math.Cos(rotation) - vector.Y * Math.Sin(rotation),
                vector.X * Math.Sin(rotation) + vector.Y * Math.Cos(rotation),
                vector.Z);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        /// <summary>
        /// Returns a value indicating the most extreme value of the
        /// provided Vector.
        /// </summary>
        public static unsafe CollisionPoint GetCollisionPoint(Vector3 velocity)
        {
            // TODO: Does this really need to be so unsafe
            int index = 0;
            void* vPtr = &velocity;
            double* ptr = (double*)vPtr;
            double max = 0;
            for (int i = 0; i < 3; i++)
            {
                double value = *(ptr + i);
                if (max < Math.Abs(value))
                {
                    index = i;
                    max = Math.Abs(value);
                }
            }
            switch (index)
            {
                case 0:
                    if (velocity.X < 0)
                        return CollisionPoint.NegativeX;
                    return CollisionPoint.PositiveX;
                case 1:
                    if (velocity.Y < 0)
                        return CollisionPoint.NegativeY;
                    return CollisionPoint.PositiveY;
                default:
                    if (velocity.Z < 0)
                        return CollisionPoint.NegativeZ;
                    return CollisionPoint.PositiveZ;
            }
        }
    }

    public enum Direction
    {
        Bottom = 0,
        Top = 1,
        North = 2,
        South = 3,
        West = 4,
        East = 5
    }

    public enum CollisionPoint
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }
}
