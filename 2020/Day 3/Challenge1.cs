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

            // 0 => x, 1 => y
            int[] currentPostition = new[] { 0, 0 };
            int[] delta = new[] { 3, 1 };
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

            Console.WriteLine($"Number of trees crossed: {numberOfTrees}");
        }
    }
}
