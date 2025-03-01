using System.Collections.Generic;
using System.Threading;

namespace RazorEnhanced
{
	public partial class UoTMovement
	{
		private static bool IsAtTarget((int x, int y) target, int range = 1) =>
			Misc.Distance(Player.Position.X, Player.Position.Y, target.x, target.y) <= range;
		private static HashSet<(int, int)> _visitedPositions = new(); 

		public static void AntiStuck((int x, int y) target, int range = 1, CancellationToken token = default)
		{
			if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
			var stuck = (Player.Position.X, Player.Position.Y); // Save stuck location
			if (!_visitedPositions.Add(stuck))
			{
				Misc.Pause(150);
				Misc.Resync();
				Misc.Pause(150);
				return;
			}
			var retries = 2; // Number of maximum attempts per stuck detection\
			Player.HeadMessage(33, "AntiStuck Moving Right");
			Misc.SendMessage("AntiStuck Moving Right");
			MoveAroundLocationRight(target, stuck, 2, token);
			var distanceRight = Misc.Distance(Player.Position.X, Player.Position.Y, target.x, target.y);
			MoveSteps(4, target, 1, GetDirectionToTarget(target.x, target.y), token);
			if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
			UoTLogger.LogErrorToFile("AntiStuck: Testing range"); // adjust ranges and measure distance improvement
			Player.HeadMessage(33, "AntiStuck Moving Left");
			Misc.SendMessage("AntiStuck Moving Left");
			MoveAroundLocationLeft(target, stuck, 2, token);
			var distanceLeft = Misc.Distance(Player.Position.X, Player.Position.Y, target.x, target.y);
			MoveSteps(4, target, 1, GetDirectionToTarget(target.x, target.y), token);
			if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
			var goRight = distanceRight < distanceLeft;
			for (var attempt = 0; attempt < retries; attempt++)
			{
				if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
				Misc.Pause(150);
				Misc.Resync();
				Misc.Pause(650);
				UoTLogger.LogErrorToFile("AntiStuck: looping right - " + goRight);
				if (goRight) Misc.SendMessage( "AntiStuck Moving Right");
				else Misc.SendMessage("AntiStuck Moving Left");
				if (goRight) MoveAroundLocationRight(target, default, attempt * 2 , token);
				else MoveAroundLocationLeft(target, default, attempt * 2, token);
				if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
				MoveSteps(attempt*2+5, target, 1, GetDirectionToTarget(target.x, target.y), token);
				if (token.IsCancellationRequested || IsAtTarget(target, range)) return;
				UoTLogger.LogErrorToFile($"AntiStuck attempt {attempt + 1} failed. Retrying...");
			}
			UoTLogger.LogErrorToFile("AntiStuck attempts exhausted.");
		}

		private static void MoveAroundLocationRight((int x, int y) target, (int x, int y) stuck, int distance = 1, CancellationToken token = default)
		{
			if (token.IsCancellationRequested) return;
			try
			{
				if (IsAtTarget(target, distance)) return; 
				MoveSteps(distance, default, 1, DirectionishValidMovement(TurnAround(GetDirectionToTarget(target.x, target.y))), token);
				Misc.SendMessage("Turning around");
				if (IsAtTarget(target, distance)) return;
				MoveSteps(distance, default, 1, DirectionishValidMovement(TurnRight(GetDirectionToTarget(target.x, target.y))), token);
				Misc.SendMessage("Turning Right");
				if (IsAtTarget(target, distance)) return;
				MoveSteps(distance, default, 1, GetDirectionToTarget(target.x, target.y), token);
				Misc.SendMessage("Moving Past");
			}
			finally
			{
				Misc.Pause(150);
				Misc.Resync();
				Misc.Pause(650);
			}
		}

		private static void MoveAroundLocationLeft((int x, int y) target, (int x, int y) stuck, int distance = 1, 
			CancellationToken token = default)
		{
			if (token.IsCancellationRequested || (UoTasks.IsCancellationRequested("Pathing")))
				return;
			try
			{
				if (IsAtTarget(target, distance)) return; 
				MoveSteps(distance, default, 1, TurnAround(GetDirectionToTarget(target.x, target.y)), token);
				Misc.SendMessage("Turning Around");
				if (IsAtTarget(target, distance)) return; 
				MoveSteps(distance, default, 1, TurnLeft(GetDirectionToTarget(target.x, target.y)), token);
				Misc.SendMessage("Turning Left");
				if (IsAtTarget(target, distance)) return; 
				MoveSteps(distance, default, 1, GetDirectionToTarget(target.x, target.y), token);
				Misc.SendMessage("Moving Past");
			}
			finally
			{
				Misc.Pause(150);
				Misc.Resync();
				Misc.Pause(650);
			}
		}
		public static bool HasMovedPastTarget((int x, int y) target, UoTMovement.Directions direction, int tolerance = 0)
		{
			var playerX = Player.Position.X;
			var playerY = Player.Position.Y;

			switch (direction)
			{
				case UoTMovement.Directions.north:
					return playerY < (target.y + tolerance);  // Consistent: earlier trigger
				case UoTMovement.Directions.south:
					return playerY > (target.y - tolerance);  // Consistent: earlier trigger
				case UoTMovement.Directions.east:
					return playerX > (target.x - tolerance);  // Consistent: earlier trigger
				case UoTMovement.Directions.west:
					return playerX < (target.x + tolerance);  // Consistent: earlier trigger
				case UoTMovement.Directions.up:    // Northeast
					return playerX > (target.x - tolerance) && playerY < (target.y + tolerance);
				case UoTMovement.Directions.down:  // Southeast
					return playerX > (target.x - tolerance) && playerY > (target.y - tolerance);
				case UoTMovement.Directions.left:  // Northwest
					return playerX < (target.x + tolerance) && playerY < (target.y + tolerance);
				case UoTMovement.Directions.right: // Southwest
					return playerX < (target.x + tolerance) && playerY > (target.y - tolerance);
				default:
					return false;
			}
		}
	}
}