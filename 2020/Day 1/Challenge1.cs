using System;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] inputLines = File.ReadAllLines(@"input.txt");
            int[] numbers = inputLines.Select(int.Parse).ToArray();
            int maxSum = 2020;
            bool found = false;

            for (int i = 0; i < numbers.Length - 1; i++)
            {
                for (int j = i; j < numbers.Length; j++)
                {
                    int x = numbers[i];
                    int y = numbers[j];

                    if (x + y == maxSum)
                    {
                        Console.WriteLine($"Found x: {x} | y: {y} | x*y={x*y}");
                        found = true;
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("Nothing found!");
            }
        }
    }
}
