using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CnC_Hack
{
	class Program
	{
		#region "Stuff"
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, int size, out int lpNumberOfBytesWritten);
		#endregion

		#region "Offsets"
		static int gameModul = 0x400000;
		static int[,][] Offsets = new int[2, 6][];
		public static void LoadOffsets()
		{
			Offsets[0, 0] = new int[] { 0x56C9B0 }; //PlayerBase
			Offsets[0, 1] = new int[] { 0xC, 0x34 }; //Money
			Offsets[0, 2] = new int[] { 0xC, 0x17C };//RankPoints
			Offsets[0, 3] = new int[] { 0xC, 0x178 };//EXP
			Offsets[0, 4] = new int[] { 0xC, 0x78 };//Energy Used
			Offsets[0, 5] = new int[] { 0xC, 0x74 };//Energy Produces
			Offsets[1, 0] = new int[] { 0x62B600 };//PlayerBase Zero Hour
			Offsets[1, 1] = new int[] { 0xC, 0x38 }; //Money Zero Hour
			Offsets[1, 2] = new int[] { 0xC, 0x190 };//RankPoints Zero Hour
			Offsets[1, 3] = new int[] { 0xC, 0x18C };//EXP Zero Hour
			Offsets[1, 4] = new int[] { 0xC, 0x88 }; //Energy Used Zero Hour
			Offsets[1, 5] = new int[] { 0xC, 0x84 }; // Energy Produces Zero Hour
		}

		#endregion

		static bool hackActive = false;
		static bool isZeroHour = false;
		static bool gubed = false;
		static short curItem = 0, c;
		static ConsoleKeyInfo key;
		static string[] choiceItems = { "Generals", "Generals - Zero Hour" };
		static string[] menuItems = { "Start Hack", "Change Game", "Debug", "Exit" };
		static Process process;  //search value
		static BackgroundWorker bwHack = new BackgroundWorker();

		static unsafe void Main(string[] args)
		{

			if (args.Length == 1)
			{
				if (new string(args[0].ToLower().ToCharArray().Reverse().ToArray()) == "gubed")
				{
					gubed = true;
				}
			}

			Console.WriteLine("Looking for Generals.exe...");
			while (Process.GetProcessesByName("Generals").Count() == 0) { }
			Console.WriteLine("Generals.exe found!");
			Console.Clear();
			process = Process.GetProcessesByName("Generals")[0];
			bwHack.DoWork += BwHack_DoWork;
			bwHack.WorkerSupportsCancellation = true;
			LoadOffsets();
			int i = RenderChoice();
			while (i == -1) { i = RenderChoice(); }
			RenderMenu();
		}

		private static void BwHack_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bw = (BackgroundWorker)sender;
			while (hackActive)
			{
				if (bw.CancellationPending) hackActive = false;
				else
				{
					int i = isZeroHour ? 1 : 0;
					hack(696969, Offsets[i, 1][0], Offsets[i, 1][1]); //Money
					hack(69, Offsets[i, 2][0], Offsets[i, 2][1]); //RankPoints
					hack(0, Offsets[i, 4][0], Offsets[i, 4][1]); //Current Used Energy
					hack(999, Offsets[i, 5][0], Offsets[i, 5][1]); //Max Current Energy
					Thread.Sleep(1000);
				}
			}
		}
		public static bool hack(int value, Int32 off1, Int32 off2)
		{
			int i = isZeroHour ? 1 : 0;
			byte[] buffer = new byte[4];
			IntPtr baseAddr = new IntPtr(gameModul + Offsets[i, 0][0]);
			IntPtr offsetAddress;
			ReadProcessMemory(process.Handle, baseAddr, buffer, buffer.Length, out int refer);
			offsetAddress = new IntPtr(BitConverter.ToInt32(buffer, 0));

			ReadProcessMemory(process.Handle, IntPtr.Add(offsetAddress, off1), buffer, buffer.Length, out refer);
			offsetAddress = new IntPtr(BitConverter.ToInt32(buffer, 0));

			buffer = StructureToByteArray(value);
			bool written = WriteProcessMemory(process.Handle, IntPtr.Add(offsetAddress, off2), buffer, buffer.Length, out refer);
			return written;
		}
		static public void RenderMenu()
		{
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
				switch (curItem)
				{
					case 0:
						hackActive = !hackActive;
						if (hackActive)
						{
							hack(5000, Offsets[isZeroHour ? 1 : 0, 3][0], Offsets[isZeroHour ? 1 : 0, 3][1]); //RankEXP -> LevelUp to get StarRank
							bwHack.RunWorkerAsync();
						}
						break;
					case 1:
						int i = RenderChoice();
						while (i == -1) { i = RenderChoice(); }
						bwHack.CancelAsync();
						break;
					case 2:
						if(gubed)
							doDebug();
						break;
					case 3:
						return;
					default:
						break;
				}
			}
		}
		static public int RenderChoice()
		{
			do
			{
				Console.Clear();
				Console.WriteLine("================ CnC Generals Hack V1 ================");
				Console.WriteLine("");
				// The loop that goes through all of the menu items.
				for (c = 0; c < choiceItems.Length; c++)
				{
					if (curItem == c)
					{
						Console.Write(">>");
						Console.WriteLine(choiceItems[c]);
					}
					else
					{
						Console.WriteLine(choiceItems[c]);
					}
				}
				Console.WriteLine("");
				Console.Write("Select your choice with the arrow keys.");
				key = Console.ReadKey(true);
				if (key.Key.ToString() == "DownArrow")
				{
					curItem++;
					if (curItem > choiceItems.Length - 1) curItem = 0;
				}
				else if (key.Key.ToString() == "UpArrow")
				{
					curItem--;
					if (curItem < 0) curItem = Convert.ToInt16(choiceItems.Length - 1);
				}
			} while (key.KeyChar != 13);
			switch (curItem)
			{
				case 0:
					curItem = 0;
					isZeroHour = false;
					return 0;
				case 1:
					curItem = 0;
					isZeroHour = true;
					return 1;
				default:
					curItem = 0;
					return -1;
					break;
			}
			curItem = 0;
			return -1;
		}
		private static byte[] StructureToByteArray(object obj)
		{
			int len = Marshal.SizeOf(obj);

			byte[] arr = new byte[len];

			IntPtr ptr = Marshal.AllocHGlobal(len);

			Marshal.StructureToPtr(obj, ptr, true);
			Marshal.Copy(ptr, arr, 0, len);
			Marshal.FreeHGlobal(ptr);

			return arr;
		}
		private static void doDebug()
		{
			try
			{
				if (!File.Exists("debug.log"))
				{
					FileStream fs = File.Create("debug.log");
					fs.Dispose();
					fs.Close();
				}
				using (StreamWriter sw = new StreamWriter("debug.log", true))
				{
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
					sw.WriteLine("");
					sw.WriteLine("");
					sw.WriteLine("==== DEBUG LOG END ====");
					sw.WriteLine("");
					sw.WriteLine("");
					sw.WriteLine("");
					sw.WriteLine("");
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.ReadKey();
			}
		}
	}
}
