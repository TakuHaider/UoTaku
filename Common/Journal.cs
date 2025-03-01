namespace RazorEnhanced
{
	public class UoTJournal
	{
		
		
		/// Sources
		///     Regular
		///     System
		///     Emote
		///     Label
		///     Focus
		///     Whisper
		///     Yell
		///     Spell
		///     Guild
		///     Alliance
		///     Party
		///     Encoded
		///     Special
		
		
                /*
                foreach (var line in recentEnties)
                {
	                var text = line.Text;
	                if (text.Contains("Mining at: ("))
	                {
		                // Remove "Mining at: (" from the start and ")" from the end
		                var coordinatesText = text.Replace("Mining at: (", "").TrimEnd(')');
    
		                // Split the remaining string by commas
		                var coordinates = coordinatesText.Split(',');
    
		                // Parse each coordinate, assuming format is "X, Y, Z"
		                if (coordinates.Length == 3)
		                {
			                int x = int.Parse(coordinates[0].Trim());
			                int y = int.Parse(coordinates[1].Trim());
			                int z = int.Parse(coordinates[2].Trim());
        
			                _lastMiningLocation = (x, y, z);
		                }

	                }
	                if (line.Text.Contains("You can't mine there.") ||
	                    line.Text.Contains("Target cannot be seen.") ||
	                    line.Text.Contains("is no metal here to mine."))
	                {
		                UoTSystem.CheckTimer(0, "", "Keep looking", UoTSystem.MessageOutput.Player, 63, 2900);
		                UpdateLastMinedTime(_lastMiningLocation);
		                _lastMiningLocation = default;
		                return false;
	                }
	                
	                // (Player.Position.X, Player.Position.Y, Player.Position.Z);
	                //"You have moved too far away to continue mining") ||
	                if (line.Text.Contains("ore and put it in your backpack") ||
	                    line.Text.Contains("you loosen some rocks but fail to find any usable ore")
	                   )
	                {
		                UoTSystem.CheckTimer(0, "", "Found ore", UoTSystem.MessageOutput.Player, 33, 2900);
		                UoTasks.StopTask("Pathing");
		                FoundNode(_lastMiningLocation);
		                _lastMiningLocation = default;
		                return true;
	                }
                }*/
	}
}