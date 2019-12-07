using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    #region Node Class

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

    #endregion

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

            List<Node> list_SAN = new List<Node>(), list_YOU = new List<Node>();
            var path_SAN = GetPath(comNode, "SAN", list_SAN);
            var path_YOU = GetPath(comNode, "YOU", list_YOU);

            Console.WriteLine("COM -> YOU: " + string.Join("->", list_YOU.Select(s => s.Data)));
            Console.WriteLine("COM -> SAN: " + string.Join("->", list_SAN.Select(s => s.Data)));

            var contains = list_SAN.Where(p => list_YOU.Contains(p)).ToArray();
            Console.WriteLine("Common: " + string.Join(",", contains.Select(s => s.Data)));

            var l_you = list_YOU.Where(p => !list_SAN.Contains(p)).Select(s => s.Data);
            var l_san = list_SAN.Where(p => !list_YOU.Contains(p)).Select(s => s.Data);

            Console.WriteLine("MIN -> YOU: " + string.Join("->", l_you));
            Console.WriteLine("MIN -> SAN: " + string.Join("->", l_san));

            Console.WriteLine($"Total: {l_you.Count() + l_san.Count() - 2}");
        }

        public static bool GetPath(Node node, string search, List<Node> list)
        {
            if (node.Data == search)
            {
                list.Add(node);
                return true;
            }
            else if (node.Children.Count != 0)
            {
                foreach (var child in node.Children)
                {
                    var result = GetPath(child, search, list);
                    if (result)
                    {
                        if (!list.Contains(node))
                            list.Add(node);
                        return true;
                    }
                }
            }
            return false;
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
    }
}
