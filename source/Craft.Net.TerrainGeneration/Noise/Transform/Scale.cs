using System;
using Craft.Net.Common;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Transform
{
    /// <summary>
    /// The scale function multiplies the input of a noise provider by a vector.
    /// </summary>
    public class Scale : INoiseProvider
    {
        protected INoiseProvider source;
        protected Vector3 scaleFactor;

        public Scale(INoiseProvider source, Vector3 scaleFactor)
        {
            this.source = source;
            this.scaleFactor = scaleFactor;
        }

        public double Get1D(double x)
        {
            return this.source.Get1D(x * this.scaleFactor.X);
        }

        public double Get2D(double x, double y)
        {
            return this.source.Get2D(x * this.scaleFactor.X, y * this.scaleFactor.Y);
        }

        public double Get3D(double x, double y, double z)
        {
            return this.source.Get3D(x * this.scaleFactor.X, y * this.scaleFactor.Y, z * this.scaleFactor.Z);
        }
    }
}

