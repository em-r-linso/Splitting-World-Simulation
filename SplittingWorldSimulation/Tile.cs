using System;
using System.Collections.Generic;
using System.Linq;

namespace SplittingWorldSimulation
{
	public class Tile
	{
		const int MaxPopulation                  = 99;
		const int MaxProgress                    = 99;
		const int IdeologicalThresholdForTension = 5;
		const int ChanceOfRandomDecline          = 10;
		const int ChanceOfRandomRuin             = 10;
		const int ChanceOfRandomRecovery         = 10;

		int    _population;
		int    _progress;
		string _tileName;
		string _toStringPrevious;

		public Tile(int tileIndex)
		{
			TileIndex = tileIndex;
			Neighbors = new();
			Ruins     = new();
		}

		// automatically update record population when this is updated
		public int Population
		{
			get => _population;
			private set
			{
				value = Math.Clamp(value, 0, MaxPopulation);

				if (value > RecordPopulation)
				{
					RecordPopulation = value;
				}

				_population = value;
			}
		}

		// automatically update record progress when this is updated
		int Progress
		{
			get => _progress;
			set
			{
				value = Math.Clamp(value, 0, MaxProgress);

				if (value > RecordProgress)
				{
					RecordProgress = value;
				}

				_progress = value;
			}
		}

		string ToStringPrevious
		{
			get
			{
				var returnValue = _toStringPrevious;
				_toStringPrevious = ToString();
				return returnValue;
			}
		}

		// when tile name is gotten for the first time, it picks a random name
		string TileName
		{
			get
			{
				_tileName ??= $"{Program.GetRandomName()} ({TileIndex})";
				return _tileName;
			}
		}

		Tile[] PopulatedNeighbors
		{
			get { return Neighbors.Where(n => n.Population > 0).ToArray(); }
		}

		Tile[] TenseNeighbors
		{
			get { return PopulatedNeighbors.Where(n => Ideology.Compare(Ideology, n.Ideology) > IdeologicalThresholdForTension).ToArray(); }
		}

		Tile[] UnpopulatedNeighbors
		{
			get { return Neighbors.Where(n => n.Population == 0).ToArray(); }
		}

		List<Tile> Neighbors        { get; }
		bool       Frozen           { get; set; }
		bool       Declining        { get; set; }
		Ideology   Ideology         { get; set; }
		int        RecordPopulation { get; set; }
		int        RecordProgress   { get; set; }
		string     RaceName         { get; set; }
		int        TileIndex        { get; }
		int        Era              { get; set; }
		int        Year             { get; set; }
		List<Ruin> Ruins            { get; }

		public void Connect(Tile neighbor) { Neighbors.Add(neighbor); }

		public void Disconnect(Tile neighbor) { Neighbors.Remove(neighbor); }

		public void Freeze()
		{
			Program.Write(1, $"{TileIndex} froze over.");
			Frozen = true;
		}

		public void SpawnNewRace()
		{
			RaceName = Program.GetRandomName();

			Program.Write(1, $"The {RaceName} race appeared on {TileName}.");

			Ideology   = new();
			Population = 1;
			Progress   = 0;
		}

