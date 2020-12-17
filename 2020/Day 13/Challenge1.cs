using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            int earliestDeparture = int.Parse(lines[0]);
            List<int> busLines = ParseBusLines(lines[1]).ToList();

            GetEarliestNearDeparture(busLines, earliestDeparture, out int busLine, out int differenceToNextDeparture);
            Console.WriteLine(busLine + " " + differenceToNextDeparture);
            Console.WriteLine(busLine * differenceToNextDeparture);
        }

        static void GetEarliestNearDeparture(List<int> busLines, int departureTime, out int busLine, out int differenceToNextDeparture)
        {
            busLine = -1;
            differenceToNextDeparture = int.MaxValue;

            Dictionary<int, int> mappingBusLinesToDifference =
                busLines.ToDictionary(line => line, i => i - (departureTime - (departureTime / i) * i));

            foreach (var kvp in mappingBusLinesToDifference)
            {
                if (kvp.Value < differenceToNextDeparture)
                {
                    busLine = kvp.Key;
                    differenceToNextDeparture = kvp.Value;
                }
            }
        }

        static IEnumerable<int> ParseBusLines(string buses)
        {
            string[] parts = buses.Split(',');

            foreach (var part in parts)
            {
                if (int.TryParse(part, out int result))
                {
                    yield return result;
                }
            }
        }
    }
}
