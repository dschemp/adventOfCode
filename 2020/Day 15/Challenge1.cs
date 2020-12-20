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

            Play(numbers, 2020);
        }

        static void Play(int[] initialNumbers, int maxTurns)
        {
            List<int> numbers = new List<int>(initialNumbers);

            int turn = 1;
            int number = 0;
            while (turn <= maxTurns)
            {
                if (turn < initialNumbers.Length + 1)
                {
                    number = initialNumbers[turn - 1];
                }
                else
                {
                    int turnIndex = numbers.LastIndexOf(number) + 1;
                    List<int> numberIndexes = FindAllIndexes(numbers, number).ToList();

                    if (turnIndex <= 0)
                    {
                        Console.WriteLine("Error!");
                        return;
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

                Console.WriteLine($"Turn {turn}: {number}");
                turn++;
            }
        }

        static IEnumerable<int> FindAllIndexes(IEnumerable<int> enumerable, int number)
        {
            var ints = enumerable as int[] ?? enumerable.ToArray();

            for (int i = 0; i < ints.Count(); i++)
            {
                if (ints.ElementAt(i) == number)
                {
                    yield return i;
                }
            }
        }
    }
}
