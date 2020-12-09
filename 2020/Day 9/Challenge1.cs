using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            List<ulong> numbers = ConvertToNumberStream(lines).ToList();
            int lengthOfPreamble = 25;

            for (int i = 0; i < numbers.Count(); i++)
            {
                IEnumerable<ulong> _scope = GetScopeForElementAt(i, numbers, lengthOfPreamble);
                List<ulong> scope = _scope == null ? new List<ulong>() : _scope.ToList();
                
                string scopeText = string.Join(", ", scope);
                bool canBeMadeUpByElementsInScope = CanNumberBeMadeUpOfElementsInScope(numbers.ElementAt(i), scope);
                
                Console.WriteLine($"{numbers.ElementAt(i)} | Is creatable: {canBeMadeUpByElementsInScope}");
                if (i >= lengthOfPreamble && !canBeMadeUpByElementsInScope)
                {
                    return;
                }
            }
        }

        static IEnumerable<ulong> ConvertToNumberStream(string[] lines)
        {
            foreach (var line in lines)
            {
                yield return ulong.Parse(line);
            }
        }

        static IEnumerable<ulong> GetScopeForElementAt(int index, IEnumerable<ulong> numbers, int lengthOfPreamble)
        {
            if (index < lengthOfPreamble)
            {
                return null;
            }

            return numbers.Skip(index - lengthOfPreamble).Take(lengthOfPreamble);
        }

        static bool CanNumberBeMadeUpOfElementsInScope(ulong num, List<ulong> scope)
        {
            foreach (var s1 in scope)
            {
                foreach (var s2 in scope)
                {
                    if (s1 + s2 == num)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