		public void TakeTurn(int era, int year)
		{
			// update ToStringPrevious
			var tsp = ToStringPrevious;

			// update era and year
			Era  = era;
			Year = year;

			// add options that this tile can do
			var turnOptions = new List<TurnOption>();

			if (Declining)
			{
				// recovery from decline
				if (Program.Random.Next(100) < ChanceOfRandomRecovery)
				{
					turnOptions.Add(
						new(
							"RECOVER FROM DECLINE",
							() => { Declining = false; },
							$"{TileName} bounced back from its decline."
						)
					);
				}

				// become ruins if population is 0 (or as a random chance when declining)
				else if ((Population == 0) || (Program.Random.Next(100) < ChanceOfRandomRuin))
				{
					turnOptions.Add(
						new(
							"FALL TO RUIN",
							() =>
							{
								Ruins.Add(new() {Ideology = Ideology, Progress = Progress});
								Declining        = false;
								Ideology         = null;
								Population       = 0;
								RecordPopulation = 0;
								Progress         = 0;
								RecordProgress   = 0;
								RaceName         = null;
							},
							$"{TileName} became ruins."
						)
					);
				}

				// loss to decline
				else
				{
					if (Population > 0)
					{
						turnOptions.Add(new("POPULATION LOSS", () => { Population = (int)(Population * 0.8); }, this));
					}

					if (Progress > 0)
					{
						turnOptions.Add(new("PROGRESS LOSS", () => { Progress = (int)(Progress * 0.8); }, this));
					}
				}
			}
			else
			{
				if (Population > 0)
				{
					// random decline
					if (Program.Random.Next(100) < ChanceOfRandomDecline)
					{
						turnOptions.Add(new("DECLINE", () => { Declining = true; }, $"{TileName} began to decline."));
					}
					else
					{
						// normal growth
						turnOptions.Add(new("POPULATION GROWTH", () => { Population++; }, this));
						turnOptions.Add(new("PROGRESS GROWTH", () => { Progress++; }, this));
						turnOptions.Add(new("CULTURAL SHIFT", () => { Ideology.Shift(); }, this));

						// interactions with neighbors
						if (PopulatedNeighbors.Length > 0)
						{
							{
								var neighbor = PopulatedNeighbors[Program.Random.Next(PopulatedNeighbors.Length)];
								turnOptions.Add(
									new(
										"CULTURAL EXCHANGE",
										() =>
										{
											Ideology.Influence(neighbor.Ideology);
											neighbor.Ideology.Influence(Ideology);
										},
										this,
										neighbor
									)
								);
							}

							if (TenseNeighbors.Any())
							{
								// TODO: increase tension instead of going straight to raids; they can reconcile later if they become more similar
								var neighbor = TenseNeighbors[Program.Random.Next(TenseNeighbors.Length)];
								turnOptions.Add(
									new(
										"RAID",
										() =>
										{
											neighbor.Progress--;
											Progress++;
											neighbor.Population--;
											Population--;
										},
										this,
										neighbor
									)
								);
							}
						}

						// interactions with unpopulated tiles
						if (UnpopulatedNeighbors.Length > 0)
						{
							var neighbor = UnpopulatedNeighbors[Program.Random.Next(UnpopulatedNeighbors.Length)];
							turnOptions.Add(
								new(
									"SPREAD",
									() =>
									{
										neighbor.RaceName   = RaceName;
										neighbor.Ideology   = Ideology;
										neighbor.Progress   = Progress;
										neighbor.Population = 1;
									},
									this,
									neighbor
								)
							);
						}

						// interactions with ruins
						if (Ruins.Any())
						{
							turnOptions.Add(
								new(
									"GAIN CULTURE FROM RUINS",
									() =>
									{
										Ruins[Program.Random.Next(Ruins.Count)].Ideology.Influence(Ideology);
									},
									this
								)
							);
						}
					}
				}
			}

			// pick a turn option (or skip if there are no options)
			if (turnOptions.Any())
			{
				ExecuteTurnOption(turnOptions[Program.Random.Next(turnOptions.Count)]);
			}
		}

		void ExecuteTurnOption(TurnOption option)
		{
			// title
			Program.Write(2, $"{Era}—{Year}: {option.Title}");

			// pre report
			if (option.Participants != null)
			{
				foreach (var participant in option.Participants)
					Program.Write(3, participant.ToString());
			}

			// change
			option.Action();

			// post report
			if (option.Participants != null)
			{
				Program.Write(3, "                    ↓     ↓        ↓       ↓");
				foreach (var participant in option.Participants)
					Program.Write(3, participant.ToString());
			}

			// messages
			if (option.Messages != null)
			{
				foreach (var message in option.Messages)
					Program.Write(3, message);
			}
		}

		public override string ToString() =>
			$"|{TileName,16}|{Population,2}/{RecordPopulation,2}|{Progress,2}/{RecordProgress,2}|{((Ideology == null)?"           ":Ideology)}|{Ruins.Count,2}";

		struct Ruin
		{
			public Ideology Ideology { get; set; }
			public int      Progress { get; set; }
		}
	}
}