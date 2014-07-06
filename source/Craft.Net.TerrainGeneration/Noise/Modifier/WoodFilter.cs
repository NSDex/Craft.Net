using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Modifier
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// This implementation is from http://www.scratchapixel.com/lessons/3d-advanced-lessons/noise-part-1/pattern-examples/
    /// </remarks>
	public class WoodFilter : INoiseProvider
	{
		public double multiplier { get; set; }
		INoiseProvider source;

		public WoodFilter(INoiseProvider source, double multiplier = 4.0)
		{
			this.source = source;
			this.multiplier = multiplier;
		}

		public double Get1D(double x) 
		{ 
			double value = source.Get1D(x) * multiplier;
			return value - (int)value;
		}
		public double Get2D(double x, double y) 
		{ 
			double value = source.Get2D(x, y) * multiplier;
			return value - (int)value;
		}
		public double Get3D(double x, double y, double z)
		{ 
			double value = source.Get3D(x, y, z) * multiplier;
			return value - (int)value;
		}
	}
}

