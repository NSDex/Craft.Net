using System;
using Craft.Net.TerrainGeneration.CoherentNoise.Generator;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Combiner
{
    /// <summary>
    /// The add function adds the result of two other noise providers.
    /// </summary>
    public class Add : INoiseProvider
    {
        INoiseProvider left, right;

        public Add(INoiseProvider left, INoiseProvider right)
        {
            this.left = left;
            this.right = right;
        }
        public Add(INoiseProvider source, double value)
        {
            this.left = source;
            this.right = new ConstNoise(value);
        }

        public double Get1D(double x)
        {
            return this.left.Get1D(x) + this.right.Get1D(x);
        }
        public double Get2D(double x, double y)
        {
            return this.left.Get2D(x, y) + this.right.Get2D(x, y);
        }
        public double Get3D(double x, double y, double z)
        {
            return this.left.Get3D(x, y, z) + this.right.Get3D(x, y, z);
        }
    }
}

