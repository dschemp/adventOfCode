using System;
using System.Collections.Generic;
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

            for (int i = 0; i < numbers.Length - 2; i++)
            {
                int x = numbers[i];
                
                for (int j = i; j < numbers.Length - 1; j++)
                {
                    int y = numbers[j];

                    if (x + y > maxSum)
                    {
                        continue;
                    }

                    for (int k = j; k < numbers.Length; k++)
                    {
                        int z = numbers[k];

                        if (x + y + z == maxSum)
                        {
                            Console.WriteLine($"Found x: {x} | y: {y} | z:{z} | x*y*z={x*y*z}");
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
