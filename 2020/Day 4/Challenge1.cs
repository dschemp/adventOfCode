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
            string[] lines = File.ReadAllLines(@"input.txt");
            List<Dictionary<string, string>> entitites = Parse(lines).ToList();

            int countOfValidPassports = 0;
            foreach (var entity in entitites)
            {
                bool isValidPassport = IsValidEntity(entity);
                if (isValidPassport)
                {
                    countOfValidPassports++;
                }

                Console.WriteLine(string.Join(" ", entity));
                Console.WriteLine($"Is Valid? {isValidPassport}");
                Console.WriteLine();
            }

            Console.WriteLine($"Valid Passports: {countOfValidPassports}");
        }

        static bool IsValidEntity(Dictionary<string, string> props)
        {
            if (props.Count == 8)
            {
                return true;
            }

            if (props.Count == 7 && !props.ContainsKey("cid"))
            {
                return true;
            }

            return false;
        }

        static IEnumerable<Dictionary<string, string>> Parse(string[] lines)
        {
            IEnumerable<string> singleLineEntities = ConvertToSingleLineEntities(lines);
            foreach (var entity in singleLineEntities)
            {
                yield return ParseLine(entity);
            }
        }

        static Dictionary<string, string> ParseLine(string lineText)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();

            string[] props = lineText.Split(' ');
            foreach (var items in props)
            {
                string[] mapping = items.Split(':');

                if (mapping.Length == 2 && !string.IsNullOrWhiteSpace(mapping[0]) && !string.IsNullOrWhiteSpace(mapping[1]))
                {
                    obj.Add(mapping.First(), mapping.Last());
                }
            }
            
            return obj;
        }

        static IEnumerable<string> ConvertToSingleLineEntities(string[] lines)
        {
            string buffer = String.Empty;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) && !string.IsNullOrWhiteSpace(buffer))
                {
                    yield return buffer;
                    buffer = string.Empty;
                }
                else
                {
                    buffer = string.Join(" ", buffer.Trim(), line?.Trim());
                }
            }

            yield return buffer;
        }
    }
}
