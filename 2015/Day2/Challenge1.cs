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

            ulong lengthOfWrappingPaper = 0;

            foreach (var dim in dimensions)
            {
                lengthOfWrappingPaper += calculateLength(dim);
            }

            Console.WriteLine(lengthOfWrappingPaper);
        }

        private static ulong calculateLength(string dim)
        {
            ulong[] dims = dim.Split('x').Select(ulong.Parse).ToArray();

            ulong area = calculateArea(dims);

            return area + Math.Min(Math.Min(dims[0] * dims[1], dims[1] * dims[2]), dims[2] * dims[0]);
        }

        private static ulong calculateArea(ulong[] dims)
        {
            return 
                  2 * dims[0] * dims[1]
                + 2 * dims[1] * dims[2]
                + 2 * dims[2] * dims[0];
        }
    }
}
