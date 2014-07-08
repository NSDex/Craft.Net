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
using Craft.Net.TerrainGeneration.CoherentNoise.Transform;


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
                    int value = (int)(noiseSrc.Get2D(chunk.X * Chunk.Width + x, chunk.Z * Chunk.Depth + z));

                    for (int h=0; h<value; h++)
                    {
                        chunk.SetBlockId(new Coordinates3D(x, h, z), 1);
                        chunk.SetMetadata(new Coordinates3D (x, h, z), 0);
                        chunk.SetBiome((byte)x, (byte)z, Biome.ExtremeHills);
                    }

                    /*
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
                    }*/
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

            this.noiseSrc = new CellNoise(123456, CellNoise.EuclidianDistanceFunc, CellNoise.InsertShortest, CellNoise.ChooseShortest);

            INoiseProvider integerNoise = new IntegerNoise((int)this.Seed);
            this.noiseSrc = new VoronoiNoise(new[] { new IntegerNoise((int)this.Seed), new IntegerNoise((int)this.Seed + 1) }, 
                CellNoise.EuclidianDistanceFunc, 
                (Vector3 inputPoint, Vector3 cellPoint, double distance) => { return integerNoise.Get2D( (int)Math.Floor(cellPoint.X), (int)Math.Floor(cellPoint.Y) ); });
            this.noiseSrc = new Multiply(new ConstNoise(40), this.noiseSrc);
            this.noiseSrc = new Abs(this.noiseSrc);
            this.noiseSrc = new Add(new ConstNoise(30), this.noiseSrc);
            this.noiseSrc = new Scale(this.noiseSrc, new Vector3(1.0/20.0, 1.0/20.0, 1.0/20.0));

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

