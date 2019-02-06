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
		[Flags]
		public enum ProcessAccessFlags : uint
		{
			All = 0x001F0FFF,
			Terminate = 0x00000001,
			CreateThread = 0x00000002,
			VMOperation = 0x00000008,
			VMRead = 0x00000010,
			VMWrite = 0x00000020,
			DupHandle = 0x00000040,
			SetInformation = 0x00000200,
			QueryInformation = 0x00000400,
			Synchronize = 0x00100000
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
		[DllImport("kernel32.dll")]
		static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, [Out] byte[] buffer, int size, out int lpNumberOfBytesWritten);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, int size, out int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll")]
		public static extern Int32 CloseHandle(IntPtr hProcess);

		const int PROCESS_ALL_ACCESS = 0x1F0FFF;
		static Process process;  //search value
		static int segment = 0x10000; //avoid the large object heap (> 84k)
		static int range = 0x7FFFFFFF - segment; //32-bit example
		static Dictionary<int, string> addresses = new Dictionary<int, string>();
		#endregion

		#region "Offsets"
		static int gameModul = 0x400000;
		static int playerbase = 0x58ED40;
		#endregion

		static unsafe void Main(string[] args)
		{
			Console.WriteLine("Looking for Generals.exe...");
			while(Process.GetProcessesByName("Generals").Count() == 0){ }
			Console.WriteLine("Generals.exe found!");
			process = Process.GetProcessesByName("Generals")[0];
			Console.Clear();
			int bytesRead;
			bool nextValue = false;
			int userInput = 0;
			do
			{
				userInput = DisplayMenu(nextValue);
				if (userInput == 55)
				{
					if (nextValue)
						Console.Write("Enter next Value: ");
					else
						Console.Write("Enter new Value: ");

					string value = Console.ReadLine();
					DateTime start = DateTime.Now;
					if (nextValue)
						bytesRead = lookDeeper(int.Parse(value));
					else
						bytesRead = lookForValue(int.Parse(value));
					Console.WriteLine();
					Console.WriteLine("Duration: {0} seconds", (DateTime.Now - start).TotalSeconds);
					Console.WriteLine("Found: {0}", addresses.Count);
					Console.WriteLine();
					nextValue = true;
				}
				else if (userInput == 56)
				{
					foreach (KeyValuePair<int, string> addr in addresses)
					{
						Console.WriteLine(addr.Key.ToString("X") + " -> " + addr.Value);
					}
				}
				else if (userInput == 1)
				{
					hackRankV1();
				}
				else if (userInput == 2)
				{
					hackMoney(999999);
				}
				else if (userInput == 10)
				{
					nextValue = false;
				}
				Console.WriteLine("");
				Console.WriteLine("");
			} while (userInput != 99);
			Console.Clear();
		}
		public static unsafe int lookForValue(int value)
		{
			int bytesRead = 0;
			for (int i = 0; i < range; i += segment)
			{
				byte[] buffer = new byte[segment];

				if (!ReadProcessMemory(process.Handle, new IntPtr(i), buffer, segment, out bytesRead))
				{
					continue;
				}
				IntPtr data = Marshal.AllocHGlobal(bytesRead);
				Marshal.Copy(buffer, 0, data, bytesRead);
				for (int j = 0; j < bytesRead; j++)
				{
					int current = *(int*)(data + j);

					if (current == value)
					{
						addresses.Add(i + j, value.ToString());
					}
				}
				Marshal.FreeHGlobal(data);
			}
			return bytesRead;
		}
		public static unsafe int lookDeeper(int value)
		{
			Dictionary<int, string> newAddr = addresses;
			addresses = new Dictionary<int, string>();
			int bytesRead = 0;
			for (int i = 0; i < range; i += segment)
			{
				byte[] buffer = new byte[segment];

				if (!ReadProcessMemory(process.Handle, new IntPtr(i), buffer, segment, out bytesRead))
				{
					continue;
				}

				IntPtr data = Marshal.AllocHGlobal(bytesRead);
				Marshal.Copy(buffer, 0, data, bytesRead);
				for (int j = 0; j < bytesRead; j++)
				{
					int current = *(int*)(data + j);

					if (current == value)
					{
						foreach (KeyValuePair<int, string> addr in newAddr)
						{
							if (addr.Key == (i + j))
							{
								string saddr = (i + j).ToString("X");
								if (saddr.StartsWith("5") && saddr.EndsWith("2B0") && saddr.Length == ("5".Length + 3 + "2B0".Length))
								{
									Console.WriteLine("Success on: " + saddr + "    with value: " + current.ToString());
									addresses.Add(i + j, current.ToString());
								}
							}

						}
					}
				}

				Marshal.FreeHGlobal(data);
			}

			return bytesRead;
		}
		public static bool hackMoney(int value)
		{
			byte[] buffer = new byte[4];
			IntPtr baseAddr = new IntPtr(gameModul + playerbase);
			ReadProcessMemory(process.Handle, baseAddr, buffer, buffer.Length, out int refer);
			IntPtr newaddres1 = new IntPtr(BitConverter.ToInt32(buffer, 0));

			ReadProcessMemory(process.Handle, IntPtr.Add(newaddres1, 0x2C), buffer, buffer.Length, out refer);
			IntPtr newaddres2 = new IntPtr(BitConverter.ToInt32(buffer, 0));

			ReadProcessMemory(process.Handle, IntPtr.Add(newaddres2, 0x18), buffer, buffer.Length, out refer);
			IntPtr newaddres3 = new IntPtr(BitConverter.ToInt32(buffer, 0));

			ReadProcessMemory(process.Handle, IntPtr.Add(newaddres3, 0x4), buffer, buffer.Length, out refer);
			IntPtr newaddres4 = new IntPtr(BitConverter.ToInt32(buffer, 0));

			ReadProcessMemory(process.Handle, IntPtr.Add(newaddres4, 0x18), buffer, buffer.Length, out refer);
			IntPtr newaddres5 = new IntPtr(BitConverter.ToInt32(buffer, 0));

			buffer = StructureToByteArray(value);
			bool written = WriteProcessMemory(process.Handle, IntPtr.Add(newaddres5, 0x440), buffer, buffer.Length, out refer);
			buffer = new byte[4];
			ReadProcessMemory(process.Handle, IntPtr.Add(newaddres5, 0x440), buffer, buffer.Length, out refer);
			int newvalue = BitConverter.ToInt32(buffer, 0);
			if (value == newvalue && written)
			{
				return true;
			}
			else
			{
				Console.WriteLine("");
				Console.WriteLine("===============================================================");
				Console.WriteLine("Could not write right MemoryAdress: ");
				Console.WriteLine("0 HEX: " + baseAddr.ToString("X"));
				Console.WriteLine("1 HEX: " + newaddres1.ToString("X"));
				Console.WriteLine("2 HEX: " + newaddres2.ToString("X"));
				Console.WriteLine("3 HEX: " + newaddres3.ToString("X"));
				Console.WriteLine("4 HEX: " + newaddres4.ToString("X"));
				Console.WriteLine("5 HEX: " + newaddres5.ToString("X"));
				Console.WriteLine("===============================================================");
				Console.WriteLine("");
				return false;
			}
		}
		public static bool hackRankV1()
		{
			byte[] buffer = new byte[16];
			int baseAddr = gameModul + playerbase;
			Console.WriteLine(baseAddr.ToString("X"));
			ReadProcessMemory(process.Handle, baseAddr, buffer, buffer.Length, out int refer);
			int newaddres1 = BitConverter.ToInt32(buffer, 0);
			IntPtr addr = (IntPtr)(newaddres1);
			Console.WriteLine(addr.ToString("X"));

			ReadProcessMemory(process.Handle, (newaddres1 + 0x2C), buffer, buffer.Length, out refer);
			int newaddres2 = BitConverter.ToInt32(buffer, 0);
			IntPtr addr2 = (IntPtr)(newaddres2);

			ReadProcessMemory(process.Handle, (newaddres2 + 0x18), buffer, buffer.Length, out refer);
			int newaddres3 = BitConverter.ToInt32(buffer, 0);
			IntPtr addr3 = (IntPtr)(newaddres3);

			ReadProcessMemory(process.Handle, (newaddres3 + 0x4), buffer, buffer.Length, out refer);
			int newaddres4 = BitConverter.ToInt32(buffer, 0);
			IntPtr addr4 = (IntPtr)(newaddres4);

			ReadProcessMemory(process.Handle, (newaddres4 + 0x18), buffer, buffer.Length, out refer);
			int newaddres5 = BitConverter.ToInt32(buffer, 0);
			IntPtr addr5 = (IntPtr)(newaddres5);

			buffer = StructureToByteArray(10);
			bool written = WriteProcessMemory(process.Handle, (newaddres5 + 0x588), buffer, buffer.Length, out refer);
			buffer = new byte[4];
			ReadProcessMemory(process.Handle, (newaddres5 + 0x588), buffer, buffer.Length, out refer);
			int value = BitConverter.ToInt32(buffer, 0);
			if (value == 10 && written)
			{
				return true;
			}
			else
			{
				Console.WriteLine("");
				Console.WriteLine("===============================================================");
				Console.WriteLine("Could not write right MemoryAdress: ");
				Console.WriteLine("0 HEX: " + baseAddr.ToString("X"));
				Console.WriteLine("1 HEX: " + newaddres1.ToString("X"));
				Console.WriteLine("2 HEX: " + newaddres2.ToString("X"));
				Console.WriteLine("3 HEX: " + newaddres3.ToString("X"));
				Console.WriteLine("4 HEX: " + newaddres4.ToString("X"));
				Console.WriteLine("5 HEX: " + newaddres5.ToString("X"));
				Console.WriteLine("5 HEX: " + newaddres5.ToString("X"));
				Console.WriteLine("===============================================================");
				Console.WriteLine("");
				return false;
			}
		}
		static public int DisplayMenu(bool next)
		{
			Console.WriteLine("CnC Hack");
			Console.WriteLine();
			Console.WriteLine("1. Hack Rank ");
			//if (next)
			//	Console.WriteLine("1. Search next Value");
			//else
			//	Console.WriteLine("1. Search Value (Value entered by YOU!)");
			//Console.WriteLine("2. List Addresses from last scan");
			Console.WriteLine("2. Hack Money");
			Console.WriteLine("10. Restart Scan");
			Console.WriteLine("99. Exit");
			var result = Console.ReadLine();
			return Convert.ToInt32(result);
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
