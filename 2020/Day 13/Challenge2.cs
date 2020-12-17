using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        /*
         *
         *  HUUUUUUGE Thanks go out to reddit user TheElTea on r/adventofcode
         *  for his solution:
         *
         *  https://www.reddit.com/r/adventofcode/comments/kc4njx/2020_day_13_solutions/gfvhle1?utm_source=share&utm_medium=web2x&context=3
         *  https://pastebin.com/jHpRYhzc
         *
         */
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            List<int> busLines = ParseBusLines(lines[1]).ToList();

            Dictionary<int, int> busLineToDiffMapping = new Dictionary<int, int>();
            for (int i = 0; i < busLines.Count; i++)
            {
                if (busLines[i] > 0)
                {
                    busLineToDiffMapping.Add(busLines[i], i);
                }
            }

            long testTime = Solve(busLineToDiffMapping, 100000000000000);
            Console.WriteLine($"Earliest departure on: {testTime}");
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
                else
                {
                    yield return 0;
                }
            }
        }

        static long GetEqualOrGreaterDepartureTime(long target, long busNumber)
        {
            long quotient = target / busNumber;
            long remainder = target % busNumber;

            if (remainder > 0)
            {
                quotient++;
            }

            return quotient * busNumber;
        }

        static long Solve(Dictionary<int, int> busLineToDiffMapping, long startTime)
        {
            int lockedIn = 0;
            long incrementAmount = busLineToDiffMapping.First().Key;
            
            startTime = GetEqualOrGreaterDepartureTime(startTime, incrementAmount);


            for (long testTime = startTime; true; testTime += incrementAmount)
            {
                int nextBusToLookFor = lockedIn + 1;
                var kvp = busLineToDiffMapping.ElementAt(nextBusToLookFor);
                long requiredDepartureTime = testTime + kvp.Value;
                long nearestDepartureTime = GetEqualOrGreaterDepartureTime(requiredDepartureTime, kvp.Key);

                if (requiredDepartureTime == nearestDepartureTime)
                {
                    incrementAmount *= kvp.Key;
                    lockedIn = nextBusToLookFor;

                    if (lockedIn == busLineToDiffMapping.Count - 1) //They're all lined up!
                    {
                        return testTime;
                    }
                }
            }
        }
    }
}
