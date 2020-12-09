using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            List<BagRule> bagMapping = new List<BagRule>();

            foreach (var line in lines)
            {
                var rule = ParseRule(line);
                bagMapping.Add(rule);
                //Console.WriteLine(rule.Bag.Name + ": " + string.Join('/', rule.ChildrenBags.Select(b => $"({b.Amount}x) {b.Name}")));
            }

            Bag shinyGoldBag = new Bag()
            {
                Name = "shiny gold bag"
            };

            BagRule shinyGoldBagRule = bagMapping.Find(s => s.Bag.Equals(shinyGoldBag));

            long shinyBagCanContainAmountOfBags = GetAmountOfBagsRecursively(shinyGoldBagRule, bagMapping);

            Console.WriteLine($"Amount of bags that a shiny gold bag can contain: {shinyBagCanContainAmountOfBags}");
        }

        public static long GetAmountOfBagsRecursively(BagRule parent, List<BagRule> ruleSet)
        {
            long amount = 0;
            foreach (var childBag in parent.ChildrenBags)
            {
                amount += childBag.Amount;

                BagRule childBagRule = ruleSet.Find(s => s.Bag.Equals(childBag));
                amount += childBag.Amount * GetAmountOfBagsRecursively(childBagRule, ruleSet);
            }

            return amount;
        }

        public static bool ContainsRecursively(BagRule parent, Bag search, List<BagRule> ruleSet)
        {
            if (parent.Bag.Equals(search))
            {
                return false;
            }

            if (parent.ChildrenBags.Contains(search))
            {
                return true;
            }

            foreach (var childBags in parent.ChildrenBags)
            {
                BagRule childBagRule = ruleSet.Find(s => s.Bag.Equals(childBags));
                if (ContainsRecursively(childBagRule, search, ruleSet))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<Token> ParseLine(string line)
        {
            string buffer = string.Empty;
            foreach (var c in line)
            {
                if (char.IsLetterOrDigit(c))
                {
                    buffer += c;
                }
                else if (!string.IsNullOrWhiteSpace(buffer))
                {
                    yield return new Token() { Value = buffer };
                    buffer = string.Empty;
                }
            }
        }

        public static BagRule ParseRule(string line)
        {
            IEnumerable<Token> tokens = ParseLine(line).ToList();

            Bag parent = new Bag();

            parent.Name = string.Join(' ', tokens.Take(3).Select(t => t)).Replace("bags", "bag");

            List<Bag> children = new List<Bag>();
            IEnumerable<Token> canContainBagTokens = tokens.Skip(4).ToList();

            int n = canContainBagTokens.Count() / 4;

            for (int i = 0; i < n; i++)
            {
                Bag b = new Bag
                {
                    Amount = int.Parse(canContainBagTokens.ElementAt(i * 4).Value),
                    Name = string.Join(' ', canContainBagTokens.Skip(i * 4 + 1).Take(3)).Replace("bags", "bag")
                };

                children.Add(b);
            }

            return new BagRule()
            {
                Bag = parent,
                ChildrenBags = children
            };
        }
    }

    class Bag
    {
        public int Amount { get; set; }

        public string Name { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Bag b)
            {
                return Name == b.Name;
            }

            return false;
        }

        protected bool Equals(Bag other)
        {
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }

    class BagRule
    {
        public Bag Bag { get; set; }

        public List<Bag> ChildrenBags { get; set; }
    }

    class Token
    {
        public string Value { get; set; }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            if (obj is Token t)
            {
                return string.Equals(Value, t.Value);
            }

            return false;
        }
    }
}
