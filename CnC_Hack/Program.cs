using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
		internal class OffsetGenerals
		{
			public int playerbase = 0x56C9B0;
			public int[] MoneyVOffset = { 0xC, 0x34 };
			public int[] RankVOffset = { 0xC, 0x17C };
			public int[] EXPVOffset = { 0xC, 0x178 };
			public int[] EnergieCurrOffset = { 0xC, 0x78 };
			public int[] EnergieMaxOffset = { 0xC, 0x74 };
		}
		internal class OffsetZeroHour
		{
			public int playerbase = 0x62B600;
			public int[] MoneyVOffset = { 0xC, 0x38 };
			public int[] RankVOffset = { 0xC, 0x190 };
			public int[] EXPVOffset = { 0xC, 0x18C };
			public int[] EnergieCurrOffset = { 0xC, 0x88 };
			public int[] EnergieMaxOffset = { 0xC, 0x84 };
		}


		#endregion

		static bool hackActive = false;
		static bool isZeroHour = false;
		static short curItem = 0, c;
		static ConsoleKeyInfo key;
		static string[] choiceItems = { "Generals", "Generals - Zero Hour" };
		static string[] menuItems = { "Start Hack", "Exit" };
		static Process process;  //search value
		static BackgroundWorker bwHack = new BackgroundWorker();
		static dynamic objOffsets;

		static unsafe void Main(string[] args)
		{
			Console.WriteLine("Looking for Generals.exe...");
			while (Process.GetProcessesByName("Generals").Count() == 0) { }
			Console.WriteLine("Generals.exe found!");
			Console.Clear();
			process = Process.GetProcessesByName("Generals")[0];
			bwHack.DoWork += BwHack_DoWork;
			int i = RenderChoice();
			while (i == -1) { i = RenderChoice(); }
			if (isZeroHour)
				objOffsets = new OffsetZeroHour();
			else
				objOffsets = new OffsetGenerals();
			RenderMenu();
		}

		private static void BwHack_DoWork(object sender, DoWorkEventArgs e)
		{
			Console.WriteLine(objOffsets.MoneyVOffset[1].ToString("X"));
			while (hackActive)
			{
				hack(696969, objOffsets.MoneyVOffset[0], objOffsets.MoneyVOffset[1]); //Money
				hack(69, objOffsets.RankVOffset[0], objOffsets.RankVOffset[1]); //RankPoints
				hack(0, objOffsets.EnergieCurrOffset[0], objOffsets.EnergieCurrOffset[1]); //Current Used Energy
				hack(999, objOffsets.EnergieMaxOffset[0], objOffsets.EnergieMaxOffset[1]); //Max Current Energy
				Thread.Sleep(1000);
			}
		}
		public static bool hack(int value, Int32 off1, Int32 off2)
		{
			byte[] buffer = new byte[4];
			IntPtr baseAddr = new IntPtr(gameModul + objOffsets.playerbase);
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
					//Console.Clear();
					Console.WriteLine("");
					Console.WriteLine("================ CnC Generals Hack V1 ================");
					Console.WriteLine("");
					Console.WriteLine("PlayerBase Address: " + objOffsets.playerbase.ToString("X"));
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
							Console.Write(">>");
							Console.WriteLine(menuItems[c]);
						}
						else
						{
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
							hack(5000, objOffsets.EXPVOffset[0], objOffsets.EXPVOffset[1]); //RankEXP -> LevelUp to get StarRank
							bwHack.RunWorkerAsync();
						}
						break;
					case 1:
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

	}
}
