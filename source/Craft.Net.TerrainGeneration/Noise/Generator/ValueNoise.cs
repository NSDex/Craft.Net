using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
	/// <summary>
	/// Value noise.
	/// </summary>
    public class ValueNoise : INoiseProvider
	{
		INoiseProvider source;
		IInterpolator valueInterpolator;

		public ValueNoise(INoiseProvider source, IInterpolator valueInterpolator)
		{
			this.source = source;
			this.valueInterpolator = valueInterpolator;
		}

		public double Get1D(double x)
		{
			// Calculate integer coordinates
			int integerX = (x > 0.0 ? (int)x : (int)x - 1);
			// Calculate remainder of coordinates
			double fractionalX = x - (double)integerX;

			// Get values for the sides
			double v1 = source.Get1D(integerX);
			double v2 = source.Get1D(integerX + 1);

			return valueInterpolator.Interpolate(v1, v2, fractionalX);
		}

		public double Get2D(double x, double y)
		{
			// Calculate integer coordinates
			int integerX = (x > 0.0 ? (int)x : (int)x - 1);
			int integerY = (y > 0.0 ? (int)y : (int)y - 1);
			// Calculate remainder of coordinates
			double fractionalX = x - (double)integerX;
			double fractionalY = y - (double)integerY;

			// Get values for corners
			double v1 = source.Get2D(integerX, integerY);
			double v2 = source.Get2D(integerX + 1, integerY);
			double v3 = source.Get2D(integerX, integerY + 1);
			double v4 = source.Get2D(integerX + 1, integerY + 1);
			// Interpolate X
			double i1 = valueInterpolator.Interpolate(v1, v2, fractionalX);
			double i2 = valueInterpolator.Interpolate(v3, v4, fractionalX);
			// Interpolate Y
			return valueInterpolator.Interpolate(i1, i2, fractionalY);
		}

		public double Get3D(double x, double y, double z)
		{
			// Calculate integer coordinates
			int integerX = (x > 0.0 ? (int)x : (int)x - 1);
			int integerY = (y > 0.0 ? (int)y : (int)y - 1);
			int integerZ = (z > 0.0 ? (int)z : (int)z - 1);
			// Calculate remainder of coordinates
			double fractionalX = x - (double)integerX;
			double fractionalY = y - (double)integerY;
			double fractionalZ = z - (double)integerZ;

			// Get values for corners
			double v1 = source.Get3D(integerX, integerY, integerZ);
			double v2 = source.Get3D(integerX + 1, integerY, integerZ);
			double v3 = source.Get3D(integerX, integerY + 1, integerZ);
			double v4 = source.Get3D(integerX + 1, integerY + 1, integerZ);
			double v5 = source.Get3D(integerX, integerY, integerZ + 1);
			double v6 = source.Get3D(integerX + 1, integerY, integerZ + 1);
			double v7 = source.Get3D(integerX, integerY + 1, integerZ + 1);
			double v8 = source.Get3D(integerX + 1, integerY + 1, integerZ + 1);
			// Interpolate X
			double ii1 = valueInterpolator.Interpolate(v1, v2, fractionalX);
			double ii2 = valueInterpolator.Interpolate(v3, v4, fractionalX);
			double ii3 = valueInterpolator.Interpolate(v5, v6, fractionalX);
			double ii4 = valueInterpolator.Interpolate(v7, v8, fractionalX);
			// Interpolate Y
			double i1 = valueInterpolator.Interpolate(ii1, ii2, fractionalY);
			double i2 = valueInterpolator.Interpolate(ii3, ii4, fractionalY);
			// Interpolate Z
			return valueInterpolator.Interpolate(i1, i2, fractionalZ);
		}
	}
}

