using System;
using Craft.Net.Anvil;
using Craft.Net.Logic;
using Craft.Net.Common;
using System.Collections.Generic;
using System.Collections;
using Craft.Net.TerrainGeneration.CoherentNoise;
using Craft.Net.TerrainGeneration.CoherentNoise.Generator;
using Craft.Net.TerrainGeneration.CoherentNoise.Modifier;
using Craft.Net.TerrainGeneration.CoherentNoise.Combiner;


namespace Craft.Net.TerrainGeneration
{
	public class DexsGenerator : IWorldGenerator
	{
		public string LevelType { get { return "DEFAULT"; } }

		public string GeneratorName { get { return "DEX'S GENERATOR"; } }

		public string GeneratorOptions { get; set; }

		public long Seed { get; set; }

		public Vector3 SpawnPoint { get; set; }


		INoiseProvider noiseSrc;

		public Chunk GenerateChunk(Coordinates2D position)
		{
			//Make a new Chunk
			var chunk = new Chunk(position);

			for (int x = 0; x < 16; x++)
			{
				for (int z = 0; z < 16; z++) 
				{
                    for (int y = 0; y < 128; y++)
                    {
                        int value = (int)(noiseSrc.Get3D(chunk.X * Chunk.Width + x, y, chunk.Z * Chunk.Depth + z)); 

                        if (value == -1)
                        {
                            chunk.SetBlockId(new Coordinates3D(x, y, z), 1);
                            chunk.SetMetadata(new Coordinates3D (x, y, z), 0);
                            chunk.SetBiome((byte)x, (byte)z, Biome.ExtremeHills);
                        }
                        else
                        {
                            chunk.SetBlockId(new Coordinates3D(x, y, z), 0);
                            chunk.SetMetadata(new Coordinates3D (x, y, z), 0);
                            chunk.SetBiome((byte)x, (byte)z, Biome.ExtremeHills);
                        }
                    }
				}
			}

			return chunk;
		}

		/// <summary>
		/// Called after the world generator is created and
		/// all values are set.
		/// </summary>
		public void Initialize(Level level)
		{
            SpawnPoint = new Vector3(0, 129, 0);

            this.noiseSrc = new GradientNoise(new Vector3(0, 0, 0), new Vector3(0, 128, 0));
            this.noiseSrc = new Select(this.noiseSrc, -1, 1, 0);

            /*
			const double persistence = 1, frequency = 0.01, amplitude = 80;
			int octaves = 2;
			//noise = new Noise(persistence, frequency, amplitude, octaves, (int)Seed);
			noiseSrc = new ValueNoise( new FastSmooth3x3( new IntegerNoise((int)Seed) ), new CosineInterpolator() );
			noiseSrc = new FBMFilter(noiseSrc, octaves, frequency, amplitude, 2, persistence);
			noiseSrc = new TranslateFilter( new AbsFilter(noiseSrc), 40 );
			//noiseSrc = new AbsFilter( new VoronoiNoise((int)Seed, 20, 1.0/100.0, true) );
			*/
		}
	}
}

