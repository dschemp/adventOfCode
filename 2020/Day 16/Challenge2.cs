using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            List<RuleSet> rules = ParseRules(lines);

            int[] yourTicket = RetrieveYourTicketInfo(lines);
            IEnumerable<int[]> nearbyTickets = RetrieveNearbyTicketInfo(lines);

            List<int[]> validTickets = FilterOutInvalidTickets(nearbyTickets, rules).ToList();
            validTickets.Add(yourTicket);
            List<RuleSet> positionalMappingOfRules = DetermineRuleMapping(validTickets, rules);

            PrettyPrint(yourTicket, positionalMappingOfRules);
            long solution = RetrieveSolution(yourTicket, positionalMappingOfRules);

            Console.WriteLine($"Final solution is {solution}");
        }

        private static void PrettyPrint(int[] ticket, List<RuleSet> rules)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                Console.WriteLine($"[{i}] {rules[i].Name}: {ticket[i]}");
            }

            Console.WriteLine();
        }

        private static long RetrieveSolution(int[] yourTicket, List<RuleSet> positionalMappingOfRules)
        {
            long result = 1;
            for (int pos = 0; pos < positionalMappingOfRules.Count; pos++)
            {
                if (positionalMappingOfRules[pos].Name.Contains("departure"))
                {
                    result *= yourTicket[pos];
                }
            }

            return result;
        }

        private static List<RuleSet> DetermineRuleMapping(IEnumerable<int[]> validTickets, List<RuleSet> rules)
        {
            Dictionary<int, List<RuleSet>> ruleMapping = new Dictionary<int, List<RuleSet>>();

            foreach (var validTicket in validTickets)
            {
                for (int i = 0; i < validTicket.Length; i++)
                {
                    IEnumerable<RuleSet> validRule = rules.Where(r => r.CheckValidity(validTicket[i]));

                    if (!ruleMapping.ContainsKey(i))
                    {
                        ruleMapping.Add(i, new List<RuleSet>());
                        ruleMapping[i].AddRange(validRule);
                    }
                    else
                    {
                        RuleSet[] commonRules = ruleMapping[i].Where(r => validRule.Contains(r)).ToArray();
                        ruleMapping[i].Clear();
                        ruleMapping[i].AddRange(commonRules);
                    }
                }
            }

            for (int i = 1; i <= ruleMapping.Count; i++)
            {
                List<RuleSet> fixedRules = ruleMapping
                    .Where(kvp => kvp.Value.Count == 1)
                    .Select(kvp => kvp.Value)
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();

                for (int j = 0; j < ruleMapping.Count; j++)
                {
                    if (ruleMapping[j].Count != 1)
                    {
                        ruleMapping[j].RemoveAll(r => fixedRules.Contains(r));
                    }
                }
            }

            List<RuleSet> outputRules = new List<RuleSet>();

            foreach (var rm in ruleMapping)
            {
                outputRules.Add(rm.Value.First());
            }
            
            return outputRules;
        }

        static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetCombinations(list, length - 1)
                .SelectMany(t => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        static IEnumerable<int[]> FilterOutInvalidTickets(IEnumerable<int[]> tickets, List<RuleSet> rules)
        {
            foreach (var ticket in tickets)
            {
                if (CheckTicketValidity(ticket, rules, out _))
                {
                    yield return ticket;
                }
            }
        }

        static bool CheckTicketValidity(int[] ticket, List<RuleSet> rules, out int errorValue)
        {
            errorValue = 0;
            if (ticket.Length != rules.Count)
            {
                return false;
            }

            foreach (var parameter in ticket)
            {
                if (!rules.Any(r => r.CheckValidity(parameter)))
                {
                    errorValue = parameter;
                    return false;
                }
            }

            return true;
        }

        static int[] ParseTicket(string line)
        {
            return line.Split(',').Select(int.Parse).ToArray();
        }

        private static int[] RetrieveYourTicketInfo(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("your ticket"))
                {
                    return ParseTicket(lines[i + 1]);
                }
            }

            return null;
        }

        static IEnumerable<int[]> RetrieveNearbyTicketInfo(string[] lines)
        {
            int startIndex = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("nearby"))
                {
                    startIndex = i;
                    break;
                }
            }

            if (startIndex == -1)
            {
                yield break;
            }

            IEnumerable<string> nearbyTicketStrings = lines.Skip(startIndex + 1);
            foreach (var nearbyTicketString in nearbyTicketStrings)
            {
                yield return ParseTicket(nearbyTicketString);
            }
        }

        static List<RuleSet> ParseRules(string[] lines)
        {
            List<RuleSet> rules = new List<RuleSet>();
            Regex fieldPropertyRegex = new Regex(@"(.*): (\d+)-(\d+) or (\d+)-(\d+)");

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = fieldPropertyRegex.Match(lines[i]);
                if (!match.Success || match.Groups.Count != 6)
                {
                    break;
                }

                RuleSet r = new RuleSet { Name = match.Groups[1].Value };
                r.AddRangeInclusive(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                r.AddRangeInclusive(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));
                rules.Add(r);
            }

            return rules;
        }
    }

    class RuleSet
    {
        private readonly Dictionary<int, int> _ranges;

        public string Name { get; set; }

        public RuleSet()
        {
            _ranges = new Dictionary<int, int>();
        }

        public void AddRangeInclusive(int a, int b)
        {
            _ranges.Add(a, b);
        }

        public bool CheckValidity(int l)
        {
            return _ranges.Any(kvp => kvp.Key <= l && l <= kvp.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is RuleSet r)
            {
                return r.Name.Equals(Name);
            }

            return false;
        }

        public override string ToString() => Name;
    }
}
