using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEnhanced
{
	/// <summary>
	/// Manages player movement-related operations.
	/// </summary>
	public partial class UoTMovement
	{
		/// <summary>
		/// Enum representing movement directions.
		/// </summary>
		[Flags]
		public enum Directions : byte
		{
			north = 0x0,
			right = 0x1,
			northeast = 0x1,
			east = 0x2,
			down = 0x3,
			southeast = 0x3,
			south = 0x4,
			left = 0x5,
			southwest = 0x5,
			west = 0x6,
			mask = 0x7,
			up = 0x7,
			northwest = 0x7,
			running = 0x80,
			valuemask = 0x87
		}
		
		/// <summary>
		/// Executes a pathfinding task to move to the given coordinates.
		/// </summary>
		/// <param name="x">X-coordinate.</param>
		/// <param name="y">Y-coordinate.</param>
		/// <param name="z">Z-coordinate (defaults to Player.Position.Z).</param>
		/// <param name="range">Allowed range for the movement (default is 1).</param>
		/// <param name="token">Cancellation token to stop the operation if needed.</param>
		/// <returns>True if the task completed successfully; otherwise false.</returns>
		public static bool PathingTask(int x, int y, int z = (int.MinValue + 1), int range = 1, CancellationToken token = default)
		{
			try
			{
				if (x == 0 && y == 0) return false;
				if (z == (int.MinValue + 1)) z = Player.Position.Z;
				
				AddPointOfInterest((x, y, z));

				// Start the pathing task using TaskManager.
				bool started = UoTasks.StartTask("Pathing", async (cancellationToken) => 
						await RunTowards(x, y, z, range, 3, true, PoiMode.Nearest, UoTasks.GetTaskCancellationToken()));

				// “started” will be false if a task with this name is already running.
				if (!started)
				{
					//UoTLogger.LogErrorToFile("Pathing was already started, added to poi");
				}

				// Give the newly started task a small delay before returning.
				Misc.Pause(50);
				return started;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			return false;
		}
		
		private static (int X, int Y) _lastPosition = default;
		public static Stopwatch PlayerMovementStopwatch = new ();
		
		/// <summary>
		/// Checks if the player has moved within the specified timer interval. Max for60 FPS is 16ms.
		/// </summary>
		/// <param name="timer">Time to check in milliseconds (default is 2000ms).</param>
		/// <returns>True if the player has moved; otherwise false.</returns>
		public static bool PlayerMoved(int timer = 2000)
		{
			var currentPosition = (Player.Position.X, Player.Position.Y);

			// Initialize position and stopwatch if this is the first call
			if (_lastPosition == default)
			{
				_lastPosition = currentPosition;
				PlayerMovementStopwatch.Start();
				return true;
			}
			
			// Check if the player has moved
			if (currentPosition != _lastPosition)
			{
				_lastPosition = currentPosition; // Update position
				PlayerMovementStopwatch.Restart();            // Reset and start the stopwatch
				return true;                    // Player has moved
			}
			// Check if timer ms have passed since the last movement
			return PlayerMovementStopwatch.ElapsedMilliseconds <= timer;
		}
		
		private static (int X, int Y) _lastThresholdPosition = default;
		public static Stopwatch HasPlayerMovedEnoughStopwatch = new ();

		/// <summary>
		/// Checks if the player moved enough tiles within the given time.
		/// </summary>
		/// <param name="timer">Time to check in milliseconds (default is 2000ms).</param>
		/// <param name="threshold">Minimum movement threshold (default is 5).</param>
		/// <returns>True if the movement threshold was exceeded; otherwise false.</returns>
		public static bool HasPlayerMovedEnough(int timer = 2000, int threshold = 5)
		{
			var currentPosition = (Player.Position.X, Player.Position.Y);
			if (_lastThresholdPosition == default)
			{
				_lastThresholdPosition = currentPosition;
				PlayerMovementStopwatch.Start();
				return true;
			}
			
			// Check if the player has moved
			if (Misc.Distance(currentPosition.X, currentPosition.Y, _lastThresholdPosition.X, _lastThresholdPosition.Y) > threshold)
			{
				_lastThresholdPosition = currentPosition; // Update position
				PlayerMovementStopwatch.Restart();            // Reset and start the stopwatch
				return true;                    // Player has moved
			}
			return PlayerMovementStopwatch.ElapsedMilliseconds <= timer;
		}
		
		private static bool HasPlayerMovedByThreshold((int X, int Y) lastCachedPosition, (int X, int Y) currentPosition, int threshold)
		{
			var deltaX = Math.Abs(currentPosition.X - lastCachedPosition.X);
			var deltaY = Math.Abs(currentPosition.Y - lastCachedPosition.Y);
			return deltaX >= threshold || deltaY >= threshold;
		}
		
		/// <summary>
		/// Validates movement from one tile to another.
		/// </summary>
		/// <param name="to">The destination coordinates.</param>
		/// <param name="from">The starting coordinates (default is (0, 0)).</param>
		/// <returns>True if the movement is valid; otherwise false.</returns>
		public static bool ValidMovement((int x, int y) to, (int x, int y) from = default)
		{
			if (from == default) from = (Player.Position.X, Player.Position.Y);
			return ValidMovement(GetDirectionToTarget(from, to), from);
		}
		
		/// <summary>
		/// Validates movement in a specific direction from a starting point.
		/// </summary>
		/// <param name="direction">Direction to move in.</param>
		/// <param name="from">Starting coordinates (default is (0, 0)).</param>
		/// <param name="tiles">Number of tiles to move (default is 1).</param>
		/// <returns>True if the movement is valid; otherwise false.</returns>
		public static bool ValidMovement(Directions direction, (int x, int y) from = default, int tiles = 1)
		{
			if (from == default) from = (Player.Position.X, Player.Position.Y);
			(int x, int y) to = GetCoordinateFromDirection(from.x, from.y, direction);
			if (!UoTiles.IsMovementPossible((to.x, to.y), UoTiles.GetTileInfo(to.x, to.y, Player.Map)))
				return false;
			for (var i = 1; i < tiles; i++)
			{
				to = GetCoordinateFromDirection(to.x, to.y, direction);
				if (!UoTiles.IsMovementPossible((to.x, to.y), UoTiles.GetTileInfo(to.x, to.y, Player.Map)))
					return false;
			}
		    // Check if the movement is diagonal
		    if (!IsDiagonal(direction)) return true;
		    (int x, int y) toAround = GetCoordinateFromDirection(to.x, to.y, TurnLeftHalf(TurnAround(direction)));
		    if (UoTiles.IsMovementPossible((toAround.x, toAround.y), UoTiles.GetTileInfo(toAround.x, toAround.y, Player.Map)))
			    //return true;
		    {
			    toAround = GetCoordinateFromDirection(to.x, to.y, TurnRightHalf(TurnAround(direction)));
			    if (UoTiles.IsMovementPossible((toAround.x, toAround.y), UoTiles.GetTileInfo(toAround.x, toAround.y, Player.Map)))
				    return true;
		    }
		    return false;
		}
		
		/// <summary>
		/// Compares two movement directions to check if they are equal.
		/// </summary>
		/// <param name="dir1">First direction to compare.</param>
		/// <param name="dir2">Second direction to compare.</param>
		/// <returns>True if the directions are equal; otherwise false.</returns>
		public static bool AreDirectionsEqual(Directions dir1, Directions dir2)
		{
			// Use the mask to isolate the basic directional bits and compare
			return (dir1 & Directions.mask) == (dir2 & Directions.mask);
		}

		/// <summary>
		/// Parses a string into a <see cref="Directions"/> enum value.
		/// </summary>
		/// <param name="direction">The direction string to parse.</param>
		/// <returns>The parsed <see cref="Directions"/> value.</returns>
		public static UoTMovement.Directions ParseDirection(string direction)
		{
			if (Enum.TryParse(direction, true, out UoTMovement.Directions parsedDirection))
			{
				return parsedDirection;
			}

			Console.WriteLine($"Invalid direction string: {direction}");
			UoTLogger.LogErrorToFile($"Invalid direction string: {direction}");
			return UoTMovement.Directions.mask; // Default or fallback direction
		}

		/// <summary>
		/// Gets the direction from a given string input.
		/// </summary>
		/// <param name="direction">The direction string to interpret.</param>
		/// <returns>A <see cref="Directions"/> corresponding to the string.</returns>
		private static UoTMovement.Directions GetDirectionFromString(string direction)
		{
			// Normalize input: trim whitespace and convert to lowercase
			var normalizedDirection = direction.Trim().ToLowerInvariant();

			return normalizedDirection switch
			{
				"north" => UoTMovement.Directions.north,
				"south" => UoTMovement.Directions.south,
				"west" => UoTMovement.Directions.west,
				"east" => UoTMovement.Directions.east,
				"right" => UoTMovement.Directions.northeast,
				"northeast" => UoTMovement.Directions.northeast,
				"left" => UoTMovement.Directions.southwest,
				"southwest" => UoTMovement.Directions.southwest,
				"down" => UoTMovement.Directions.southeast,
				"southeast" => UoTMovement.Directions.southeast,
				"up" => UoTMovement.Directions.northwest,
				"northwest" => UoTMovement.Directions.northwest,

				_ => throw new ArgumentException($"Invalid direction string: {direction}")
			};
		}

		
		/// <summary>
		/// Checks whether the player's facing direction matches the target direction.
		/// </summary>
		/// <param name="playerDirectionString">The player's direction as a string.</param>
		/// <param name="directionToTarget">The target direction to check against.</param>
		/// <returns>True if the directions are equal; otherwise false.</returns>
		public static bool DirectionEquals(string playerDirectionString, UoTMovement.Directions directionToTarget)
		{
			var playerDirectionEnum = GetDirectionFromString(playerDirectionString);

			// Mask out irrelevant flags (like running) to focus on the main direction
			var filteredPlayerDirection = playerDirectionEnum & UoTMovement.Directions.valuemask;
			var filteredDirectionToTarget = directionToTarget & UoTMovement.Directions.valuemask;

			if (filteredPlayerDirection != filteredDirectionToTarget)
			{
				Console.WriteLine($"Player is not facing the target. Facing: {filteredPlayerDirection}, Target: {filteredDirectionToTarget}");
				// Handle mismatched direction logic here
				return false;
			}
			else
			{
				Console.WriteLine("Player is already facing the target.");
				return true;
			}
		}

		/// <summary>
		/// Determines the direction needed to move left relative to the current direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning left</returns>
		public static UoTMovement.Directions TurnLeft(UoTMovement.Directions direction)
		{
			return TurnLeftHalf(TurnLeftHalf(direction));
		}

		/// <summary>
		/// Finds the direction when turning left from the given direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning left</returns>
		public static UoTMovement.Directions TurnLeftish(UoTMovement.Directions direction)
		{
			return Directionish(TurnLeft(direction));
		}

		/// <summary>
		/// Determines the direction needed to move right relative to the current direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning right</returns>
		public static UoTMovement.Directions TurnRight(UoTMovement.Directions direction)
		{
			return TurnRightHalf(TurnRightHalf(direction));
		}

		/// <summary>
		/// Determines the direction needed to move rightish relative to the current direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning rightish</returns>
		public static UoTMovement.Directions TurnRightish(UoTMovement.Directions direction)
		{
			return Directionish(TurnRight(direction));
		}

		/// <summary>
		/// Determines the direction needed to move leftish relative to the current direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning leftish</returns>
		public static UoTMovement.Directions TurnLeftHalf(UoTMovement.Directions direction)
		{
			var newDirection = (int)direction - 1;
			if (newDirection < 0) 
				newDirection += (int)UoTMovement.Directions.valuemask;
			return (UoTMovement.Directions)newDirection;
		}

		/// <summary>
		/// Finds the direction when turning right from the given direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The direction after turning right</returns>
		public static UoTMovement.Directions TurnRightHalf(UoTMovement.Directions direction)
		{
			var newDirection = (int)direction + 1;
			if (newDirection > (int)UoTMovement.Directions.mask)
				newDirection -= (int)UoTMovement.Directions.valuemask;
			return (UoTMovement.Directions)newDirection;

		}

		/// <summary>
		/// Returns the reverse (opposite) direction to the given direction.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The opposite direction</returns>
		public static UoTMovement.Directions TurnAround(UoTMovement.Directions direction)
		{
			return direction switch
			{
				UoTMovement.Directions.north => UoTMovement.Directions.south,
				UoTMovement.Directions.south => UoTMovement.Directions.north,
				UoTMovement.Directions.east => UoTMovement.Directions.west,
				UoTMovement.Directions.west => UoTMovement.Directions.east,
				UoTMovement.Directions.northeast => UoTMovement.Directions.southwest,
				UoTMovement.Directions.southeast => UoTMovement.Directions.northwest,
				UoTMovement.Directions.southwest => UoTMovement.Directions.northeast,
				UoTMovement.Directions.northwest => UoTMovement.Directions.southeast,
				_ => UoTMovement.Directions.mask // Fallback for undefined directions
			};
		}

		/// <summary>
		/// Returns the reverse (opposite) direction to the given direction, give or take half a step.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The opposite direction</returns>
		public static UoTMovement.Directions TurnAroundish(UoTMovement.Directions direction)
		{
			return Directionish(TurnAround(direction));
		}

		/// <summary>
		/// Returns a direction, randomly between itself, 45 degrees left or right.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>Random direction</returns>
		public static UoTMovement.Directions Directionish(UoTMovement.Directions direction)
		{
			var randomResponse = UoTMath.randomGen.Next(1, 4);;
			if (randomResponse == 1) return direction;
			if (randomResponse == 2) return TurnLeftHalf(direction);
			return TurnRightHalf(direction);
		}
		private static (int x, int y) _lastDirectionishValidMovement = default; // Tracks how far you've moved since the last random generation.
		private static int _lastDirectionishValidMovementCount = 0; // Tracks how many times you've moved since the last random generation.'
		private static bool _lastDirectionishValidMovementGoRight; // Tracks how many times you've moved since the last random generation.'
		private static Directions _lastDirectionishValidDirection = Directions.mask; // Tracks how many times you've moved since the last random generation.'
		/// <summary>
		/// Returns the given direction, after minor movement validation, probable offset by 45 degrees and possible offset by 90 degrees.
		/// </summary>
		/// <param name="direction">The current direction</param>
		/// <returns>The opposite direction</returns>
		public static UoTMovement.Directions DirectionishValidMovement(UoTMovement.Directions direction)
		{
			if (Misc.Distance(_lastDirectionishValidMovement.x, _lastDirectionishValidMovement.y, 
				    Player.Position.X, Player.Position.Y) > 2)
			{
				_lastDirectionishValidMovement = (Player.Position.X, Player.Position.Y);
				if (UoTMath.randomGen.Next(1, 3) == 1 ) _lastDirectionishValidMovementGoRight = true;
				else _lastDirectionishValidMovementGoRight = false;
				_lastDirectionishValidMovementCount = 0;
			}
			_lastDirectionishValidMovementCount++;
			if (_lastDirectionishValidMovementCount is 5 or 10 or 15 or 20) Misc.Resync();
			//if (_lastDirectionishValidMovementCount > 5) Misc.SendMessage( "DirectionishValidMovement: " + _lastDirectionishValidMovementCount);
			if (!_lastDirectionishValidMovementGoRight && _lastDirectionishValidMovementCount > 15) return Directionish(TurnLeft(direction));
			if ( _lastDirectionishValidMovementGoRight && _lastDirectionishValidMovementCount > 15) return Directionish(TurnRight(direction));
			if (_lastDirectionishValidMovementCount is > 10 and < 15) return Directionish(direction);
			if (ValidMovement(direction)) return direction;
			if (!_lastDirectionishValidMovementGoRight) return Directionish(TurnLeft(direction));
			if (_lastDirectionishValidMovementGoRight) return Directionish(TurnRight(direction));
			return direction;
		}

		/// <summary>
		/// Calculates the exact coordinates after moving in a given direction.
		/// </summary>
		/// <param name="x">Initial X-coordinate.</param>
		/// <param name="y">Initial Y-coordinate.</param>
		/// <param name="direction">The direction to move in.</param>
		/// <returns>The resulting coordinates after the movement.</returns>
		public static (int, int) GetCoordinateFromDirection(int x, int y, string direction)
		{
			// Normalize input: trim spaces and convert to lowercase
			string normalizedDirection = direction.Trim().ToLowerInvariant();

			// Calculate coordinates based on the given direction
			return normalizedDirection switch
			{
				"up" => (x - 1, y - 1),
				"down" => (x + 1, y + 1),
				"left" => (x - 1, y + 1),
				"right" => (x + 1, y - 1),
				"north" => (x, y - 1),
				"south" => (x, y + 1),
				"east" => (x + 1, y),
				"west" => (x - 1, y),
				"northeast" => (x - 1, y - 1),
				"northwest" => (x + 1, y - 1),
				"southeast" => (x + 1, y + 1),
				"southwest" => (x - 1, y + 1),
				_ => (x, y) // Default case, return current position if direction is undefined
			};
		}

		/// <summary>
		/// Determines whether the given direction is diagonal.
		/// </summary>
		/// <param name="direction">The direction to check.</param>
		/// <returns>True if the direction is diagonal; otherwise false.</returns>
		public static bool IsDiagonal(Directions direction)
		{
			// Define diagonal directions
			return direction == Directions.northeast ||
			       direction == Directions.southeast ||
			       direction == Directions.southwest ||
			       direction == Directions.northwest;
		}
		/// <summary>
		/// A predefined guide for movement directions represented as coordinate offsets (dx, dy).
		/// </summary>
		/// <remarks>
		/// The guide contains coordinate pairs for the following directions:
		/// <list type="bullet">
		/// <item><term>Right</term> <description>(1, 0)</description></item>
		/// <item><term>Up</term> <description>(0, 1)</description></item>
		/// <item><term>Diagonal right-up</term> <description>(1, 1)</description></item>
		/// <item><term>Diagonal left-down</term> <description>(-1, -1)</description></item>
		/// <item><term>Left</term> <description>(-1, 0)</description></item>
		/// <item><term>Down</term> <description>(0, -1)</description></item>
		/// <item><term>Diagonal left-up</term> <description>(-1, 1)</description></item>
		/// <item><term>Diagonal right-down</term> <description>(1, -1)</description></item>
		/// </list>
		/// </remarks>

		public static readonly (int dx, int dy)[] DirectionsGuide = new (int dx, int dy)[]
		{
			(1, 0),   // Right
			(0, 1),   // Up
			(1, 1),   // Diagonal right-up
			(-1, -1), // Diagonal left-down
			(-1, 0),  // Left
			(0, -1),  // Down
			(-1, 1),  // Diagonal left-up
			(1, -1)   // Diagonal right-down
		};
		public static (int, int) GetCoordinateFromDirection(int x, int y, UoTMovement.Directions direction)
		{
			// Calculate coordinates based on the given direction
			return direction switch
			{
				UoTMovement.Directions.up => (x - 1, y - 1),   // Northeast
				UoTMovement.Directions.down => (x + 1, y + 1), // Southeast
				UoTMovement.Directions.left => (x - 1, y + 1), // Southwest
				UoTMovement.Directions.right => (x + 1, y - 1),//Northwest
				UoTMovement.Directions.north => (x, y - 1),
				UoTMovement.Directions.south => (x, y + 1),
				UoTMovement.Directions.east => (x + 1, y),
				UoTMovement.Directions.west => (x - 1, y),
				_ => (x, y) // Default case, return current position if direction is undefined
			};
		}

		/// <summary>
		/// Determines the cardinal or intercardinal direction from the player's current position to the target coordinates.
		/// </summary>
		/// <param name="targetX">The X-coordinate of the target.</param>
		/// <param name="targetY">The Y-coordinate of the target.</param>
		/// <returns>
		/// A <see cref="UoTMovement.Directions"/> enum representing the direction from the player's position
		/// to the target position (e.g., north, south, northeast, etc.).
		/// If the direction cannot be determined, returns <see cref="UoTMovement.Directions.mask"/> as a fail-safe.
		/// </returns>
		public static UoTMovement.Directions GetDirectionToTarget(int targetX, int targetY)
		{
			// Calculate the difference between the target and player position
			int dx = targetX - Player.Position.X;
			int dy = targetY - Player.Position.Y;

			// Determine the direction based on dx and dy
			if (dx == 0 && dy < 0) return UoTMovement.Directions.north;
			if (dx == 0 && dy > 0) return UoTMovement.Directions.south;
			if (dx > 0 && dy == 0) return UoTMovement.Directions.east;
			if (dx < 0 && dy == 0) return UoTMovement.Directions.west;

			if (dx > 0 && dy < 0) return UoTMovement.Directions.northeast;
			if (dx > 0 && dy > 0) return UoTMovement.Directions.southeast;
			if (dx < 0 && dy < 0) return UoTMovement.Directions.northwest;
			if (dx < 0 && dy > 0) return UoTMovement.Directions.southwest;

			return UoTMovement.Directions.mask; // Fail-safe, return mask as undefined
		}
		/// <summary>
		/// Determines the cardinal or intercardinal direction from one coordinate to another.
		/// </summary>
		/// <param name="from">The starting coordinates as a tuple (x, y).</param>
		/// <param name="to">The target coordinates as a tuple (x, y).</param>
		/// <returns>
		/// A <see cref="UoTMovement.Directions"/> enum representing the direction from
		/// the starting point to the target point (e.g., north, south, northeast, etc.).
		/// Returns <see cref="UoTMovement.Directions.mask"/> if the direction cannot be determined.
		/// </returns>
		public static UoTMovement.Directions GetDirectionToTarget((int x,int y) from, (int x, int y) to)
		{
			// Calculate the difference between the target and player position
			int dx = to.x - from.x;
			int dy = to.y - from.y;

			// Determine the direction based on dx and dy
			if (dx == 0 && dy < 0) return UoTMovement.Directions.north;
			if (dx == 0 && dy > 0) return UoTMovement.Directions.south;
			if (dx > 0 && dy == 0) return UoTMovement.Directions.east;
			if (dx < 0 && dy == 0) return UoTMovement.Directions.west;

			if (dx > 0 && dy < 0) return UoTMovement.Directions.northeast;
			if (dx > 0 && dy > 0) return UoTMovement.Directions.southeast;
			if (dx < 0 && dy < 0) return UoTMovement.Directions.northwest;
			if (dx < 0 && dy > 0) return UoTMovement.Directions.southwest;

			return UoTMovement.Directions.mask; // Fail-safe, return mask as undefined
		}

		/// <summary>
		/// Checks if two mobiles are facing opposite directions (towards each other).
		/// </summary>
		/// <param name="m1">First mobile</param>
		/// <param name="m2">Second mobile</param>
		/// <returns>True if the mobiles are facing each other, false otherwise</returns>
		public static bool FacingEachother(Mobile m1, Mobile m2)
		{
			if (Enum.TryParse(m1.Direction, true, out UoTMovement.Directions direction1) &&
			    Enum.TryParse(m2.Direction, true, out UoTMovement.Directions direction2))
			{
				// Check if the directions are opposites
				return direction2 == TurnAround(direction1);
			}

			Console.WriteLine($"Failed to parse directions: {m1.Direction} or {m2.Direction}");
			return false;
		}

		/// <summary>
		/// Checks if mobile is facing location.
		/// </summary>
		/// <param name="mobile">Mobile location</param>
		/// <param name="facing">Direction mobile is facing</param>
		/// <param name="point">Location you are checking</param>
		/// <returns>True if the mobiles are facing each other, false otherwise</returns>
		public static bool FacingPoint((int mx, int my) mobile, UoTMovement.Directions facing, (int x,int y) point)
		{
			UoTMovement.Directions targetDirection = GetDirectionToTarget(point.x - mobile.mx, point.y - mobile.my);

			// Check if the mobile's current facing direction matches the target direction
			return facing == targetDirection;
		}
		
		
		/// <summary>
		/// Moves the player a specific number of steps.
		/// </summary>
		/// <param name="steps">Number of steps to move.</param>
		/// <param name="target">Optional target coordinates to aim at.</param>
		/// <param name="distanceFromTarget">Distance to maintain from the target (default is 1).</param>
		/// <param name="direction">The direction to move in (default is Directions.running).</param>
		/// <param name="token">Cancellation token to stop the movement prematurely.</param>
		public static void MoveSteps(int steps, (int x, int y) target = default, int distanceFromTarget = 1,
			UoTMovement.Directions direction = Directions.running,   CancellationToken token = default)
		{ 
			// Target and direction are not specified, set as player's
			if (target == default && direction == Directions.running) direction = ParseDirection(Player.Direction);
			// Target given but no directions
			if (target != default && direction == Directions.running) direction = GetDirectionToTarget(target.x, target.y);
			PlayerMoved();
			for (var i = 0; i < steps; i++)
			{
				if (token.IsCancellationRequested || IsAtTarget(target, distanceFromTarget)) return;
				MoveOneStep(DirectionishValidMovement(direction), token);
				if (HasMovedPastTarget(target, direction))
				{
					Misc.SendMessage("Did I move past the target?");
					return;
				}
				if (!PlayerMoved(500)) return;
			}
			
		}
		/// <summary>
		/// Attempts to move the player one step in the specified direction.
		/// </summary>
		/// <param name="direction">The direction to move.</param>
		/// <param name="token">Cancellation token to stop the movement prematurely.</param>
		/// <param name="sideStep">Whether this movement is a sidestep (default is false).</param>
		/// <returns>True if the movement was successful; otherwise false.</returns>
		public static bool MoveOneStep(UoTMovement.Directions direction, CancellationToken token = default, bool sideStep = false)
		{
			token.ThrowIfCancellationRequested();
			const int MovementDelay = 120;  // Delay between movement attempts
			const int DirectionChangeDelay = 200;  // Delay between direction changes
			const int MovementCheckInterval = 20;  // Internal delay for movement checks
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			try
			{
				// Ensure player is facing the target direction
				if (!DirectionEquals(Player.Direction, direction))
				{
					Player.Run(direction.ToString());
					Misc.Pause(DirectionChangeDelay); // Wait after changing direction
					UoTLogger.LogErrorToFile($"Turned to face direction: {direction}");
				}

				PlayerMoved();
				var start = Player.Position;
				// Attempt to move in the target direction
				Player.Run(direction.ToString());
				Misc.Pause(MovementDelay);

				// Continuously check if the player is moving
				while (PlayerMoved(MovementCheckInterval))
				{
					Misc.Pause(5);
				}
				Misc.Pause(5);

				return !(start.X == Player.Position.X && start.Y == Player.Position.Y);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
		/// <summary>
		/// Static storage for managing points of interest (POIs) represented by 3D coordinates (X, Y, Z).
		/// </summary>
		private static readonly List<(int X, int Y, int Z)> PointsOfInterest = new List<(int X, int Y, int Z)>();

		/// <summary>
		/// Temporarily stores points of interest for pausing, reapplication or reordering.
		/// </summary>
		private static List<(int X, int Y, int Z)> _savedPoints = new List<(int X, int Y, int Z)>();

		/// <summary>
		/// Enum representing different reapplication modes for points of interest.
		/// </summary>
		public enum PoiMode
		{
			Front,
			End,
			Ordered,
			Nearest
		}
		/// <summary>
		/// Transfers points of interest from the main storage list into temporary storage (_savedPoints)
		/// while optionally processing them based on the specified reapplication mode.
		/// </summary>
		/// <param name="mode">The mode in which points should be processed before moving to storage.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid reapplication mode is provided.</exception>
		public static void MovePoiToStorage(PoiMode mode)
		{
			if (_savedPoints == null)
			{
				// Initialize _savedPoints with PointsOfInterest
				_savedPoints = new List<(int X, int Y, int Z)>(PointsOfInterest);

				// If Ordered, sort the initialized list
				if (mode == PoiMode.Ordered)
				{
					_savedPoints = _savedPoints
						.OrderBy(point => point.X)
						.ThenBy(point => point.Y)
						.ToList();
				}
				// If Nearest, sort the PointsOfInterest by distance and initialize _savedPoints accordingly
				else if (mode == PoiMode.Nearest)
				{
					_savedPoints = PointsOfInterest
						.OrderBy(poi => Misc.Distance(Player.Position.X, Player.Position.Y, poi.X, poi.Y))
						.ToList();
				}

			}
			else
			{
				// Handle based on the mode when _savedPoints is not null
				switch (mode)
				{
					case PoiMode.Front:
						// Add points to the front
						_savedPoints.InsertRange(0, PointsOfInterest);
						break;

					case PoiMode.End:
						// Add points to the end
						_savedPoints.AddRange(PointsOfInterest);
						break;

					case PoiMode.Ordered:
						// Add points and then sort the combined list
						_savedPoints.AddRange(PointsOfInterest);
						_savedPoints = _savedPoints
							.OrderBy(point => point.X)
							.ThenBy(point => point.Y)
							.ToList();
						break;
					
					case PoiMode.Nearest:
						// Sort PointsOfInterest by distance and add to _savedPoints
						_savedPoints.AddRange(
							PointsOfInterest
								.OrderBy(poi => Misc.Distance(Player.Position.X, Player.Position.Y, poi.X, poi.Y))
						);
						break;


					default:
						throw new ArgumentOutOfRangeException(nameof(mode), "Invalid storage mode.");
				}
			}

			// Clear the original PointsOfInterest list
			PointsOfInterest.Clear();
		}

		/// <summary>
		/// Reapplies points of interest from the temporary storage (_savedPoints) back to the main list.
		/// </summary>
		/// <param name="mode">The reapplication mode that determines how points are reintegrated.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid reapplication mode is provided.</exception>
		public static void ReapplyPointsFromSharedStorage(PoiMode mode)
		{
			if (_savedPoints == null || _savedPoints.Count == 0)
				return; // Nothing to reapply if storage is empty

			switch (mode)
			{
				case PoiMode.Front: // Add points to the front of PointsOfInterest
					PointsOfInterest.InsertRange(0, _savedPoints);
					break;

				case PoiMode.End: // Add points to the end of PointsOfInterest
					PointsOfInterest.AddRange(_savedPoints);
					break;

				case PoiMode.Ordered: // Insert points into PointsOfInterest in order
					foreach (var point in _savedPoints)
					{
						AddPointOfInterestInOrder(point); // Assuming InsertPointInOrder is defined
					}
					break;

				case PoiMode.Nearest: // Combine and sort both lists by distance to Player.Position
					{
						// Combine PointsOfInterest and _savedPoints and sort by distance
						var combinedPoints = PointsOfInterest
							.Concat(_savedPoints)
							.OrderBy(point => Misc.Distance(Player.Position.X, Player.Position.Y, point.X, point.Y))
							.ToList();

						// Clear PointsOfInterest and add back the sorted points
						PointsOfInterest.Clear();
						PointsOfInterest.AddRange(combinedPoints);
					}
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(mode), "Invalid reapply mode.");
			}

			// Clear saved points after reapplying
			_savedPoints.Clear();
		}
		
		/// <summary>
		/// Adds a point of interest to the main list if it's not already present.
		/// </summary>
		/// <param name="x">The X coordinate of the point.</param>
		/// <param name="y">The Y coordinate of the point.</param>
		/// <param name="z">The Z coordinate of the point.</param>
		/// <param name="addToEnd">If true, adds the point to the end of the list. Otherwise, adds it to the front.</param>
		public static void AddPointOfInterest((int x, int y, int z) poi, PoiMode mode = PoiMode.End)
		{

			switch (mode)
			{
				case PoiMode.Front: // Add points to the front of PointsOfInterest
					PointsOfInterest.Prepend(poi);
					break;

				case PoiMode.End: // Add points to the end of PointsOfInterest
					PointsOfInterest.Add(poi);
					break;

				case PoiMode.Ordered: // Insert points into PointsOfInterest in order
					AddPointOfInterestInOrder(poi); // Assuming InsertPointInOrder is defined
					break;

				case PoiMode.Nearest: // Combine and sort both lists by distance to Player.Position
				{
					if (!PointsOfInterest.Contains(poi))
					{
						var indexToInsert = PointsOfInterest
							.Select((p, index) => new { Index = index, Distance = Misc.Distance(Player.Position.X, Player.Position.Y, p.X, p.Y) })
							.OrderBy(item => item.Distance)
							.Select(item => item.Index)
							.FirstOrDefault();

						PointsOfInterest.Insert(indexToInsert, poi);
					}
				}
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(mode), "Invalid reapply mode.");
			}

		}
		
		/// <summary>
		/// Inserts a point of interest into the main list in a sorted order based on distance.
		/// </summary>
		/// <param name="newPoint">The point to insert, represented by its 3D coordinates.</param>
		private static void AddPointOfInterestInOrder((int X, int Y, int Z) newPoint)
		{
			if (PointsOfInterest.Count == 0)
			{
				// If the list is empty, simply add the new point
				PointsOfInterest.Add(newPoint);
				return;
			}

			// Find the appropriate index for the new point by comparing distances
			var indexToInsert = PointsOfInterest
				.Select((point, index) => new { Index = index, Distance = Misc.Distance(newPoint.X, newPoint.Y, point.X, point.Y) })
				.OrderBy(item => item.Distance)
				.Select(item => item.Index)
				.FirstOrDefault();

			// Insert the new point at the determined index
			PointsOfInterest.Insert(indexToInsert, newPoint);
		}
		
		/// <summary>
		/// Clears all points of interest from the main list.
		/// </summary>
		// Helper method to clear POIs
		public static void ClearPointOfInterest()
		{
			PointsOfInterest.Clear();
		}
		/// <summary>
		/// Retrieves and removes the first point of interest from the main list, if available.
		/// </summary>
		/// <returns>The first point of interest as a tuple (X, Y, Z), or null if the list is empty.</returns>
		private static (int X, int Y, int Z)? DequeuePoi()
		{
			if (PointsOfInterest == null || PointsOfInterest.Count == 0)
				return null; // List is empty, return null

			var firstPoi = PointsOfInterest[0]; // Get the first element
			PointsOfInterest.RemoveAt(0); // Remove the first element
			return firstPoi; // Return the removed element
		}
		/// <summary>
		/// Retrieves and removes the last point of interest from the main list, if available.
		/// </summary>
		/// <returns>The last point of interest as a tuple (X, Y, Z), or null if the list is empty.</returns>
		private static (int X, int Y, int Z)? PopPoi()
		{
			if (PointsOfInterest.Count == 0)
				return null; // List is empty, return null

			var lastPoi = PointsOfInterest[PointsOfInterest.Count - 1]; // Get the last element
			PointsOfInterest.RemoveAt(PointsOfInterest.Count - 1); // Remove the last element
			return lastPoi; // Return the removed element
		}
		/// <summary>
		/// Gets the nearest point of interest to the player and removes it from the list.
		/// </summary>
		/// <returns>
		/// The nearest point of interest as a tuple (X, Y, Z),
		/// or the player's current position if no POIs are available.
		/// </returns>
		private static (int X, int Y, int Z)? GetNearestPoi()
		{
			if (PointsOfInterest.Count == 0) return (Player.Position.X, Player.Position.Y, Player.Position.Z);
        
			var nearestPoi = PointsOfInterest
				.OrderBy(poi => Misc.Distance(Player.Position.X, Player.Position.Y, poi.X, poi.Y))
				.FirstOrDefault();
			if (nearestPoi != default)
			{
				// Remove all points matching these coordinates
				PointsOfInterest.RemoveAll(poi => 
					poi.X == nearestPoi.X && 
					poi.Y == nearestPoi.Y);
			}
			return nearestPoi;
		}

		/// <summary>
		/// Fallback method to move the player toward the target coordinates using Player.Run and Directions.
		/// </summary>
		/// <param name="x">Target X coordinate</param>
		/// <param name="y">Target Y coordinate</param>
		/// <param name="z">Target Z coordinate</param>
		/// <param name="range">Distance from target</param>
		/// <param name="token">Cancellation token</param>
		/// <param name="retryLimit">Retry limit to prevent infinite loops</param>
		public static async Task RunTowards(int x, int y, int z, int range = 1,  int retryLimit = 3, bool timeout = true, PoiMode mode = PoiMode.Nearest, CancellationToken token = default)
		{
			var retries = 0;
			const int MovementDelay = 100;  // Delay between movement attempts
			const int DirectionChangeDelay = 200;  // Delay between direction changes
			const int StuckCheckThreshold = 5000;  // Threshold to consider player stuck
			const int MovementCheckInterval = 20;  // Internal delay for movement checks
			const int MovementTimeout = 5;  // Seconds before poi times out
			const int MovementMaxRange = 16;  // Internal delay for movement checks

			// Create a HashSet to store visited POIs temporarily
			var visitedPois = new HashSet<(int X, int Y)>();
			try
			{
				//if (mode == PoiMode.Nearest) 
				PointsOfInterest.Add((x, y, z));
				// Continue while there are points of interest
				while (PointsOfInterest.Count > 0)
				{
					token.ThrowIfCancellationRequested();
					//Misc.SendMessage("Poi count:" + PointsOfInterest.Count);
					var nextPoi = GetNearestPoi();
					//var nextPoi = GetFirstPoi;
					//var nextPoi = GetLastPoi;
					if (nextPoi == null) return;
					if (visitedPois.Contains((nextPoi.Value.X, nextPoi.Value.Y)))
					{
						PointsOfInterest.RemoveAll(poi => poi == nextPoi.Value);
						continue;
					}
					//Misc.SendMessage("Heading to :" + nextPoi.Value.X + ", " + nextPoi.Value.Y);
					UoTLogger.LogErrorToFile("Heading to :" + nextPoi.Value.X + ", " + nextPoi.Value.Y);
					var attemptStartTime = DateTime.Now;
					while (!token.IsCancellationRequested)
					{
						token.ThrowIfCancellationRequested();
						var currentDistance = Misc.Distance(Player.Position.X, Player.Position.Y, nextPoi.Value.X, nextPoi.Value.Y);
						if (currentDistance <= range || currentDistance > MovementMaxRange || timeout && (DateTime.Now - attemptStartTime).TotalSeconds > MovementTimeout)
						{
							//Misc.SendMessage("Point of interest either timedout or succeded.");
							//UoTLogger.LogErrorToFile("Point of interest either timedout or succeded.");
							PointsOfInterest.RemoveAll(poi => poi == nextPoi.Value);
							visitedPois.Add((nextPoi.Value.X, nextPoi.Value.Y));
							break;
						}
						//Misc.SendMessage("Distance :" + currentDistance);
						var directionToTarget = GetDirectionToTarget(nextPoi.Value.X, nextPoi.Value.Y);
						directionToTarget = DirectionishValidMovement(directionToTarget);
						MoveOneStep(directionToTarget, token);
						if (!PlayerMoved())
						{
							//try pathto
							//UoTLogger.LogErrorToFile($"Player appears stuck heading to POI. Attempting AntiStuck... (Retry {retries}/{retryLimit})");
							//AntiStuck((nextPoi.Value.X, nextPoi.Value.Y), range, token);
						}
					}
				}
			}

			catch (Exception e)
			{
				UoTLogger.LogErrorToFile($"Error running towards target: {e.Message}");
			}

		}
	}
}