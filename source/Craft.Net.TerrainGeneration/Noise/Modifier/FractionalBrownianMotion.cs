using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Modifier
{
	/// <summary>
	/// Applies Fractional Brownian Motion to a given noise provider.
	/// </summary>
    public class FractionalBrownianMotion : INoiseProvider
	{
		INoiseProvider source;
		double frequency;
		double amplitude;
		double lacunarity;
		double persistence; // aka Gain
		int octaves;

        public FractionalBrownianMotion(INoiseProvider source, int octaves, double frequency, double amplitude, double lacunarity = 2.0, double persistence = 0.5)
		{
			this.source = source;
			this.octaves = octaves;
			this.frequency = frequency;
			this.amplitude = amplitude;
			this.lacunarity = lacunarity;
			this.persistence = persistence;
		}

		public double Get1D(double x)
		{
			double total = 0;
			double tempAmplitude = amplitude;
			double tempFrequency = frequency;

			for (int i = 0; i < octaves; i++)
			{
				total += source.Get1D(x * tempFrequency) * tempAmplitude;
				tempFrequency *= lacunarity;
				tempAmplitude *= persistence;
			}
			return total;
		}

		public double Get2D(double x, double y)
		{
			double total = 0;
			double tempAmplitude = amplitude;
			double tempFrequency = frequency;

			for (int i = 0; i < octaves; i++)
			{
				total += source.Get2D(x * tempFrequency, y * tempFrequency) * tempAmplitude;
				tempFrequency *= lacunarity;
				tempAmplitude *= persistence;
			}
			return total;
		}

		public double Get3D(double x, double y, double z)
		{
			double total = 0;
			double tempAmplitude = amplitude;
			double tempFrequency = frequency;

			for (int i = 0; i < octaves; i++)
			{
				total += source.Get3D(x * tempFrequency, y * tempFrequency, z * tempFrequency) * tempAmplitude;
				tempFrequency *= lacunarity;
				tempAmplitude *= persistence;
			}
			return total;
		}
	}
}

