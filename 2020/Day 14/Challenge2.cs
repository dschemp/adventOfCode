﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");
            Memory mem = new Memory();
            ParseAndRun(lines, mem);

            Console.WriteLine($"Sum of memory content: {mem.SumOfMemoryContent()}");
        }

        static void ParseAndRun(IEnumerable<string> lines, Memory mem)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith("mask"))
                {
                    mem.SetMask(line.Substring(7));
                }
                else
                {
                    (long address, long value) = ParseMemorySetLine(line);
                    if (address != -1)
                    {
                        mem.SetMemory(address, value);
                    }
                }
            }
        }

        private static (long, long) ParseMemorySetLine(string line)
        {
            Regex regex = new Regex(@"mem\[(\d+)\] = (\d+)");
            Match match = regex.Match(line);

            if (match.Groups.Count != 3)
            {
                return (-1, -1);
            }

            long address = long.Parse(match.Groups[1].Value);
            long value = long.Parse(match.Groups[2].Value);
            return (address, value);
        }
    }

    class Memory
    {
        private Dictionary<long, long> MemoryMapping { get; set; }

        private Func<long, IEnumerable<long>> ApplyBitmaskFunction { get; set; }

        public Memory()
        {
            MemoryMapping = new Dictionary<long, long>();
            ApplyBitmaskFunction = null;
        }

        public void SetMask(string mask)
        {
            Console.WriteLine($"Setting bitmask to '{mask}'");

            List<int> setToOneOffsets = new List<int>();
            List<int> setToFloatingOffsets = new List<int>();

            for (int i = mask.Length - 1; i >= 0; i--)
            {
                char c = mask[i];
                if (c == '1')
                {
                    setToOneOffsets.Add(mask.Length - i - 1);
                }
                else if (c == 'X')
                {
                    setToFloatingOffsets.Add(mask.Length - i - 1);
                }
            }

            ApplyBitmaskFunction = l =>
            {
                foreach (var toOneOffset in setToOneOffsets)
                {
                    l |= ((long)1 << toOneOffset);
                }

                return GetAllFloatingCombinations(l, setToFloatingOffsets);
            };
        }

        private IEnumerable<long> GetAllFloatingCombinations(long value, IEnumerable<int> offsets)
        {
            var enumerable = offsets.ToList();
            if (!enumerable.Any())
            {
                yield break;
            }

            int offset = enumerable.First();

            value &= ~((long) 1 << offset);
            yield return value;
            foreach (var allFloatingCombination in GetAllFloatingCombinations(value, enumerable.Skip(1)))
            {
                yield return allFloatingCombination;
            }

            value |= ((long) 1 << offset);
            yield return value;
            foreach (var allFloatingCombination in GetAllFloatingCombinations(value, enumerable.Skip(1)))
            {
                yield return allFloatingCombination;
            }
        }

        public void SetMemory(long address, long value)
        {
            long[] addresses = ApplyBitmaskFunction(address).ToHashSet().ToArray();
            foreach (var addr in addresses)
            {
                Console.WriteLine($"Setting memory at '{addr}' from to '{value}'");

                if (MemoryMapping.ContainsKey(addr))
                {
                    MemoryMapping[addr] = value;
                }
                else
                {
                    MemoryMapping.Add(addr, value);
                }
            }
        }

        public long SumOfMemoryContent()
        {
            return MemoryMapping.Sum(kvp => kvp.Value);
        }
    }
}
