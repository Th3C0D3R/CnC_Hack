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
		static int entryBaseAddressZH = 0x8E0338;
		static int entryBaseAddress = 0x8425B8;
		static int[,][] Offsets = new int[2, 20][];
		public static void LoadOffsets()
		{
			//Generals Offsets
			Offsets[0, 0] = new int[] { 0x56C9B0 }; //PlayerBase
			Offsets[0, 1] = new int[] { 0xC, 0x34 }; //Money
			Offsets[0, 2] = new int[] { 0xC, 0x17C };//RankPoints
			Offsets[0, 3] = new int[] { 0xC, 0x178 };//EXP
			Offsets[0, 4] = new int[] { 0xC, 0x78 };//Energy Used
			Offsets[0, 5] = new int[] { 0xC, 0x74 };//Energy Produces
			Offsets[0, 6] = new int[] { 0x132E9C, 0x132EA7, 0x132EA8 }; // Unit + Buildings
			Offsets[0, 7] = new int[] { 0x1 }; // Units
			Offsets[0, 8] = new int[] { 0x90 }; // Buildings

			//Zero Hour Offsets
			Offsets[1, 0] = new int[] { 0x62B600 };//PlayerBase Zero Hour
			Offsets[1, 1] = new int[] { 0xC, 0x38 }; //Money Zero Hour
			Offsets[1, 2] = new int[] { 0xC, 0x190 };//RankPoints Zero Hour
			Offsets[1, 3] = new int[] { 0xC, 0x18C };//EXP Zero Hour
			Offsets[1, 4] = new int[] { 0xC, 0x88 }; //Energy Used Zero Hour
			Offsets[1, 5] = new int[] { 0xC, 0x84 }; // Energy Produces Zero Hour
			Offsets[1, 6] = new int[] { 0x13A0FC, 0x13A107, 0x13A108 }; // Unit + Buildings
			Offsets[1, 7] = new int[] { 0x1 }; // Units
			Offsets[1, 8] = new int[] { 0x90 }; // Buildings
		}

		#endregion

		static bool hackActive = false;
		static bool isZeroHour = false;
		static bool gubed = false;
		static string[] menuItems = { "Start Hack", "Debug", "Exit" };
		static Process process;  //search value
		static ConsoleHelper ch = new ConsoleHelper();

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
			foreach (Process item in Process.GetProcessesByName("Generals"))
			{
				Console.WriteLine(item.MainModule.EntryPointAddress);
				Console.WriteLine(entryBaseAddress);
				Console.WriteLine(entryBaseAddressZH);
				if (item.MainModule.EntryPointAddress == new IntPtr(entryBaseAddress))
				{
					isZeroHour = false;
					process = item;
				}
				else if(item.MainModule.EntryPointAddress == new IntPtr(entryBaseAddressZH))
				{
					isZeroHour = true;
					process = item;
				}
				Console.ReadKey();
			}
			LoadOffsets();
			RenderMenu();
		}
		public static bool hack(int value, Int32 off1, Int32 off2)
		{
			try
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine(value.ToString() + " - " + off1.ToString("X") +" - " + off2.ToString("X"));
				Console.WriteLine(gameModul.ToString("X") + " - " + Offsets[0, 0][0].ToString("X"));
				Console.ReadKey();
				return false;
			}

		}
		public static int hackRadar()
		{
			int i = isZeroHour ? 1 : 0;
			byte[] bufferU = new byte[1];
			byte[] bufferB = new byte[1];
			IntPtr baseAddrUnit = new IntPtr(gameModul + Offsets[i, 6][0]);
			IntPtr baseAddrBuilding = new IntPtr(gameModul + Offsets[i, 6][1]);
			IntPtr baseAddrBuilding2 = new IntPtr(gameModul + Offsets[i, 6][2]);
			bufferU = BitConverter.GetBytes(Offsets[i, 7][0]);
			bufferB = BitConverter.GetBytes(Offsets[i, 8][0]);
			WriteProcessMemory(process.Handle, baseAddrUnit, bufferU, 1, out int refer);
			WriteProcessMemory(process.Handle, baseAddrBuilding, bufferB, 1, out refer);
			WriteProcessMemory(process.Handle, baseAddrBuilding2, bufferB, 1, out refer);
			return 1;
		}
		static public void RenderMenu()
		{
			int item = ch.WriteHackMenu(menuItems, hackActive, gubed);
			while (item != 3)
			{
				switch (item)
				{
					case 0:
						hackActive = !hackActive;
						if (hackActive)
						{
							int u = isZeroHour ? 1 : 0;
							hack(5000, Offsets[u, 3][0], Offsets[u, 3][1]); //RankEXP -> LevelUp to get StarRank
							hackRadar();
							Task.Factory.StartNew(() =>
							{
								doHack();
							});
						}
						break;
					case 1:
						if (gubed)
							doDebug();
						break;
					case 2:
						return;
					default:
						break;
				}
				item = ch.WriteHackMenu(menuItems, hackActive, gubed);
			}
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
					ch.WriteLog(sw, gameModul, Offsets);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.ReadKey();
			}
		}
		private static void doHack()
		{
			while (hackActive)
			{
				int i = isZeroHour ? 1 : 0;
				hack(696969, Offsets[i, 1][0], Offsets[i, 1][1]); //Money
				hack(69, Offsets[i, 2][0], Offsets[i, 2][1]); //RankPoints
				hack(0, Offsets[i, 4][0], Offsets[i, 4][1]); //Current Used Energy
				hack(999, Offsets[i, 5][0], Offsets[i, 5][1]); //Max Current Energy
				Thread.Sleep(100);
			}
		}
	}
}
