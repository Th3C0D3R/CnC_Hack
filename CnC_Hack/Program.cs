using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static CnC_Hack.Memory;

namespace CnC_Hack
{
    class Program
    {
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
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        static Process process;  //search value
        static int segment = 0x10000; //avoid the large object heap (> 84k)
        static int range = 0x7FFFFFFF - segment; //32-bit example
        static Dictionary<int,string> addresses = new Dictionary<int, string>();

        static unsafe void Main(string[] args)
        {
            process = Process.GetProcessesByName("Generals")[0];
            Console.Clear();
            int bytesRead;
            bool nextValue = false;
            int userInput = 0;
            do
            {
                userInput = DisplayMenu(nextValue);
                if(userInput == 1)
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
                else if(userInput == 2)
                {
                    foreach (KeyValuePair<int,string> addr in addresses)
                    {
                        Console.WriteLine(addr.Key.ToString("X") + " -> "+ addr.Value);
                    }
                }
                else if (userInput == 3)
                {
                    Console.Write("Enter Value written to memory: ");
                    string value = Console.ReadLine();
                    WriteMoney(addresses.First().Key, int.Parse(value));
                }
                else if (userInput == 4)
                {
                    int value = ReadMoney(addresses.First().Key);
                    Console.WriteLine("Value at " + addresses.First().Key.ToString("X") + ": " + value);
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
                        addresses.Add(i + j,value.ToString());
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
                                    Console.WriteLine("Success on: " + saddr + "    with value: "+ current.ToString());
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
        public static int ReadMoney(int addr)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(process.Handle, (IntPtr)addr, buffer, buffer.Length, out int refer);
            return BitConverter.ToInt32(buffer, 0);
        }
        public static void  WriteMoney(int addr, int value)
        {
            byte[] buffer = StructureToByteArray(value);
            WriteProcessMemory((int)process.Handle, addr, buffer, buffer.Length, out int refer);
        }
        static public int DisplayMenu(bool next)
        {
            Console.WriteLine("CnC Hack");
            Console.WriteLine();
            if(next)
                Console.WriteLine("1. Search next Value");
            else
                Console.WriteLine("1. Search Value (Value entered by YOU!)");
            Console.WriteLine("2. List Addresses from last scan");
            Console.WriteLine("3. Write Memory");
            Console.WriteLine("4. Read Memory");
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
