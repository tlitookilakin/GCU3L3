using System.Text;

namespace DiceRoller;

class Program
{
	const int MAX_SIDES = 10;

	static readonly Dictionary<string, int[]> totalScores = new() 
	{
		{"Win!", [7, 11, 18]},
		{"Craps", [2, 3, 12, 16]},
		{"Natural 20!", [20]}
	};
	static readonly Dictionary<(int, int), string> combos = new()
	{
		{(1, 2), "Ace Deuce"},
		{(3, 5), "Lesser Primes"},
		{(7, 9), "Greater Primes"}
	};
	static readonly string?[] doubles = [
		"Snake Eyes", "Deuces", null, null, "Double Bluff", "Boxcars", "Lucky Sevens", "Doubles", "The Tower", "Double Bluff"
	];

	static void Main(string[] args)
	{
		// welcome
		Console.WriteLine("Welcome to the dice roller!");

		// main loop
		do
		{
			Console.WriteLine("How many sides should the dice have?");
			int sides = GetNumberInput();

			if (sides == 1)
			{
				// snark, for fun. A one-sided die will only ever roll ones.
				Console.WriteLine("\"Oh look at that, a 2. Why? Because I'm the GM and I say so.\"\n");
			}
			else
			{
				// roll a number and print the results
				Console.WriteLine($"Rolling 2 dice with {sides} sides each...");
				PrintResults(GetRolls(sides, out int die2), die2);
			}
		}
		while (PromptYesNo(true, "Would you like to roll again?"));

		// goodbye
		Console.WriteLine("Thanks for playing! Press any key to exit...");
		Console.ReadKey();
	}

	static void PrintResults(int die1, int die2)
	{
		// display rolled numbers
		Console.WriteLine($"Rolled {die1} & {die2}.");

		// get combos and wins
		string combos = GetCombos(die1, die2).Trim();
		string wins = GetWins(die1, die2).Trim();
		StringBuilder sb = new(combos.Length + wins.Length + 3);

		// add combos and wins, comma-separated if valid
		sb.Append(combos);
		if (combos.Length != 0)
			sb.Append(", ");
		sb.Append(wins);

		// print combos & wins
		var result = sb.ToString();
		if (result.Length != 0)
			Console.WriteLine(result);

		// spacing
		Console.WriteLine();
	}

	static string GetWins(int die1, int die2)
	{
		// total score
		int total = die1 + die2;

		// check and see if there's a matching win condition
		foreach ((string name, int[] totals) in totalScores)
			if (totals.Contains(total))
				return name;

		// no matches, return empty
		return string.Empty;
	}

	static string GetCombos(int die1, int die2)
	{
		// check for doubles
		if (die1 == die2 && doubles[die1 - 1] is string dubs)
			return dubs;

		// check combo, and combo reverse
		else if (combos.TryGetValue((die1, die2), out string? combo) || combos.TryGetValue((die2, die1), out combo))
			return combo;

		// no combo, return empty
		return string.Empty;
	}

	static int GetRolls(int sides, out int roll2)
	{
		// get rng
		Random rand = Random.Shared;

		// roll numbers
		roll2 = rand.Next(sides) + 1;
		return rand.Next(sides) + 1;
	}

	static int GetNumberInput()
	{
		while (true)
		{
			// get input
			var line = Console.ReadLine();

			try
			{
				// try to read
				int sides = int.Parse(line!);

				// too big
				if (sides > MAX_SIDES)
					Console.Write($"{sides} is too many sides for a die!");

				// in range
				else if (sides > 0)
					return sides;

				// too small
				else
					Console.Write("Non-euclidean dice are not allowed!");
			}
			catch (OverflowException)
			{
				// too big
				Console.Write($"{line} is too many sides for a die!");
			}
			catch
			{
				// something else
				Console.Write($"{line} isn't a number!");
			}

			// prompt for re-entry
			Console.WriteLine(" Please enter a different value.");
		}
	}

	static bool PromptYesNo(bool allowEscape, string message)
	{
		Console.WriteLine(message + " [Y/N]");

		while (true)
		{
			// get keystroke
			char key = Console.ReadKey().KeyChar;

			// deletes echoed keystroke from output
			Console.Write("\b\\\b");

			// process keystroke
			switch (key)
			{
				// yes
				case 'y':
				case 'Y':
					Console.Clear();
					return true;

				// no
				case 'n':
				case 'N':
					return false;

				// escape key
				case '\x1b':
					if (allowEscape)
						return false;
					break;
			}
		}
	}
}
