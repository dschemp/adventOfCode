using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");
            List<int> outputJolts = lines.Select(int.Parse).ToList();

            int maxChargerJolts = outputJolts.Max();
            int ownChargerJolts = maxChargerJolts + 3;

            Console.WriteLine($"Max Charger J: {maxChargerJolts} | Own Charger J: {ownChargerJolts}");

            outputJolts.Add(0);
            outputJolts.Add(ownChargerJolts);

            List<int> orderedChargers = outputJolts.OrderBy(t => t).ToList();

            long amount = GetAmountOfValidCombinations(orderedChargers);

            Console.WriteLine($"Amount of valid combinations: {amount}");
        }

        /*
         * Thanks for this solution go out to lukasj3731
         *
         * His Solution:
         * https://github.com/lukasj3731/Advent_of_Code/blob/master/AOC2020/src/AOC10.java
         *
         */
        static long GetAmountOfValidCombinations(List<int> chargers)
        {
            Dictionary<int, long> mapping = new Dictionary<int, long>();
            int ownChargerJolts = chargers.Max();

            mapping.Add(ownChargerJolts, 1);

            for (int i = chargers.Count - 2; i >= 0; i--)
            {
                int currentCharger = chargers[i];

                long total = 0;
                for (int j = 1; j <= 3; j++)
                {
                    if (mapping.ContainsKey(currentCharger + j))
                    {
                        total += mapping[currentCharger + j];
                    }
                }
                mapping.Add(currentCharger, total);
            }

            return mapping[0];
        }
    }
}
