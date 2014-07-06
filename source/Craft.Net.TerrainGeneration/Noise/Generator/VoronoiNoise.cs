using System;
using System.Collections;
using Craft.Net.Common;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
	/// <summary>
	/// Noise provider that implements Voronoi Noise.
	/// </summary>
	/// <remarks>
	/// In mathematics, a Voronoi cell is a region containing all the
	/// points that are closer to a specific seed point than to any
	/// other seed point.  These cells mesh with one another, producing
	/// polygon-like formations.
    /// 
    /// This implementation is from libNoise <http://aftbit.com/cell-noise-2/>
    /// with modifications by NSDex.
	/// </remarks>
	public class VoronoiNoise : INoiseProvider
	{
        public static double EuclidianDistanceFunc(Vector3 p1, Vector3 p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z);
        }
        public static double ManhattanDistanceFunc(Vector3 p1, Vector3 p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
        }
        public static double ChebyshevDistanceFunc(Vector3 p1, Vector3 p2)
        {
            Vector3 diff = p1 - p2;
            return Math.Max(Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)), Math.Abs(diff.Z));
        }

        /// <summary>
        /// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
        /// it will not be added to the array.
        /// </summary>
        /// <param name="arr">The array to insert the value into.</param>
        /// <param name="value">The value to insert into the array.</param>
        public static void InsertShortest(ArrayList arr, double value)
        {
            double temp;
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (value > (double)arr[i]) break;
                temp = (double)arr[i];
                arr[i] = value;
                if (i + 1 < arr.Count) arr[i + 1] = temp;
            }
        }

        public static double ChooseShortest(ArrayList arr)
        {
            return (double)arr[0];
        }


        /// <summary>
        /// </summary>
        public delegate double DistanceFunction(Vector3 p1,Vector3 p2);
        /// <summary>
        /// </summary>
        public delegate double InsertionFunction(ArrayList points, double value);
        /// <summary>
        /// </summary>
        public delegate double ReductionFunction(ArrayList points);

        protected DistanceFunction distanceFunction;
        protected InsertionFunction insertFunction;
        protected ReductionFunction reductionFunction;
        protected int seed;

        public VoronoiNoise(int seed, DistanceFunction distanceFunc, InsertionFunction insertFunc, ReductionFunction reductionFunc)
		{
            this.seed = seed;
            this.distanceFunction = distanceFunc;
            this.insertFunction = insertFunc;
            this.reductionFunction = reductionFunc;
		}

		public double Get1D(double x)
		{
            ArrayList distanceArray = new ArrayList(2);
            Vector3 input = new Vector3(x, 0, 0);

            // Determine which cube the evaluation point is in
            int evalCubeX = (int)Math.Floor(x);

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int i = -1; i < 2; i++) {

                int cubeX = evalCubeX + i; 

                // Generate a reproducible random number generator for the cube
                uint lastRandom = lcgRandom(hash((uint)(cubeX + this.seed), 0, 0));
                // Determine how many feature points are in the cube
                uint numberFeaturePoints = probLookup(lastRandom);

                // Randomly place the feature points in the cube
                for (uint l = 0; l < numberFeaturePoints; l++) {
                    Vector3 randomDiff, featurePoint;

                    lastRandom = lcgRandom(lastRandom);
                    randomDiff.X = (float)lastRandom / 0x100000000;

                    featurePoint = new Vector3(randomDiff.X + (float)cubeX, 0, 0);

                    // Find the feature point closest to the evaluation point. 
                    // This is done by inserting the distances to the feature points into a sorted list
                    this.insertFunction(distanceArray, this.distanceFunction(input, featurePoint));
                }

                // Check the neighboring cubes to ensure their are no closer evaluation points.
                // This is done by repeating steps 1 through 5 above for each neighboring cube
            }

            return this.reductionFunction(distanceArray);
		}

		public double Get2D(double x, double z)
		{
            ArrayList distanceArray = new ArrayList(2);
            Vector3 input = new Vector3(x, 0, z);

            // Determine which cube the evaluation point is in
            int evalCubeX = (int)Math.Floor(x);
            int evalCubeZ = (int)Math.Floor(z);

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int i = -1; i < 2; i++) {
                for (int k = -1; k < 2; k++) {

                    int cubeX = evalCubeX + i; 
                    int cubeZ = evalCubeZ + k;

                    // Generate a reproducible random number generator for the cube
                    uint lastRandom = lcgRandom(hash((uint)(cubeX + this.seed), 0, (uint)(cubeZ)));
                    // Determine how many feature points are in the cube
                    uint numberFeaturePoints = probLookup(lastRandom);

                    // Randomly place the feature points in the cube
                    for (uint l = 0; l < numberFeaturePoints; l++) {
                        Vector3 randomDiff, featurePoint;

                        lastRandom = lcgRandom(lastRandom);
                        randomDiff.X = (float)lastRandom / 0x100000000;

                        lastRandom = lcgRandom(lastRandom);
                        randomDiff.Z = (float)lastRandom / 0x100000000;

                        featurePoint = new Vector3(randomDiff.X + (float)cubeX, 0, randomDiff.Z + (float)cubeZ);

                        // Find the feature point closest to the evaluation point. 
                        // This is done by inserting the distances to the feature points into a sorted list
                        this.insertFunction(distanceArray, this.distanceFunction(input, featurePoint));
                    }

                    // Check the neighboring cubes to ensure their are no closer evaluation points.
                    // This is done by repeating steps 1 through 5 above for each neighboring cube
                }
            }

            return this.reductionFunction(distanceArray);
		}

		public double Get3D(double x, double y, double z)
		{
            ArrayList distanceArray = new ArrayList(2);
            Vector3 input = new Vector3(x, y, z);

            // Determine which cube the evaluation point is in
            int evalCubeX = (int)Math.Floor(x);
            int evalCubeY = (int)Math.Floor(y);
            int evalCubeZ = (int)Math.Floor(z);

			// Inside each unit cube, there is a seed point at a random position.  Go
			// through each of the nearby cubes until we find a cube with a seed point
			// that is closest to the specified position.
			for (int i = -1; i < 2; i++) {
				for (int j = -1; j < 2; j++) {
					for (int k = -1; k < 2; k++) {

                        int cubeX = evalCubeX + i; 
                        int cubeY = evalCubeY + j;
                        int cubeZ = evalCubeZ + k;

                        // Generate a reproducible random number generator for the cube
                        uint lastRandom = lcgRandom(hash((uint)(cubeX + this.seed), (uint)(cubeY), (uint)(cubeZ)));
                        // Determine how many feature points are in the cube
                        uint numberFeaturePoints = probLookup(lastRandom);

                        // Randomly place the feature points in the cube
                        for (uint l = 0; l < numberFeaturePoints; l++) {
                            Vector3 randomDiff, featurePoint;

                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.X = (float)lastRandom / 0x100000000;

                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.Y = (float)lastRandom / 0x100000000;

                            lastRandom = lcgRandom(lastRandom);
                            randomDiff.Z = (float)lastRandom / 0x100000000;

                            featurePoint = new Vector3(randomDiff.X + (float)cubeX, randomDiff.Y + (float)cubeY, randomDiff.Z + (float)cubeZ);

                            // Find the feature point closest to the evaluation point. 
                            // This is done by inserting the distances to the feature points into a sorted list
                            this.insertFunction(distanceArray, this.distanceFunction(input, featurePoint));
                        }

                        // Check the neighboring cubes to ensure their are no closer evaluation points.
                        // This is done by repeating steps 1 through 5 above for each neighboring cube
					}
				}
			}

            return this.reductionFunction(distanceArray);
		}

        /// <summary>
        /// Given a uniformly distributed random number this function returns the number of feature points in a given cube.
        /// </summary>
        /// <param name="value">a uniformly distributed random number</param>
        /// <returns>The number of feature points in a cube.</returns>
        //  Generated using mathmatica with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]"
        private static uint probLookup(uint value)
        {
            if (value < 393325350) return 1;
            if (value < 1022645910) return 2;
            if (value < 1861739990) return 3;
            if (value < 2700834071) return 4;
            if (value < 3372109335) return 5;
            if (value < 3819626178) return 6;
            if (value < 4075350088) return 7;
            if (value < 4203212043) return 8;
            return 9;
        }

        /// <summary>
        /// LCG Random Number Generator.
        /// LCG: http://en.wikipedia.org/wiki/Linear_congruential_generator
        /// </summary>
        /// <param name="lastValue">The last value calculated by the lcg or a seed</param>
        /// <returns>A new random number</returns>
        private static uint lcgRandom(uint lastValue)
        {
            return (uint)((1103515245u * lastValue + 12345u) % 0x100000000u);
        }

        /// <summary>
        /// Constant used in FNV hash function.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint OFFSET_BASIS = 2166136261;
        /// <summary>
        /// Constant used in FNV hash function
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint FNV_PRIME = 16777619;
        /// <summary>
        /// Hashes three integers into a single integer using FNV hash.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        /// <returns>hash value</returns>
        private static uint hash(uint i, uint j, uint k)
        {
            return (uint)((((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME) ^ (uint)k) * FNV_PRIME);
        }
	}
}

