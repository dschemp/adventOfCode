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
            //string input = "0,3,6";
            string input = "1,0,15,2,10,13";
            int[] numbers = input.Split(',').Select(int.Parse).ToArray();

            int maxTurns = 30000000;
            //int maxTurns = 2020;
            int result = Play(numbers, maxTurns, 100000);
            Console.WriteLine($"The {maxTurns}th number is {result}");
        }

        static int Play(int[] initialNumbers, int maxTurns, int indicatorDistance = 1000)
        {
            List<int> numbers = new List<int>(initialNumbers);
            Dictionary<int, List<int>> numberToIndexMapping = new Dictionary<int, List<int>>();
            DateTime time = DateTime.Now;

            int turn = 0;
            int number = 0;
            while (turn < maxTurns)
            {
                if (turn < initialNumbers.Length)
                {
                    number = initialNumbers[turn];
                }
                else
                {
                    int turnIndex = numbers.LastIndexOf(number) + 1;
                    List<int> numberIndexes = numberToIndexMapping[number];

                    if (turnIndex <= 0)
                    {
                        Console.WriteLine("Error!");
                        return -1;
                    }

                    if (numberIndexes.Count == 1)
                    {
                        number = 0;
                    }
                    else if (numberIndexes.Count > 1)
                    {
                        int[] index = numberIndexes.TakeLast(2).ToArray();

                        number = index[1] - index[0];
                    }
                    numbers.Add(number);
                }

                if (turn % indicatorDistance == 0)
                {
                    TimeSpan span = DateTime.Now - time;
                    Console.WriteLine($"Turn {turn}: {number}");
                    Console.WriteLine($"Time taken: {(span).Seconds} s {span.Milliseconds} ms");
                    Console.WriteLine();
                    time = DateTime.Now;
                }
                turn++;

                if (!numberToIndexMapping.ContainsKey(number))
                {
                    numberToIndexMapping.Add(number, new List<int>());
                }
                numberToIndexMapping[number].Add(turn);
            }

            return number;
        }

        static IEnumerable<int> GetLastIndices(IEnumerable<int> enumerable, int number, int amount)
        {
            var ints = enumerable as int[] ?? enumerable.ToArray();
            int count = 0;

            for (int i = ints.Count() - 1; i >= 0 && count != amount; i--)
            {
                if (ints.ElementAt(i) == number)
                {
                    yield return i;
                    count++;
                }
            }
        }
    }
}
