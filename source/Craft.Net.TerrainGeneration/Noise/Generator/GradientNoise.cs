using System;
using Craft.Net.Common;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
    /// <summary>
    /// Gradient noise returns the distance of the input point along a line
    /// extending from the low point to the high point.
    /// </summary>
    /// <remarks>
    /// This implementation is from the Accidental Noise Library
    /// <https://code.google.com/p/accidental-noise-library/>.
    /// 
    /// Values returned by this noise generator will always range from [-1, 1].
    /// </remarks>
    public class GradientNoise : INoiseProvider
    {
        protected Vector3 lowPoint;
        protected Vector3 highPoint;
        protected Vector3 delta;
        protected double lengthSquared;
        protected IInterpolator interpolator;

        public GradientNoise(Vector3 lowPoint, Vector3 highPoint, IInterpolator interpolator = null)
        {
            this.lowPoint = lowPoint;
            this.highPoint = highPoint;
            this.delta = highPoint - lowPoint;

            this.lengthSquared = (this.delta.X * this.delta.X + this.delta.Y * this.delta.Y + this.delta.Z * this.delta.Z);

            this.interpolator = interpolator != null ? interpolator : new LinearInterpolator();
        }

        public double Get1D(double x)
        {
            double dx = x - lowPoint.X;
            double dp = clamp(dx / this.lengthSquared, 0, 1);
            return this.interpolator.Interpolate(-1, 1, dp);
        }

        public double Get2D(double x, double y)
        {
            double dx = x - lowPoint.X;
            double dy = y - lowPoint.Y;
            double dp = dx * this.delta.X + dy * this.delta.Y;
            dp = clamp(dp / this.lengthSquared, 0, 1);
            return this.interpolator.Interpolate(-1, 1, dp);
        }

        public double Get3D(double x, double y, double z)
        {
            double dx = x - lowPoint.X;
            double dy = y - lowPoint.Y;
            double dz = z - lowPoint.Z;
            double dp = dx * this.delta.X + dy * this.delta.Y + dz * this.delta.Z;
            dp = clamp(dp / this.lengthSquared, 0, 1);
            return this.interpolator.Interpolate(-1, 1, dp);
        }

        protected double clamp(double p, double a, double b)
        {
            return Math.Max(Math.Min(p, b), a);
        }
    }
}

