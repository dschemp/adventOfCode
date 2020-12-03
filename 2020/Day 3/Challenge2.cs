using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"input.txt");

            List<int[]> deltas = new List<int[]>
            {
                new [] {1, 1},
                new [] {3, 1},
                new [] {5, 1},
                new [] {7, 1},
                new [] {1, 2}
            };

            IEnumerable<int> numberOfTrees = deltas.Select(d => CountTreesAlongPath(lines, d));

            string foundTreesPerDelta = string.Join("|", numberOfTrees);

            ulong product = 1;
            foreach (var numberOfTree in numberOfTrees)
            {
                product *= (ulong)numberOfTree;
            }

            Console.WriteLine($"Number of trees per delta: {foundTreesPerDelta} / Product: {product}");
        }

        static int CountTreesAlongPath(string[] lines, int[] delta)
        {
            // 0 => x, 1 => y
            int[] currentPostition = new[] { 0, 0 };
            int segmentWidth = lines[0].Length;

            int numberOfTrees = 0;

            while (currentPostition[1] < lines.Length)
            {
                int y = currentPostition[1];
                string line = lines[y];

                char c = line.ElementAt(currentPostition[0]);
                if (c == '#')
                {
                    numberOfTrees++;
                }

                currentPostition[0] = (currentPostition[0] + delta[0]) % segmentWidth;
                currentPostition[1] += delta[1];
            }

            return numberOfTrees;
        }
    }
}
