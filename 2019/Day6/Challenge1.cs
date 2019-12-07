using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Node
    {
        public string Data { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();

#if DEBUG
        // Debug message in VS
        public override string ToString()
        {
            return $"{Data} ) {string.Join(",", Children.Select(s => s.Data))}";
        }
#endif
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<(string planet, string orbit)> list =
                File.ReadAllLines(@"input.txt")
                .Select(s => s.Split(')'))
                .Select(s => (s[0], s[1]))
                .ToList();

            var comNode = new Node { Data = "COM" };

            BuildTree(comNode, list);

            var indirects = CalculateIndirectOrbits(comNode);

            Console.WriteLine($"Indirects: {indirects}");
            Console.WriteLine($"Directs:   {list.Count}");
            Console.WriteLine($"Total:     {list.Count + indirects}");

            Console.WriteLine("Hello World!");
        }

        private static void BuildTree(Node parentNode, List<(string planet, string orbit)> list)
        {
            var children = list
                .Where(s => s.planet == parentNode.Data)
                .Select(s => s.orbit)
                .ToArray();

            foreach (var child in children)
            {
                parentNode.Children.Add(new Node { Data = child });
            }

            foreach (var child in parentNode.Children)
            {
                BuildTree(child, list);
            }
        }

        public static int CalculateIndirectOrbits(Node node, int n = -1)
        {
            int result = (n < 0) ? 0: n;
            n++;
            foreach (var child in node.Children)
            {
                result += CalculateIndirectOrbits(child, n);
            }

            return result;
        }
    }
}
