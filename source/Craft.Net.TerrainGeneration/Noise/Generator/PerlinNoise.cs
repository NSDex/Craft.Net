using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
	/// <summary>
	/// Perlin noise generator.
	/// </summary>
    /// <remarks>
    /// Some implementations confuse Perlin Noise with Fractional Brownian Motion
    /// (FBM).  Perlin Noise is actually just one possible input function to a 
    /// FBM function.  A noise provider that implements Fractional Brownian Motion
    /// is provided separately.
    /// 
    /// This Perlin noise generator implementation matches Mojang's implementation
    /// (NoiseGeneratorImproved), which also matches Ken Perlin's reference
    /// implementation except in how the permutations array is initialized (See the
    /// comment in the constructor).  Mojang's implementation includes other 
    /// functionality which I believe is best factored into other INoiseProviders.
    /// 
    /// References:
    /// http://mrl.nyu.edu/~perlin/noise/
    /// http://code.google.com/p/fractalterraingeneration/wiki/Perlin_Noise
    /// 
    /// Values returned by this noise generator will always range from [-1, 1].
    /// </remarks>
	public class PerlinNoise : INoiseProvider
	{
		private int[] permutations;

		public PerlinNoise(Random random)
		{
			permutations = new int[512];

			// Initialize the noise table.
			for (int i = 0; i < 256; i++)
				permutations[i] = i;

			// Shuffle the array and mirror the bottom half into the top half.
			for (int i = 0; i < 256; i++) {
                // As far as I am aware, this method of aquiring a random number to
                // index into the permutations array deviates from the standard
                // implementation: random.Next(256).  I use this implementation
                // because it matches Mojang's.
				int j = random.Next(256 - i) + i;
				int swap = permutations[i];
				permutations[i] = permutations[j];
				permutations[j] = swap;
				permutations[i + 256] = permutations[i];
			}
		}

		public double Get1D(double x)
		{
			throw new Exception("1D Perlin noise is not available.");
		}

		public double Get2D(double x, double z)
		{
			int X = (int)Math.Floor(x) & 255;
			int Z = (int)Math.Floor(z) & 255;

			x -= (double)Math.Floor(x);
			z -= (double)Math.Floor(z);

			double fadeX = fade(x);
			double fadeZ = fade(z);

			int A = this.permutations[X] + 0;
			int AA = this.permutations[A] + Z;
			int B = this.permutations[X + 1] + 0;
			int BB = this.permutations[B] + Z;

			return lerp(fadeZ,
			            lerp(fadeX,
			          		 grad(this.permutations[AA], x, 0.0, z),
			          	     grad(this.permutations[BB], x - 1.0, 0.0, z)),
			            lerp(fadeX,
			          		 grad(this.permutations[AA + 1], x, 0.0, z - 1.0),
			          		 grad(this.permutations[BB + 1], x - 1.0, 0.0, z - 1.0)));
		}

		public double Get3D(double x, double y, double z)
		{
			// Calculate integer coordinates
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            int Z = (int)Math.Floor(z) & 255;

			// Calculate remainder of coordinates
			x -= (double)X;
			y -= (double)Y;
			z -= (double)Z;

			double u = fade(x), v = fade(y), w = fade(z);

            int A  = permutations[X] + Y; 
            int AA = permutations[A] + Z; 
            int AB = permutations[(A + 1)] + Z;
            int B  = permutations[(X + 1)] + Y; 
            int BA = permutations[B] + Z; 
            int BB = permutations[(B + 1)] + Z;

            return lerp(w, lerp(v, lerp(u, grad(permutations[AA], x, y, z),
                                           grad(permutations[BA], x - 1, y, z)),
                                   lerp(u, grad(permutations[AB], x, y - 1, z),
                                           grad(permutations[BB], x - 1, y - 1, z))),
                           lerp(v, lerp(u, grad(permutations[(AA + 1)], x, y, z - 1),
                                           grad(permutations[(BA + 1)], x - 1, y, z - 1)),
                                   lerp(u, grad(permutations[(AB + 1)], x, y - 1, z - 1),
                                           grad(permutations[(BB + 1)], x - 1, y - 1, z - 1))));
		}

		private static double fade(double t) 
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private static double lerp(double t, double a, double b) 
		{
			return a + t * (b - a);
		}

		/// <summary>
		/// Returns a gradient vector for the latice point given by x,y,z, based upon the given hash.
		/// </summary>
		/// <param name="hash">Hash.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		private static double grad(int hash, double x, double y, double z)
		{
			int h = hash & 15;
			double u = h < 8 ? x : y;
			double v = h < 4 ? y : (h != 12 && h != 14 ? z : x);
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}
	}
}

