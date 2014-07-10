using System;

namespace Craft.Net.TerrainGeneration.CoherentNoise.Modifier
{
	public class FastSmooth3x3 : INoiseProvider
	{
		INoiseProvider source;

		public FastSmooth3x3(INoiseProvider source)
		{
			this.source = source;
		}

		public double Get1D(double x) 
		{ 
			// 1/4 + 1/4 + 1/2 = 1;

			return source.Get1D(x) / 2 + source.Get1D(x - 1) / 4 + source.Get1D(x + 1) / 4;
		}

		public double Get2D(double x, double y) 
		{ 
			// 4/16 + 4/8 + 1/4 = 1

			// -- +- -+ ++
			double corners = (source.Get2D(x - 1, y - 1) + source.Get2D(x + 1, y - 1) + source.Get2D(x - 1, y + 1) + source.Get2D(x + 1, y + 1)) / 16;
			// -0 +0 0- 0+
			double sides = (source.Get2D(x - 1, y) + source.Get2D(x + 1, y) + source.Get2D(x, y - 1) + source.Get2D(x, y + 1)) / 8;
			// 00
			double center = source.Get2D(x, y) / 4;
			return corners + sides + center;
		}

		public double Get3D(double x, double y, double z) 
		{ 
			// 12/48 + 8/32 + 6/16 + 1/8 = 1

			// ++0 -+0 0++ 0+-   +-0 --0 0-+ 0--   +0+ +0- -0+ -0-
			double edges = 0;
			edges += source.Get3D(x + 1, y + 1, z) + source.Get3D(x - 1, y + 1, z) + source.Get3D(x, y + 1, z + 1) + source.Get3D(x, y + 1, z - 1);
			edges += source.Get3D(x + 1, y - 1, z) + source.Get3D(x - 1, y - 1, z) + source.Get3D(x, y - 1, z + 1) + source.Get3D(x, y - 1, z - 1);
			edges += source.Get3D(x + 1, y, z + 1) + source.Get3D(x + 1, y, z - 1) + source.Get3D(x - 1, y, z + 1) + source.Get3D(x - 1, y, z - 1);
			edges /= 48;
			// --- --+ -+- -++ +-- +-+ ++- +++
			double corners = 0;
			corners += source.Get3D(x - 1, y - 1, z - 1) + source.Get3D(x - 1, y - 1, z + 1) + source.Get3D(x - 1, y + 1, z - 1) + source.Get3D(x - 1, y + 1, z + 1);
			corners += source.Get3D(x + 1, y - 1, z - 1) + source.Get3D(x + 1, y - 1, z + 1) + source.Get3D(x + 1, y + 1, z - 1) + source.Get3D(x + 1, y + 1, z + 1);
			corners /= 32;
			// +00 -00 0+0 0-0 00+ 00-
			double sides = 0;
			corners += source.Get3D(x - 1, y, z) + source.Get3D(x - 1, y, z) + source.Get3D(x, y + 1, z);
			corners += source.Get3D(x, y - 1, z) + source.Get3D(x, y, z + 1) + source.Get3D(x, y, z - 1);
			corners /= 16;
			// 000
			double center = source.Get3D(x, y, z) / 8;

			return corners + sides + center;
		}
	}
}

