using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
	/// <summary>
	/// Noise generator that outputs a constant value.
	/// </summary>
    public class ConstNoise : INoiseProvider
	{
		public double constValue { get; set; }

		public ConstNoise(double value = 0) 
		{ 
			this.constValue = value;
		}

		public double Get1D(double x)
		{
			return constValue;
		}
		public double Get2D(double x, double y)
		{
			return constValue;
		}
		public double Get3D(double x, double y, double z)
		{
			return constValue;
		}
	}
}

