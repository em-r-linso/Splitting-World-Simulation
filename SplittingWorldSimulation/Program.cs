using System;
using System.Collections.Generic;

namespace SplittingWorldSimulation
{
	internal class Program
	{
		public static readonly Random Random = new();

		static readonly List<string> RandomNames = new()
		{
			"Alpha",
			"Beta",
			"Gamma",
			"Delta",
			"Epsilon",
			"Theta",
			"Iota",
			"Kappa",
			"Lambda",
			"Nu",
			"Omicron",
			"Pi",
			"Rho",
			"Sigma",
			"Tau",
			"Omega",
			"Bravo",
			"Charlie",
			"Echo",
			"Foxtrot",
			"Golf",
			"Hotel",
			"India",
			"Juliet",
			"Kilo",
			"Lima",
			"Mike",
			"November",
			"Oscar",
			"Papa",
			"Quebec",
			"Romeo",
			"Sierra",
			"Tango",
			"Uniform",
			"Victor",
			"Whiskey",
			"X-ray",
			"Yankee",
			"Zulu",
			"Ram",
			"Bull",
			"Twins",
			"Crab",
			"Lion",
			"Maiden",
			"Scales",
			"Scorpion",
			"Centaur",
			"Goat",
			"Fish",
			"Fool",
			"Magician",
			"Priestess",
			"Empress",
			"Hierophant",
			"Lovers",
			"Chariot",
			"Strength",
			"Hermit",
			"Wheel",
			"Justice",
			"Hanged",
			"Death",
			"Temperance",
			"Devil",
			"Tower",
			"Star",
			"Moon",
			"Sun",
			"Judgement"
		};

		static void Main(string[] args)
		{
			// TODO: set maxEra to 9 when done testing
			var map = new Map(
				9,
				50,
				1,
				3
			);
			map.Simulate();
		}

		public static void Write(int indent, string s)
		{
			var tabs = "";

			for (var i = 0; i < indent; i++)
			{
				tabs += "  ";
			}

			Console.WriteLine(tabs + s);
		}

		// returns a random name after removing it from the list
		public static string GetRandomName()
		{
			if (RandomNames.Count == 0)
			{
				throw new("Not enough names!");
			}

			var randomIndex = Random.Next(RandomNames.Count);
			var randomName  = RandomNames[randomIndex];
			RandomNames.RemoveAt(randomIndex);
			return randomName;
		}
	}
}