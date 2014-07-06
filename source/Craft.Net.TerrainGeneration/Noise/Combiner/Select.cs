using System;
using Craft.Net.TerrainGeneration.CoherentNoise.Generator;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Combiner
{
    /// <summary>
    /// The select function chooses between two values based on the value of a
    /// control source.
    /// </summary>
    /// <remarks>
    /// This implementation is from the Accidental Noise Library
    /// <https://code.google.com/p/accidental-noise-library/>.
    /// </remarks>
    public class Select : INoiseProvider
    {
        protected INoiseProvider lowSource;
        protected INoiseProvider highSource;
        protected INoiseProvider controlSource;
        protected INoiseProvider thresholdSource;
        protected INoiseProvider falloffSource;
        protected IInterpolator falloffInterpolator;

        public Select(INoiseProvider controlSource, INoiseProvider lowSource, INoiseProvider highSource, INoiseProvider thresholdSource, INoiseProvider falloffSource, IInterpolator falloffInterpolator)
        {
            this.controlSource = controlSource;
            this.lowSource = lowSource;
            this.highSource = highSource;
            this.thresholdSource = thresholdSource;
            this.falloffSource = falloffSource;
            this.falloffInterpolator = falloffInterpolator;
        }
        public Select(INoiseProvider controlSource, INoiseProvider lowSource, INoiseProvider highSource, double threshold, double falloff = 0.0)
        {
            this.controlSource = controlSource;
            this.lowSource = lowSource;
            this.highSource = highSource;
            this.thresholdSource = new ConstNoise(threshold);
            this.falloffSource = new ConstNoise(falloff);
            this.falloffInterpolator = new LinearInterpolator();
        }
        public Select(INoiseProvider controlSource, double low, double high, double threshold, double falloff = 0.0)
        {
            this.controlSource = controlSource;
            this.lowSource = new ConstNoise(low);
            this.highSource = new ConstNoise(high);
            this.thresholdSource = new ConstNoise(threshold);
            this.falloffSource = new ConstNoise(falloff);
            this.falloffInterpolator = new LinearInterpolator();
        }

        public double Get1D(double x)
        {
            double control = this.controlSource.Get1D(x);
            double falloff = this.falloffSource.Get1D(x);
            double threshold = this.thresholdSource.Get1D(x);

            if(falloff > 0.0)
            {
                if(control < (threshold-falloff))
                {
                    // Lies outside of falloff area below threshold, return first source
                    return this.lowSource.Get1D(x);
                }
                else if(control > (threshold+falloff))
                {
                    // Lise outside of falloff area above threshold, return second source
                    return this.highSource.Get1D(x);
                }
                else
                {
                    // Lies within falloff area.
                    double lower = threshold-falloff;
                    double upper = threshold+falloff;
                    double blend = fade((control-lower)/(upper-lower));
                    return this.falloffInterpolator.Interpolate(this.lowSource.Get1D(x), this.highSource.Get1D(x), blend);
                }
            }
            else
            {
                if(control < threshold) 
                    return this.lowSource.Get1D(x);
                else
                    return this.highSource.Get1D(x);
            }
        }

        public double Get2D(double x, double y)
        {
            double control = this.controlSource.Get2D(x, y);
            double falloff = this.falloffSource.Get2D(x, y);
            double threshold = this.thresholdSource.Get2D(x, y);

            if(falloff > 0.0)
            {
                if(control < (threshold-falloff))
                {
                    // Lies outside of falloff area below threshold, return first source
                    return this.lowSource.Get2D(x, y);
                }
                else if(control > (threshold+falloff))
                {
                    // Lise outside of falloff area above threshold, return second source
                    return this.highSource.Get2D(x, y);
                }
                else
                {
                    // Lies within falloff area.
                    double lower = threshold-falloff;
                    double upper = threshold+falloff;
                    double blend = fade((control-lower)/(upper-lower));
                    return this.falloffInterpolator.Interpolate(this.lowSource.Get2D(x, y), this.highSource.Get2D(x, y), blend);
                }
            }
            else
            {
                if(control < threshold) 
                    return this.lowSource.Get2D(x, y);
                else
                    return this.highSource.Get2D(x, y);
            }
        }

        public double Get3D(double x, double y, double z)
        {
            double control = this.controlSource.Get3D(x, y, z);
            double falloff = this.falloffSource.Get3D(x, y, z);
            double threshold = this.thresholdSource.Get3D(x, y, z);

            if(falloff > 0.0)
            {
                if(control < (threshold-falloff))
                {
                    // Lies outside of falloff area below threshold, return first source
                    return this.lowSource.Get3D(x, y, z);
                }
                else if(control > (threshold+falloff))
                {
                    // Lise outside of falloff area above threshold, return second source
                    return this.highSource.Get3D(x, y, z);
                }
                else
                {
                    // Lies within falloff area.
                    double lower = threshold-falloff;
                    double upper = threshold+falloff;
                    double blend = fade((control-lower)/(upper-lower));
                    return this.falloffInterpolator.Interpolate(this.lowSource.Get3D(x, y, z), this.highSource.Get3D(x, y, z), blend);
                }
            }
            else
            {
                if(control < threshold) 
                    return this.lowSource.Get3D(x, y, z);
                else
                    return this.highSource.Get3D(x, y, z);
            }
        }

        private static double fade(double t) 
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
    }
}

