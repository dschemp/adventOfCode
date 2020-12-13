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

            long amountIterative = GetAmountOfValidCombinations(orderedChargers);

            long amountRecursive = GetValidCombinations(0, orderedChargers, new Dictionary<int, long>());

            Console.WriteLine($"Amount of valid combinations: {amountRecursive}");
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

        /*
         * Alternative Solution
         */
        static long GetValidCombinations(int value, List<int> chargers, Dictionary<int, long> mapping)
        {
            if (!chargers.Contains(value))
            {
                return 0;
            }

            if (value == chargers.Max())
            {
                return 1;
            }

            if (mapping.ContainsKey(value))
            {
                return mapping[value];
            }

            long val1 = GetValidCombinations(value + 1, chargers, mapping);
            if (!mapping.ContainsKey(value + 1))
            {
                mapping.Add(value + 1, val1);
            }

            long val2 = GetValidCombinations(value + 2, chargers, mapping);
            if (!mapping.ContainsKey(value + 2))
            {
                mapping.Add(value + 2, val2);
            }

            long val3 = GetValidCombinations(value + 3, chargers, mapping);
            if (!mapping.ContainsKey(value + 3))
            {
                mapping.Add(value + 3, val3);
            }

            return GetValidCombinations(value + 1, chargers, mapping) +
                   GetValidCombinations(value + 2, chargers, mapping) +
                   GetValidCombinations(value + 3, chargers, mapping);
        }
    }
}
