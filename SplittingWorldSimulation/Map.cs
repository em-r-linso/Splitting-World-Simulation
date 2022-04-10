using System;
using System.Collections.Generic;
using System.Linq;

namespace SplittingWorldSimulation
{
	public class Map
	{
		public Map(int maxEra,
		           int maxCentury,
		           int worldPopulationRequirementIncreasePerCentury,
		           int eraCooldown)
		{
			Tiles                      = new();
			Era                        = 0;
			Century                    = 0;
			WorldPopulationRequirement = 0;

			MaxEra                                       = maxEra;
			MaxCentury                                   = maxCentury;
			WorldPopulationRequirementIncreasePerCentury = worldPopulationRequirementIncreasePerCentury;

			EraCooldownMax = EraCooldown = eraCooldown;
		}

		Dictionary<int, Tile> Tiles                                        { get; }
		int                   Era                                          { get; set; }
		int                   Century                                      { get; set; }
		int                   MaxEra                                       { get; }
		int                   MaxCentury                                   { get; }
		int                   WorldPopulationRequirement                   { get; set; }
		int                   WorldPopulationRequirementIncreasePerCentury { get; }
		int                   EraCooldown                                  { get; set; }
		int                   EraCooldownMax                               { get; }

		public void Simulate()
		{
			AdvanceToNextEra();

			while (true)
			{
				if (EraCooldown <= 0)
				{
					var worldPopulation = Tiles.Values.Sum(tile => tile.Population);
					if (worldPopulation < WorldPopulationRequirement)
					{
						AdvanceToNextEra();
						continue;
					}
				}

				AdvanceToNextCentury();
			}

			// ReSharper disable once FunctionNeverReturns
		}

		void AdvanceToNextCentury()
		{
			Century++;

			if (Century > MaxCentury)
			{
				throw new("Too many centuries passed. Try changing some settings and trying again.");
			}

			Program.Write(1, $"CENTURY {Century,2} ----------------------------------------------------------");

			WorldPopulationRequirement += WorldPopulationRequirementIncreasePerCentury;
			EraCooldown--;

			// randomly order the tile keys, so that they take their turns in a random order
			var tileKeys = Tiles.Keys.ToArray().OrderBy(x => Program.Random.Next()).ToArray();

			float year = (Century - 1) * 100;
			foreach (var tileKey in tileKeys)
			{
				Tiles[tileKey].TakeTurn(Era, (int)year);
				year += 100f / tileKeys.Length;
			}
		}

