using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

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

            bool isValidTicket = CheckTicketValidity(yourTicket, rules, out _);

            long ticketScanningErrorRate = GetTicketScanningErrorRate(nearbyTickets, rules);

            Console.WriteLine($"The ticket scanning error rate for the nearby tickets is {ticketScanningErrorRate}");
        }

        static long GetTicketScanningErrorRate(IEnumerable<int[]> tickets, List<RuleSet> rules)
        {
            long result = 0;

            foreach (var ticket in tickets)
            {
                _ = CheckTicketValidity(ticket, rules, out int errorValue);
                result += errorValue;
            }

            return result;
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
            Regex fieldPropertyRegex = new Regex(@"(\d+)-(\d+) or (\d+)-(\d+)");

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = fieldPropertyRegex.Match(lines[i]);
                if (!match.Success || match.Groups.Count != 5)
                {
                    break;
                }
                
                RuleSet r = new RuleSet();
                r.AddRangeInclusive(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                r.AddRangeInclusive(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
                rules.Add(r);
            }

            return rules;
        }
    }

    class RuleSet
    {
        private readonly Dictionary<int, int> _ranges;

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
    }
}
