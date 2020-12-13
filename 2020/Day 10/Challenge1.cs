using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

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

            Dictionary<int, int> differences = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 } };

            for (int i = 0; i < orderedChargers.Count - 1; i++)
            {
                int diff = orderedChargers[i + 1] - orderedChargers[i];
                differences[diff]++;
            }
            var diffTexts = differences.Select(kvp => kvp.Key + ":" + kvp.Value.ToString());

            Console.WriteLine($"Differences: {string.Join(" | ", diffTexts)}");

            Console.WriteLine($"N(1) x N(3) = {differences[1] * differences[3]}");
        }
    }
}
