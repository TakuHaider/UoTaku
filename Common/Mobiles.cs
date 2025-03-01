using System.Collections.Generic;
using System.Linq;


namespace RazorEnhanced
{
	public static class UoTMobiles
	{

		//pkFilter.IsHuman = True
		//pkFilter.IsGhost = False
		//pkFilter.Friend = False
		//pkFilter.Notorieties = List[Byte](bytes([ 6 ]))


		//Notorieties
		// Supports .Add() and .AddRange()
		//     1: blue, innocent
		//     2: green, friend
		//     3: gray, neutral
		//     4: gray, criminal
		//     5: orange, enemy
		//     6: red, hostile 
		//     6: yellow, invulnerable

		public static List<byte> EnemyNotorieties = new List<byte> { 3, 4, 5, 6 };

		public static Mobiles.Filter enemyNotorietiesFilterWithBunnies = new Mobiles.Filter
		{
			Notorieties = EnemyNotorieties,
			Friend = 0,
			CheckLineOfSight = true,
			RangeMax = 15
		};

		public static Mobiles.Filter enemyNotorietiesFilter = new Mobiles.Filter
		{
			Notorieties = EnemyNotorieties,
			Friend = 0,
			CheckLineOfSight = true,
			RangeMax = 15,
			Warmode = 1
		};

		public static List<Mobile> FindEnemiesEvenBunnies()
		{
			var bunnies = Mobiles.ApplyFilter(enemyNotorietiesFilterWithBunnies);
			return bunnies ?? new List<Mobile>();
		}

		public static List<Mobile> FindEnemies()
		{
			var mobs = Mobiles.ApplyFilter(enemyNotorietiesFilter);
			return mobs ?? new List<Mobile>();
		}

		public static Mobiles.Filter animalNotorietiesFilter = new Mobiles.Filter
		{
			//Bodies
			//Graphics
			//Hues
			//ZLevelMin
			//ZLevelMax
			//Bodies = GetAnimalIDsAtOrOverTamingDifficulty( minimumTamingDifficulty ),
			//Name = "release",
			Notorieties = new List<byte> { 3 },
			RangeMax = 15,
			CheckLineOfSight = true,
			IsHuman = 0,
			IsGhost = 0,
			Friend = 0,
			CheckIgnoreObject = true
		};

		public static Mobiles.Filter petNotorietiesFilter = new Mobiles.Filter
		{
			Notorieties = new List<byte>{1,2},
			RangeMax = 15,
			CheckLineOfSight = true,
			IsHuman = 0,
			IsGhost = 0,
			CheckIgnoreObject = true
		};

		public static void IsMobThere()
		{
//just normal mob scan.
			
		}
		public static void ReleaseAll()
		{
			var mobs = Mobiles.ApplyFilter(petNotorietiesFilter);
			foreach (var mob in mobs)
			{
				Mobiles.WaitForProps(mob,1000);
				//var sumProp = mob.Properties.FirstOrDefault(p => p.Number == 502006);
				Misc.SendMessage("Opening release gump", 33);
				Misc.WaitForContext(mob, 500);
				Misc.Pause(2000);
				Misc.ContextReply(mob, 9);
				Misc.Pause(2000);
				Misc.SendMessage("Attempting to release " + mob.Name, 33);
				Gumps.SendAction(0x5f473502, 2);
				Misc.Pause(2000);
				if (Gumps.HasGump(0x5f473502))
				{
					Misc.SendMessage("Closing release gump", 33);
					Gumps.CloseGump(0x5f473502);
					Misc.Pause(2000);
				}
				Misc.Pause(2000);
			}
		}
	
	

	public static List<Mobile> CombineMobileLists(List<Mobile> list1, List<Mobile> list2)
        {
            if((list1 == null || list1.Count == 0) && list2 != null)
                return list2;
            if((list2 == null || list2.Count == 0) && list1 != null)
                return list1;
            return list1 != null ? list1.Union(list2).ToList() : new List<Mobile>();
        }
        public static List<Mobile> FindPackAnimals()
        {
	        Misc.Pause(250);
            var packFilter = new Mobiles.Filter
            {
                Enabled = true,
                // only look for pack animals flagged green
                Notorieties = new List<byte>{2},
                RangeMax = 5,
                Bodies = new List<int>(UoTLists.PackAnimals),
                CheckIgnoreObject = true,
                CheckLineOfSight = true
            };
            var packList = Mobiles.ApplyFilter(packFilter);
            return packList ?? new List<Mobile>();
        }

        public static Mobile FindPackAnimal()
        {
            return Mobiles.Select(FindPackAnimals(), "nearest");
        }
        
