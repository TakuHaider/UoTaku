namespace RazorEnhanced
{
	public class Scrap
	{
		
/*
var journal = new Journal();
//journal.SearchByName("", Player.Name);
string entryCache = "";

while (!Player.IsGhost)
{
	var entryNew = journal.GetLineText("");
	if (entryCache != entryNew)
	{
		Misc.SendMessage(entryNew);
		entryCache = entryNew;
	}

	Misc.Pause(20);
}*/

  
                        /*
                        if (gumpData.gumpLayout != null && gumpData.gumpText != null)
{
    UoTLogger.LogErrorToFile("Combined text with buttons");

    var layout = gumpData.gumpLayout;

    // Fixed button mappings for specific button IDs (e.g., static button texts)
    var buttonTextMap = new Dictionary<int, string>
    {
        {0, "EXIT"},
        //{84, "CANCEL MAKE"},
        //{42, "REPAIR ITEM"},
        //{49, "MARK ITEM"},
        //{63, "ENHANCE ITEM"},
        //{77, "NON QUEST ITEM"},
        //{21, "MAKE LAST"}
    };

    // Locate "LAST TEN" in the gumpText list to dynamically determine the starting index for dynamic texts
    int lastTenIndex = gumpData.gumpText.IndexOf("LAST TEN");
    if (lastTenIndex == -1)
    {
        UoTLogger.LogErrorToFile("Error: 'LAST TEN' not found in gumpText!");
        return;
    }

    // Dynamic text starts after "LAST TEN"
    var dynamicTextStartIndex = lastTenIndex;

    // LEFT COLUMN BUTTONS: Positions at X = 15, excluding material selection button (Button ID 7)
    var leftColumnButtons = Regex.Matches(layout, @"button 15 (\d+) 4005 4007 1 0 (\d+)")
        .Cast<Match>()
        //Where(m => int.Parse(m.Groups[2].Value) != 7) // Exclude Button ID 7
        .Select(m => new
        {
            Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
            ButtonId = int.Parse(m.Groups[2].Value) // Button ID
        })
        .Where(m => m.Y <= 300 )
        .OrderBy(b => b.Y)
        .ToList();

    // RIGHT COLUMN BUTTONS: Positions at X = 220
    var rightColumnButtons = Regex.Matches(layout, @"button 220 (\d+) 4005 4007 1 0 (\d+)")
        .Cast<Match>()
        .Select(m => new
        {
            Y = int.Parse(m.Groups[1].Value), // Button's Y-coordinate
            ButtonId = int.Parse(m.Groups[2].Value) // Button ID
        })
        .Where(m => m.Y <= 300 )
        .OrderBy(b => b.Y)
        .ToList();

    // Handle the special "Material Type" button (Button ID: 7)
    var materialTypeButton = Regex.Match(layout, @"button 15 (\d+) 4005 4007 1 0 7");
    if (materialTypeButton.Success)
    {
        var materialButtonY = int.Parse(materialTypeButton.Groups[1].Value); // Y-coordinate of the material button
        string materialText =  gumpData.gumpText[dynamicTextStartIndex-1];
        UoTLogger.LogErrorToFile($"Material Type Button - Y: {materialButtonY}, Button ID: 7, Text: {materialText}");
    }

    // Collect remaining dynamic texts, starting after the material text
    var dynamicTextMap = gumpData.gumpText
        .Skip(dynamicTextStartIndex)
        .ToList();

    // Use dynamicTextIndex for assigning texts dynamically to other buttons
    int dynamicTextIndex = 0;

    // Process each LEFT COLUMN button
    foreach (var button in leftColumnButtons)
    {
        string text;
        if (buttonTextMap.ContainsKey(button.ButtonId)) // Check if it's a static button
        {
            text = buttonTextMap[button.ButtonId];
        }
        else
        {
            // Dynamically assign available text to left-column buttons
            text = dynamicTextIndex < dynamicTextMap.Count ? dynamicTextMap[dynamicTextIndex] : "Unknown";
            dynamicTextIndex++; // Move to the next text
        }

        UoTLogger.LogErrorToFile($"Left Column - Y: {button.Y}, Button ID: {button.ButtonId}, Text: {text}");
    }

    // Process each RIGHT COLUMN button
    foreach (var button in rightColumnButtons)
    {
        string text;
        if (buttonTextMap.ContainsKey(button.ButtonId)) // Check if it's a static button
        {
            text = buttonTextMap[button.ButtonId];
        }
        else
        {
            // Dynamically assign available text to right-column buttons
            text = dynamicTextIndex < dynamicTextMap.Count ? dynamicTextMap[dynamicTextIndex] : "Unknown";
            dynamicTextIndex++; // Move to the next text
        }

        UoTLogger.LogErrorToFile($"Right Column - Y: {button.Y}, Button ID: {button.ButtonId}, Text: {text}");
    }
}
*/
          
	}
}