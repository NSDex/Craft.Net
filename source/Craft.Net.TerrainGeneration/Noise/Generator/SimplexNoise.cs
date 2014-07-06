using System;
using Craft.Net.Common;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Generator
{
    /// <summary>
    /// A simplex noise generator.
    /// </summary>
    /// <remarks>
    /// This simplex noise generator matches Mojang's implementation
    /// (NoiseGeneratorSimplex).
    /// 
    /// References:
    /// http://code.google.com/p/fractalterraingeneration/wiki/Simplex_Noise
    /// 
    /// This implementation is from the Terasology source:
    /// https://github.com/MovingBlocks/Terasology/blob/develop/engine/src/main/java/org/terasology/utilities/procedural/SimplexNoise.java
    /// 
    /// Values returned by this noise generator will always range from [-1, 1].
    /// </remarks>
    public class SimplexNoise : INoiseProvider
    {
        private static Vector3[] grad3 = {
            new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0), 
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), 
            new Vector3(0, 1, 1), new Vector3(0, -1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, -1)
        };

        // Skewing and unskewing factors 
        private static double F2 = 0.5 * (Math.Sqrt(3.0) - 1.0);
        private static double G2 = (3.0 - Math.Sqrt(3.0)) / 6.0;
        private static double F3 = 1.0 / 3.0;
        private static double G3 = 1.0 / 6.0;

        private int[] permutations;
        private int[] permutationsMod12;

        public SimplexNoise(Random random)
        {
            permutations = new int[512];
            permutationsMod12 = new int[512];

            // Initialize the noise table.
            for (int i = 0; i < 256; i++)
                permutations[i] = i;

            // Shuffle the array and mirror the bottom half into the top half.
            for (int i = 0; i < 256; ++i)
            {
                int j = random.Next(256 - i) + i;
                int k = this.permutations[i];
                this.permutations[i] = this.permutations[j];
                this.permutations[j] = k;

                this.permutations[i + 256] = this.permutations[i];

                this.permutationsMod12[i] = this.permutations[i] % 12;
                this.permutationsMod12[i + 256] = this.permutationsMod12[i];
            }
        }

        public double Get1D(double x)
        {
            throw new Exception("1D Simplex noise is not available.");
        }

        public double Get2D(double x, double y)
        {
            double n0;
            double n1;
            double n2; // Noise contributions from the three corners

            // Skew the input space to determine which simplex cell we're in
            double s = (x + y) * F2; // Hairy factor for 2D
            int i = fastfloor(x + s);
            int j = fastfloor(y + s);
            double t = (i + j) * G2;
            double xo0 = i - t; // Unskew the cell origin back to (x,y) space
            double yo0 = j - t;
            double x0 = x - xo0; // The x,y distances from the cell origin
            double y0 = y - yo0;

            // For the 2D case, the simplex shape is an equilateral triangle.
            // Determine which simplex we are in.
            int i1; // Offsets for second (middle) corner of simplex in (i,j) coords
            int j1;
            if (x0 > y0) { // lower triangle, XY order: (0,0)->(1,0)->(1,1)
                i1 = 1;
                j1 = 0;
            } else { // upper triangle, YX order: (0,0)->(0,1)->(1,1)
                i1 = 0;
                j1 = 1;
            }

            // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
            // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
            // c = (3-sqrt(3))/6
            double x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
            double y1 = y0 - j1 + G2;
            double x2 = x0 - 1.0 + 2.0 * G2; // Offsets for last corner in (x,y) unskewed coords
            double y2 = y0 - 1.0 + 2.0 * G2;

            // Work out the hashed gradient indices of the three simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int gi0 = permutationsMod12[ii + permutations[jj]];
            int gi1 = permutationsMod12[ii + i1 + permutations[jj + j1]];
            int gi2 = permutationsMod12[ii + 1 + permutations[jj + 1]];

            // Calculate the contribution from the three corners
            double t0 = 0.5 - x0 * x0 - y0 * y0;
            if (t0 < 0) {
                n0 = 0.0;
            } else {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient
            }
            double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 < 0) {
                n1 = 0.0;
            } else {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad3[gi1], x1, y1);
            }
            double t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 < 0) {
                n2 = 0.0;
            } else {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad3[gi2], x2, y2);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to return values in the interval [-1,1].
            return 70.0 * (n0 + n1 + n2);
        }

        public double Get3D(double x, double y, double z)
        {
            double n0;
            double n1;
            double n2;
            double n3; // Noise contributions from the four corners

            // Skew the input space to determine which simplex cell we're in
            double s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
            int i = fastfloor(x + s);
            int j = fastfloor(y + s);
            int k = fastfloor(z + s);
            double t = (i + j + k) * G3;
            double xo0 = i - t; // Unskew the cell origin back to (x,y,z) space
            double yo0 = j - t;
            double zo0 = k - t;
            double x0 = x - xo0; // The x,y,z distances from the cell origin
            double y0 = y - yo0;
            double z0 = z - zo0;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1;
            int j1;
            int k1; // Offsets for second corner of simplex in (i,j,k) coords

            int i2;
            int j2;
            int k2; // Offsets for third corner of simplex in (i,j,k) coords

            if (x0 >= y0) {
                if (y0 >= z0) {         // X Y Z order
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                } else if (x0 >= z0) {  // X Z Y order
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                } else {                // Z X Y order
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            } else { // x0<y0
                if (y0 < z0) {          // Z Y X order
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } else if (x0 < z0) {   // Y Z X order
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                } else {                // Y X Z order
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }
            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
            // c = 1/6.
            double x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;
            double x2 = x0 - i2 + 2.0 * G3; // Offsets for third corner in (x,y,z) coords
            double y2 = y0 - j2 + 2.0 * G3;
            double z2 = z0 - k2 + 2.0 * G3;
            double x3 = x0 - 1.0 + 3.0 * G3; // Offsets for last corner in (x,y,z) coords
            double y3 = y0 - 1.0 + 3.0 * G3;
            double z3 = z0 - 1.0 + 3.0 * G3;

            // Work out the hashed gradient indices of the four simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int gi0 = permutationsMod12[ii + permutations[jj + permutations[kk]]];
            int gi1 = permutationsMod12[ii + i1 + permutations[jj + j1 + permutations[kk + k1]]];
            int gi2 = permutationsMod12[ii + i2 + permutations[jj + j2 + permutations[kk + k2]]];
            int gi3 = permutationsMod12[ii + 1 + permutations[jj + 1 + permutations[kk + 1]]];

            // Calculate the contribution from the four corners
            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0) {
                n0 = 0.0;
            } else {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
            }
            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0) {
                n1 = 0.0;
            } else {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
            }
            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) {
                n2 = 0.0;
            } else {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
            }
            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) {
                n3 = 0.0;
            } else {
                t3 *= t3;
                n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            // The result is scaled to stay just inside [-1,1]
            return 32.0 * (n0 + n1 + n2 + n3);
        }

        private static int fastfloor(double x)
        {
            return x > 0.0D ? (int)x : (int)x - 1;
        }

        private static double dot(Vector3 g, double x, double y) {
            return g.X * x + g.Y * y;
        }

        private static double dot(Vector3 g, double x, double y, double z) {
            return g.X * x + g.Y * y + g.Z * z;
        }
    }
}

