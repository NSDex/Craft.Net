using System;
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
    /// This implementation is from libNoise <http://libnoise.sourceforge.net/>
    /// with modifications by NSDex.
    /// </remarks>
    public class VoronoiNoise : INoiseProvider
    {
        /// <summary>
        /// </summary>
        public delegate double DistanceFunction(Vector3 p1,Vector3 p2);

        /// <summary>
        /// </summary>
        public delegate double ReductionFunction(Vector3 inputPoint, Vector3 cellPoint, double distance);

        protected DistanceFunction distanceFunction;
        protected ReductionFunction reductionFunction;
        protected INoiseProvider[] seedNoiseProvider;

        public VoronoiNoise(INoiseProvider[] seedNoiseProvider, DistanceFunction distanceFunc, ReductionFunction reductionFunc)
        {
            this.distanceFunction = distanceFunc;
            this.reductionFunction = reductionFunc;
            this.seedNoiseProvider = seedNoiseProvider;
        }

        public double Get1D(double x)
        {
            int xInt = (x > 0.0 ? (int)x: (int)x - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++) {
                // Calculate the position and distance to the seed point inside of
                // this unit cube.
                double xPos = xCur + this.seedNoiseProvider[0].Get1D(xCur);
                double xDist = xPos - x;
                double dist = xDist * xDist;

                if (dist < minDist) {
                    // This seed point is closer to any others found so far, so record
                    // this seed point.
                    minDist = dist;
                    xCandidate = xPos;
                }
            }

            return this.reductionFunction(new Vector3(x, 0, 0), new Vector3(xCandidate, 0, 0), minDist);
        }

        public double Get2D(double x, double y)
        {
            int xInt = (x > 0.0 ? (int)x: (int)x - 1);
            int yInt = (y > 0.0 ? (int)y: (int)y - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++) {
                for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++) {

                    // Calculate the position and distance to the seed point inside of
                    // this unit cube.
                    double xPos = xCur + this.seedNoiseProvider[0].Get2D(xCur, yCur);
                    double yPos = yCur + this.seedNoiseProvider[1].Get2D(xCur, yCur);
                    double xDist = xPos - x;
                    double yDist = yPos - y;
                    double dist = xDist * xDist + yDist * yDist;

                    if (dist < minDist) {
                        // This seed point is closer to any others found so far, so record
                        // this seed point.
                        minDist = dist;
                        xCandidate = xPos;
                        yCandidate = yPos;
                    }
                }
            }

            return this.reductionFunction(new Vector3(x, y, 0), new Vector3(xCandidate, yCandidate, 0), minDist);
        }

        public double Get3D(double x, double y, double z)
        {
            int xInt = (x > 0.0 ? (int)x: (int)x - 1);
            int yInt = (y > 0.0 ? (int)y: (int)y - 1);
            int zInt = (z > 0.0 ? (int)z: (int)z - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++) {
                for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++) {
                    for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++) {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        double xPos = xCur + this.seedNoiseProvider[0].Get3D(xCur, yCur, zCur);
                        double yPos = yCur + this.seedNoiseProvider[1].Get3D(xCur, yCur, zCur);
                        double zPos = zCur + this.seedNoiseProvider[2].Get3D(xCur, yCur, zCur);
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist) {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            return this.reductionFunction(new Vector3(x, y, z), new Vector3(xCandidate, yCandidate, zCandidate), minDist);
        }
    }
}

