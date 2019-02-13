using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CnC_Hack
{
	class ConsoleHelper
	{
		public int WriteHackMenu(string[] menuItems, bool hackActive, bool gubed)
		{
			int curItem = 0, c;
			ConsoleKeyInfo key;
			while (true)
			{
				do
				{
					Console.Clear();
					Console.WriteLine("");
					Console.WriteLine("================ CnC Generals Hack V1 ================");
					Console.WriteLine("");
					// The loop that goes through all of the menu items.
					for (c = 0; c < menuItems.Length; c++)
					{
						if (hackActive)
							menuItems[0] = "Stop Hack";
						else
							menuItems[0] = "Start Hack";
						if (curItem == c)
						{
							if (!gubed) { if (menuItems[c] == "Debug") menuItems[c] = "Debug [Not Available]"; }
							else { if (menuItems[c] == "Debug" || menuItems[c] == "Debug [Not Available]") menuItems[c] = "Debug"; }
							Console.Write(">>");
							Console.WriteLine(menuItems[c]);
						}
						else
						{
							if (!gubed) { if (menuItems[c] == "Debug") menuItems[c] = "Debug [Not Available]"; }
							else { if (menuItems[c] == "Debug" || menuItems[c] == "Debug [Not Available]") menuItems[c] = "Debug"; }
							Console.WriteLine(menuItems[c]);
						}
					}
					Console.WriteLine("");
					Console.Write("Select your choice with the arrow keys.");
					key = Console.ReadKey(true);
					if (key.Key.ToString() == "DownArrow")
					{
						curItem++;
						if (curItem > menuItems.Length - 1) curItem = 0;
					}
					else if (key.Key.ToString() == "UpArrow")
					{
						curItem--;
						if (curItem < 0) curItem = Convert.ToInt16(menuItems.Length - 1);
					}
				} while (key.KeyChar != 13);
				return curItem;
			}
		}
		public void WriteLog(StreamWriter sw, int gameModul, int[,][] Offsets)
		{
			//sw.WriteLine("==== DEBUG LOG START ====");
			//sw.WriteLine("");
			//sw.WriteLine("");
			//sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GameModul Startaddress: 0x" + gameModul.ToString("X"));
			//sw.WriteLine("");
			//sw.WriteLine("");
			//for (int x = 0; x < Offsets.GetLength(0); x++)
			//{
			//	if(x==0)
			//		sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Offsets for Generals:");
			//	else
			//		sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Offsets for Generals - Zero Hour:");
			//	sw.WriteLine("");
			//	for (int y = 0; y < Offsets.GetLength(1); y++)
			//	{
			//		foreach (int item in Offsets[x,y])
			//		{
			//			sw.Write("");
			//		}
			//	}
			//}

			sw.WriteLine("==== DEBUG LOG START ====");
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] GameModul Startaddress: 0x" + gameModul.ToString("X"));
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Offsets for Generals:");
			sw.WriteLine("");
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] PlayBase: 0x" + Offsets[0, 0][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Money: 0x" + Offsets[0, 1][0].ToString("X") + " & 0x" + Offsets[0, 1][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Rank Points: 0x" + Offsets[0, 2][0].ToString("X") + " & 0x" + Offsets[0, 2][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Experience: 0x" + Offsets[0, 3][0].ToString("X") + " & 0x" + Offsets[0, 3][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Energy Used: 0x" + Offsets[0, 4][0].ToString("X") + " & 0x" + Offsets[0, 4][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Energy Buffer: 0x" + Offsets[0, 5][0].ToString("X") + " & 0x" + Offsets[0, 5][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Building + Units: 0x" + Offsets[0, 6][0].ToString("X") + " & 0x" + Offsets[0, 6][1].ToString("X") + " & 0x" + Offsets[0, 6][2].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Units Bytes: 0x" + Offsets[0, 7][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Bulding Bytes: 0x" + Offsets[0, 8][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Radar: 0x" + Offsets[0, 9][0].ToString("X") + " & 0x" + Offsets[0, 9][1].ToString("X"));
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Offsets for Generals - Zero Hour:");
			sw.WriteLine("");
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] PlayBase: 0x" + Offsets[1, 0][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Money: 0x" + Offsets[1, 1][0].ToString("X") + " & 0x" + Offsets[1, 1][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Rank Points: 0x" + Offsets[1, 2][0].ToString("X") + " & 0x" + Offsets[1, 2][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Experience: 0x" + Offsets[1, 3][0].ToString("X") + " & 0x" + Offsets[1, 3][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Energy Used: 0x" + Offsets[1, 4][0].ToString("X") + " & 0x" + Offsets[1, 4][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Energy Buffer: 0x" + Offsets[1, 5][0].ToString("X") + " & 0x" + Offsets[1, 5][1].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Building + Units: 0x" + Offsets[1, 6][0].ToString("X") + " & 0x" + Offsets[1, 6][1].ToString("X") + " & 0x" + Offsets[1, 6][2].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Units Bytes: 0x" + Offsets[1, 7][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Bulding Bytes: 0x" + Offsets[1, 8][0].ToString("X"));
			sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] Radar: 0x" + Offsets[1, 9][0].ToString("X") + " & 0x" + Offsets[1, 9][1].ToString("X"));
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("==== DEBUG LOG END ====");
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("");
			sw.WriteLine("");
		}
	}
	static class Extension
	{
		public static string GetDescription(this Enum e)
		{
			var attribute =
			e.GetType()
				.GetTypeInfo()
				.GetMember(e.ToString())
				.FirstOrDefault(member => member.MemberType == MemberTypes.Field)
				.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault()
				as DescriptionAttribute;

			return attribute?.Description ?? e.ToString();
		}
	}
}
