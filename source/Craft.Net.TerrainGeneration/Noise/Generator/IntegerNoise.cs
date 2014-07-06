using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
	/// <summary>
	/// A basic integer noise generator.
	/// </summary>
    /// <remarks>
    /// This is sometimes refered to as White Noise.
    /// 
    /// Values returned by this noise generator will always range from [-1, 1]
    /// </remarks>
    public class IntegerNoise : INoiseProvider
	{
		int seed;

		public IntegerNoise(int seed) 
		{ 
			this.seed = seed;
		}

		// Returns a random number between [-1, 1] based upon the number it is given
		// The big numbers are prime numbers and can be changed to other primes
		public double Get1D(double x) 
		{ 
			int n = ((int)x * IntegerNoiseConst.NOISE_MAGIC_X + IntegerNoiseConst.NOISE_MAGIC_SEED * seed) & 0x7fffffff;
			n = (n << 13) ^ n;
			return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
		}
		public double Get2D(double x, double y) 
		{ 
			int n = ((int)x * IntegerNoiseConst.NOISE_MAGIC_X + (int)y * IntegerNoiseConst.NOISE_MAGIC_Y * IntegerNoiseConst.NOISE_MAGIC_SEED * seed) & 0x7fffffff;
			n = (n << 13) ^ n;
			return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
		}
		public double Get3D(double x, double y, double z) 
		{ 
			int n = ((int)x * IntegerNoiseConst.NOISE_MAGIC_X + (int)y * IntegerNoiseConst.NOISE_MAGIC_Y + (int)z * IntegerNoiseConst.NOISE_MAGIC_Z * IntegerNoiseConst.NOISE_MAGIC_SEED * seed) & 0x7fffffff;
			n = (n << 13) ^ n;
			return (1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
		}


		public static class IntegerNoiseConst
		{
			public static int NOISE_MAGIC_X = 1619;
			public static int NOISE_MAGIC_Y = 31337;
			public static int NOISE_MAGIC_Z = 52591;
			public static int NOISE_MAGIC_SEED = 1013;
		}
	}
}

