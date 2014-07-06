using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Modifier
{
    /// <summary>
    /// The Abs function returns the absolute value of the result from a noise provider.
    /// </summary>
	public class Abs : INoiseProvider
	{
		INoiseProvider source;

        public Abs(INoiseProvider source)
		{
			this.source = source;
		}

		public double Get1D(double x) 
		{ 
			double value = source.Get1D(x);
			return Math.Abs(value);
		}
		public double Get2D(double x, double y) 
		{ 
			double value = source.Get2D(x, y);
			return Math.Abs(value);
		}
		public double Get3D(double x, double y, double z)
		{ 
			double value = source.Get3D(x, y, z);
			return Math.Abs(value);
		}
	}
}

