using System;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main (string[] args)
        {
            string[] dimensions = File.ReadLines(@"input.txt").ToArray();

            ulong lengthOfRibbon = 0;

            foreach (var dim in dimensions)
            {
                lengthOfRibbon += calculateTotalLength(dim);
            }

            Console.WriteLine(lengthOfRibbon);
        }

        private static ulong calculateTotalLength(string dim)
        {
            ulong[] dims = dim.Split('x').Select(ulong.Parse).ToArray();

            ulong len = calculateLength(dims);

            return len + (dims[0] * dims[1] * dims[2]);
        }

        private static ulong calculateLength(ulong[] dims)
        {
            return
                  2 * dims[0]
                + 2 * dims[1]
                + 2 * dims[2]
                - 2 * Math.Max(Math.Max(dims[0], dims[1]), dims[2]);
        }
    }
}
