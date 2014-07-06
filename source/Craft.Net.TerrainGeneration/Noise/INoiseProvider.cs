using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise
{
	/// <summary>
    /// Interface for noise providers.
    /// </summary>
    /// <remarks>
	/// A noise provider is an object that calculates and outputs a 
    /// scalar value given an input value.
    /// </remarks>
	public interface INoiseProvider
	{
		double Get1D(double x);
        double Get2D(double x, double y);
        double Get3D(double x, double y, double z);
	}
}