		void AdvanceToNextEra()
		{
			Program.Write(0, $"END OF ERA {Era,2} ====================================================");
			foreach (var tile in Tiles)
			{
				Program.Write(1, tile.Value.ToString());
			}

			Era++;

			Program.Write(0, $"ERA {Era,2} ================================================================");

			if (Era > MaxEra)
			{
				Environment.Exit(0);
			}

			EraCooldown = EraCooldownMax;
			Century     = 0;

			AddNewLand();

			switch (Era)
			{
				case 1:
					// nothing needs doing
					break;
				case 2:
					Disconnect((11, 16), (14, 15), (13, 14));
					Connect(
						(11, 22),
						(11, 21),
						(12, 22),
						(13, 22),
						(13, 23),
						(14, 24),
						(15, 25),
						(15, 26),
						(16, 26)
					);
					break;
				case 3:
					Disconnect(
						(21, 22),
						(22, 23),
						(23, 24),
						(25, 26),
						(15, 25),
						(13, 23),
						(11, 21)
					);
					Connect(
						(22, 32),
						(23, 33),
						(24, 34),
						(24, 35),
						(25, 35),
						(15, 36),
						(26, 36),
						(26, 31),
						(21, 31)
					);
					break;
				case 4:
					Disconnect((31, 32), (32, 33), (35, 36));
					Connect(
						(31, 41),
						(32, 42),
						(33, 43),
						(33, 44),
						(34, 44),
						(35, 44),
						(35, 45),
						(36, 41),
						(36, 46)
					);
					break;
				case 5:
					Disconnect(
						(41, 46),
						(46, 45),
						(45, 44),
						(43, 44)
					);
					Connect(
						(51, 36),
						(51, 41),
						(52, 41),
						(52, 42),
						(52, 43),
						(53, 43),
						(54, 44),
						(55, 45),
						(56, 46)
					);
					break;
				case 6:
					Disconnect(
						(51, 52),
						(52, 53),
						(53, 54),
						(54, 55),
						(55, 56),
						(51, 41),
						(36, 51)
					);
					Connect(
						(15, 51),
						(36, 51),
						(51, 61),
						(61, 56),
						(56, 66),
						(65, 55),
						(64, 54),
						(63, 53),
						(62, 52)
					);
					Freeze(
						11,
						12,
						13,
						14
					);
					break;
				case 7:
					Disconnect(
						(61, 62),
						(62, 63),
						(63, 64),
						(65, 66),
						(51, 15),
						(51, 36)
					);
					Connect(
						(51, 15),
						(71, 61),
						(71, 66),
						(76, 66),
						(75, 65),
						(75, 64),
						(74, 64),
						(73, 63),
						(72, 62)
					);
					Freeze(
						24,
						25,
						23,
						21,
						22
					);
					break;
				case 8:
					Disconnect(
						(71, 72),
						(72, 73),
						(75, 76),
						(51, 15)
					);
					Connect(
						(81, 71),
						(82, 72),
						(83, 73),
						(84, 74),
						(85, 75),
						(86, 76),
						(81, 76),
						(84, 73),
						(84, 75)
					);
					Freeze(
						33,
						34,
						35,
						32,
						31,
						26,
						16
					);
					break;
				case 9:
					Disconnect(
						(83, 84),
						(85, 86),
						(81, 86),
						(83, 73)
					);
					Connect(
						(81, 91),
						(82, 92),
						(83, 93),
						(84, 94),
						(85, 95),
						(86, 96),
						(91, 76),
						(92, 81),
						(94, 73),
						(95, 84)
					);
					Freeze(
						44,
						45,
						41,
						42,
						43,
						36,
						15
					);
					break;
				default:
					throw new ArgumentException("Era must be between 1 and 9");
			}
		}

		void AddNewLand()
		{
			// create six new tiles indexed ?1-?6
			for (var i = 1; i <= 6; i++)
			{
				var tileIndex = TileIndexFromIndex(i);
				Tiles[tileIndex] = new(tileIndex);
			}

			// connect new tiles in a circle, because they will always be in that shape
			for (var a = 1; a <= 6; a++)
			{
				var b = a < 6 ? a + 1 : 1;
				Connect(TileIndexFromIndex(a), TileIndexFromIndex(b));
			}

			// spawn a new race on one of the new tiles
			Tiles[TileIndexFromIndex(Program.Random.Next(6) + 1)].SpawnNewRace();
		}

		int TileIndexFromIndex(int index)
		{
			var tileIndexString = $"{Era}{index}";
			var tileIndex       = int.Parse(tileIndexString);
			return tileIndex;
		}

		// create connections between two tiles
		void Connect(int a, int b)
		{
			Tiles[a].Connect(Tiles[b]);
			Tiles[b].Connect(Tiles[a]);
		}

		// connection shorthand allowing as many tuples as needed
		void Connect(params (int, int)[] pairs)
		{
			foreach (var (a, b) in pairs)
				Connect(a, b);
		}

		// sever connections between two tiles
		void Disconnect(int a, int b)
		{
			Tiles[a].Disconnect(Tiles[b]);
			Tiles[b].Disconnect(Tiles[a]);
		}

		// disconnection shorthand allowing as many tuples as needed
		void Disconnect(params (int, int)[] pairs)
		{
			foreach (var (a, b) in pairs)
				Disconnect(a, b);
		}

		void Freeze(params int[] tiles)
		{
			foreach (var tile in tiles)
				Tiles[tile].Freeze();
		}
	}
}