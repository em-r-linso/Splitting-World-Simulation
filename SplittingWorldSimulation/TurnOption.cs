using System;

namespace SplittingWorldSimulation
{
	public class TurnOption
	{
		public TurnOption(string title, Action action, params Tile[] participants)
		{
			Action       = action;
			Title        = title;
			Participants = participants;
		}

		public TurnOption(string title, Action action, params string[] messages)
		{
			Action   = action;
			Title    = title;
			Messages = messages;
		}

		public Action   Action       { get; set; }
		public string   Title        { get; set; }
		public Tile[]   Participants { get; set; }
		public string[] Messages     { get; set; }
	}
}