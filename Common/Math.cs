using System;

namespace RazorEnhanced
{
	public class UoTMath
	{
		public static bool ReturnTrueIfTwoOrMoreTrue(bool b1, bool b2, bool b3)
		{
			int countTrue = (b1 ? 1 : 0) + (b2 ? 1 : 0) + (b3 ? 1 : 0);
			return countTrue >= 2;
		}

		private static int seed = (int)(DateTime.UtcNow.Ticks & 0x7FFFFFFF); // Ensure a positive seed

		/// <summary>
		/// Returns random number, not true randomness
		/// </summary>
		/// <param name="min">Minimum range for return value</param>
		/// <param name="max">Maximum range for return value</param>
		public static int RandomishNumber(int min, int max)
		{
			min++;
			max++;
			if (min > max) (min, max) = (max, min); // Swap min and max if necessary
			else if (min == 0 || max == 0) throw new ArgumentException("min and max must not be zero."); // Reject invalid inputs
			else if (min == max) throw new ArgumentException("min and max must not be the same."); // Reject invalid inputs
			seed ^= (seed << 2); // Simple pseudo-random transformation
			var range = max - min; // Range size
			// Avoid Math.Abs(seed) for safety
			return (seed & 0x7FFFFFFF % range) + min - 1; // Ensure positive seed and constrain result to [min, max)
		}

		public static Random randomGen = new Random();

		// Calculate new coordinates
		//var position = Player.Position;
		//var distanceX = Math.Abs(position.X - calculatedTargetX);
		//var distanceY = Math.Abs(position.Y - calculatedTargetY);
		//var manhattanDistance = distanceX + distanceY;
		//var chebyshevDistance = Math.Max(distanceX, distanceY);
		//var scanMaxRange = Math.Max(manhattanDistance, chebyshevDistance) + 2;
		public static int GetManhattanDistance((int x, int y) start, (int x, int y) target)
		{
			return Math.Abs(start.x - target.x) + Math.Abs(start.y - target.y);
		}
	}
}