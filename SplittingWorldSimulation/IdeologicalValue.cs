using System;

namespace SplittingWorldSimulation
{
	public class IdeologicalValue
	{
		const int MaxLeaning = 9;

		int _leaning;
		public int Leaning
		{
			get => _leaning;
			set => _leaning = Math.Clamp(value, -MaxLeaning, MaxLeaning);
		}

		public string Name, ExtremeA, ExtremeB;

		public IdeologicalValue(string name, string extremeA, string extremeB)
		{
			Name     = name;
			ExtremeA = extremeA;
			ExtremeB = extremeB;

			Leaning = 0;
		}
	}
}