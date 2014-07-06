using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise
{
	public interface IInterpolator
	{
		// Interpolate  between two points, a and b, based upon the value p (which is between 0 and 1)
		// When p is 0, a is returned
		// When p is 1, b is returned
		double Interpolate(double a, double b, double p);
	}


	/// <summary>
	/// Linear interpolation is the fastest but has most jagged output.
	/// </summary>
	public class LinearInterpolator : IInterpolator
	{
		public double Interpolate(double a, double b, double p)
		{
            return a + p * (b - a);
		}
	}

	
	public class CosineInterpolator : IInterpolator
	{
		public double Interpolate(double a, double b, double p)
		{
			double ft = p * Math.PI;
			double f = (1 - Math.Cos(ft)) * 0.5;
			// Interpolate linear
			return a * (1 - f) + b * f;
		}
	}


	public class SmoothstepInterpolator : IInterpolator
	{
		public double Interpolate(double a, double b, double p)
		{
			double f = p * p * ( 3 - 2 * p );
			// Interpolate linear
			return a * (1 - f) + b * f;
		}
	}
}

