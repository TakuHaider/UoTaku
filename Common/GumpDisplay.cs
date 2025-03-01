using System;

namespace RazorEnhanced
{
	internal class GumpDisplay
	{
		// Make sure you do not use any of UO's defaults
		private const int gumpId = 84765431;

		public void Run()
		{
			var time = UnixTime();
			Misc.SendMessage("> " + time, 201);

			this.CreateMenuInstance();

			while (true)
			{
				// Do not crash the editor
				Misc.Pause(1);

				this.EventListener();
			}
		}

		public static long UnixTime()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public void CreateMenuInstance()
		{
			var time = UnixTime();

			var gump = Gumps.CreateGump(true, true, true, false);
			gump.gumpId = gumpId;
			gump.serial = (uint)Player.Serial;
			Gumps.AddPage(ref gump, 0);
			Gumps.AddBackground(ref gump, 0, 0, 220, 100, 9200);
			Gumps.AddLabel(ref gump, 10, 10, 0, time.ToString());

			Gumps.AddButton(ref gump, 5, 5, 2242, 2242, 22, 1, 0);
			Gumps.SendGump(gump.gumpId, gump.serial, 0, 0, gump.gumpDefinition, gump.gumpStrings);

			// Do not use WaitForGump() as it is a blocking event
			//Gumps.WaitForGump(gumpId, 1000);
		}

		public void EventListener()
		{
			var gumpData = Gumps.GetGumpData(gumpId);
			if (gumpData.buttonid == 22)
			{
				// Make sure you clean up the created menu
				Gumps.CloseGump(gumpId);
				// Redraw your next menu if any
				this.CreateMenuInstance();
				Misc.SendMessage("> OnButtonEvent()", 201);
			}
		}
	}
}