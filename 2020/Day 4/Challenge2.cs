using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        private static readonly string[] validEyeColors = {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};

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
            if (!(props.Count == 8 || props.Count == 7 && !props.ContainsKey("cid")))
            {
                return false;
            }

            string byr = props["byr"];
            if (int.TryParse(byr, out int i))
            {
                if (i < 1920 || i > 2002)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string iyr = props["iyr"];
            if (int.TryParse(iyr, out int j))
            {
                if (j < 2010 || j > 2020)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string eyr = props["eyr"];
            if (int.TryParse(eyr, out int k))
            {
                if (k < 2020 || k > 2030)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string hgt = props["hgt"];
            if (hgt.Length < 3)
            {
                return false;
            }
            string unit = hgt.Substring(hgt.Length - 2);
            if (unit.Equals("in") || unit.Equals("cm"))
            {
                if (int.TryParse(hgt[0..^2], out int l))
                {
                    if (unit.Equals("cm") && (l < 150 || l > 193))
                    {
                        return false; // not in cm range
                    }

                    if (unit.Equals("in") && (l < 59 || l > 76))
                    {
                        return false; // not in in range
                    }
                }
                else
                {
                    return false; // cant parse height
                }
            }
            else
            {
                return false; // does not include a unit
            }
            
            string hcl = props["hcl"];
            if (hcl[0] != '#')
            {
                return false;
            }

            if (!int.TryParse(hcl.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            {
                return false;
            }
            
            string ecl = props["ecl"];
            if (!validEyeColors.Contains(ecl))
            {
                return false;
            }

            string pid = props["pid"];
            if (pid.Length != 9)
            {
                return false;
            }

            return true;
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
