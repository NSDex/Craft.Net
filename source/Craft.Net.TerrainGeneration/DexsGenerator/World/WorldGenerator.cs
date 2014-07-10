using System;

namespace Craft.Net.TerrainGeneration.Dex
{
    public class WorldGenerator
    {
        protected WorldOptions options;
        protected long seed;

        public WorldGenerator(long seed, WorldOptions options = null)
        {
            this.seed = seed;
            this.options = (options != null) ? options : new WorldOptions();
        }
    }
}

