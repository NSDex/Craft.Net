using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Modifier
{
	public class Clamp : INoiseProvider
	{
		public double lowerBound { get; set; }
		public double upperBound { get; set; }
		INoiseProvider source;

        public Clamp(INoiseProvider source, double lowerBound = 0.0, double upperBound = 1.0)
		{
			this.source = source;
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		public double Get1D(double x) 
		{ 
			double value = source.Get1D(x);
			if (value < lowerBound) {
				return lowerBound;
			} else if (value > upperBound) {
				return upperBound;
			} else {
				return value;
			}
		}
		public double Get2D(double x, double y) 
		{ 
			double value = source.Get2D(x, y);
			if (value < lowerBound) {
				return lowerBound;
			} else if (value > upperBound) {
				return upperBound;
			} else {
				return value;
			}
		}
		public double Get3D(double x, double y, double z)
		{ 
			double value = source.Get3D(x, y, z);
			if (value < lowerBound) {
				return lowerBound;
			} else if (value > upperBound) {
				return upperBound;
			} else {
				return value;
			}
		}
	}
}