        public static Mobile FindSmeltingBeetle()
        {
	        var mobileFilter = new Mobiles.Filter()
	        {   
		        Enabled = true,
		        RangeMax = 5,
		        Bodies = new List<int>{0x00A9},
		        CheckIgnoreObject = true,
		        CheckLineOfSight = true
	        };
	        var b = Mobiles.ApplyFilter(mobileFilter);
	        if (b == null || !b.Any()) return null;
	        return b[0];
        }
        public static Mobile FindBeetle()
        {
            var mobileFilter = new Mobiles.Filter()
            {   
                Enabled = true,
                RangeMax = 5,
                Bodies = new List<int>{0x0317},
                CheckIgnoreObject = true,
                CheckLineOfSight = true
            };
            var b = Mobiles.ApplyFilter(mobileFilter);
            if (b == null || !b.Any()) return null;
            return b[0];
        }
        public static List<int> FindMatchedPair(List<(int, int)> tuples, int targetElement)
        {
            // It's ok for targetElement to be null
            if (tuples == null || tuples.Count == 0) return new List<int>();
            var pairedMobs = new List<int>();
            var count = tuples.Count;
            for (var i = 0; i < count; i++)
            {
                if (tuples[i].Item1==targetElement)
                {
                    pairedMobs.Add(tuples[i].Item2); // Retrieve the paired element
                }
                else if (tuples[i].Item2==targetElement)
                {
                    pairedMobs.Add(tuples[i].Item1); // Retrieve the paired element
                }
            }
            return pairedMobs;
        }
        public static List<int> FindMatchedPairAndRemove(ref List<(int, int)> tuples, int targetElement)
        {
            // It's ok for targetElement to be null
            if (tuples == null || tuples.Count == 0) return new List<int>();
            var pairedMobs = new List<int>();
            var removeTuples = new List<int>();
            var count = tuples.Count;
            for (var i = 0; i < count; i++)
            {
                if (tuples[i].Item1 == 0 && tuples[i].Item2 == 0)
                {
                    removeTuples.Add(i); // Remove empty tuple
                }
                if (tuples[i].Item1==targetElement)
                {
                    removeTuples.Add(i); // Remove matching tuple
                    pairedMobs.Add(tuples[i].Item2); // Retrieve the paired element
                }
                if (tuples[i].Item2==targetElement)
                {
                    removeTuples.Add(i);; // Remove matching tuple
                    pairedMobs.Add(tuples[i].Item1); // Retrieve the paired element
                }
                if (tuples[i].Item1 == targetElement && tuples[i].Item2 == targetElement)
                {
                    removeTuples.Add(i);; // Remove glitched tuple
                }
            }
            //trim tuples
            tuples = tuples.Where((mob, index) => !removeTuples.Contains(index)).ToList();
            return pairedMobs;
        }
        
        public static List<Mobile> FindMatchedPair(List<(Mobile, Mobile)> tuples, Mobile targetElement)
        {
            if (tuples == null || tuples.Count == 0) return new List<Mobile>();
            var pairedMobs = new List<Mobile>();
            var count = tuples.Count;
            for (var i = 0; i < count; i++)
            {
                if (tuples[i].Item1.Serial==targetElement.Serial)
                {
                    pairedMobs.Add(tuples[i].Item2); // Retrieve the paired element
                }
                else if (tuples[i].Item2.Serial==targetElement.Serial)
                {
                    pairedMobs.Add(tuples[i].Item1); // Retrieve the paired element
                }
            }
            return pairedMobs;
        }
        public static List<Mobile> FindMatchedPairAndRemove(ref List<(Mobile, Mobile)> tuples, Mobile targetElement)
        {
            // It's ok for targetElement to be null
            if (tuples == null || tuples.Count == 0) return new List<Mobile>();
            var pairedMobs = new List<Mobile>();
            var removeTuples = new List<int>();
            var count = tuples.Count;
            for (var i = 0; i < count; i++)
            {
                if (tuples[i].Item1.Serial == targetElement.Serial)
                {
                    removeTuples.Add(i); // Remove matching tuple
                    pairedMobs.Add(tuples[i].Item2); // Retrieve the paired element
                }
                if (tuples[i].Item2.Serial == targetElement.Serial)
                {
                    removeTuples.Add(i);; // Remove matching tuple
                    pairedMobs.Add(tuples[i].Item1); // Retrieve the paired element
                }
                if (tuples[i].Item1.Serial == targetElement.Serial && tuples[i].Item2.Serial == targetElement.Serial)
                {
                    removeTuples.Add(i);; // Remove glitched tuple
                }
            }
            //trim tuples
            tuples = tuples.Where((mob, index) => !removeTuples.Contains(index)).ToList();
            return pairedMobs;
        }
        //add FindEnemy method with Warmode, Friend, Notoriety
        public static Mobile FindMobileBySerial(List<int> serials)
        {
            if (serials == null || serials.Count == 1 && serials[0] == 0)
            {
                var mobileFilter = new Mobiles.Filter()
                {
                    //Enabled
                    //Serials
                    //Bodies
                    //Graphics
                    //Name
                    //Hues
                    //RangeMin
                    //RangeMax
                    //ZLevelMin
                    //ZLevelMax
                    //CheckLineOfSight
                    //CheckLineOfSite
                    //Poisoned
                    //Blessed
                    //IsHuman
                    //IsGhost
                    //Female
                    //Warmode
                    //Friend
                    //Paralized
                    //CheckIgnoreObject
                    //IgnorePets
                    //Notorieties
                    // Supports .Add() and .AddRange()
                    //     1: blue, innocent
                    //     2: green, friend
                    //     3: gray, neutral
                    //     4: gray, criminal
                    //     5: orange, enemy
                    //     6: red, hostile 
                    //     6: yellow, invulnerable
                    Enabled = true,
                    RangeMax = 15
                };
                var mobiles = Mobiles.ApplyFilter(mobileFilter);
                if (mobiles == null || mobiles.Count == 0) return null;
                Misc.SendMessage("No Serial Found " + mobiles.Count + " mobiles",33);
                return Mobiles.Select(mobiles, "nearest");
            }
            else
            {
                var mobileFilter = new Mobiles.Filter()
                {
                    Enabled = true,
                    RangeMax = 15,
                    Serials = serials
                };
                var mobiles = Mobiles.ApplyFilter(mobileFilter);
                if (mobiles == null || mobiles.Count == 0) return null;
                Misc.SendMessage("Serials Found " + mobiles.Count + " mobiles",33);
                return Mobiles.Select(mobiles, "nearest");
            }
        }
    }
}