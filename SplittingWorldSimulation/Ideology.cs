using System;
using System.Linq;

namespace SplittingWorldSimulation
{
	public class Ideology
	{
		readonly IdeologicalValue[] Values;

		public Ideology()
		{
			Values = new[]
			{
				// 8values / Humankind ideologies
				new IdeologicalValue("economic",   "equality",  "markets"),
				new IdeologicalValue("diplomatic", "nation",    "globe"),
				new IdeologicalValue("civil",      "liberty",   "authority"),
				new IdeologicalValue("society",    "tradition", "progress")
			};
		}

		public static int Compare(Ideology a, Ideology b)
		{
			var difference = 0;

			for (var value = 0; value < a.Values.Length; value++)
			{
				difference += Math.Abs(a.Values[value].Leaning - b.Values[value].Leaning);
			}

			return difference;
		}

		public void Influence(Ideology b)
		{
			for (var value = 0; value < Values.Length; value++)
			{
				var difference = Values[value].Leaning - b.Values[value].Leaning;
				b.Values[value].Leaning += Math.Clamp(difference, -1, 1);
			}
		}

		// for each value, add or remove 1, or stay the same
		public void Shift()
		{
			foreach (var value in Values)
				value.Leaning += Program.Random.Next(3) - 1;
		}

		public override string ToString()
		{
			var leanings = Values.Select(value => $"{value.Leaning,2}").ToArray();
			return string.Join(" ", leanings);
		}
	}
}