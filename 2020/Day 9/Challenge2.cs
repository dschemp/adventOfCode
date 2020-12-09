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

            List<long> numbers = ConvertToNumberStream(lines).ToList();
            int lengthOfPreamble = 25;

            for (int i = 0; i < numbers.Count(); i++)
            {
                long num = numbers.ElementAt(i);

                IEnumerable<long> _scope = GetScopeForElementAt(i, numbers, lengthOfPreamble);
                List<long> scope = _scope == null ? new List<long>() : _scope.ToList();
                
                bool canBeMadeUpByElementsInScope = CanNumberBeMadeUpOfElementsInScope(num, scope);
                
                Console.WriteLine($"{num} | Is creatable: {canBeMadeUpByElementsInScope}");
                if (i >= lengthOfPreamble && !canBeMadeUpByElementsInScope)
                {
                    List<long> searchSlice = numbers.Take(i - 1).ToList();
                    long encryptionWeakness = FindEncryptionWeakness(num, searchSlice);

                    Console.WriteLine($"Found weakness: {encryptionWeakness}");
                    return;
                }
            }
        }

        static IEnumerable<long> ConvertToNumberStream(string[] lines)
        {
            foreach (var line in lines)
            {
                yield return long.Parse(line);
            }
        }

        static IEnumerable<long> GetScopeForElementAt(int index, IEnumerable<long> numbers, int lengthOfPreamble)
        {
            if (index < lengthOfPreamble)
            {
                return null;
            }

            return numbers.Skip(index - lengthOfPreamble).Take(lengthOfPreamble);
        }

        static bool CanNumberBeMadeUpOfElementsInScope(long num, List<long> scope)
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

        static long FindEncryptionWeakness(long num, List<long> scope)
        {
            for (int i = 0; i < scope.Count; i++)
            {
                List<long> slice = scope.Skip(i).ToList();
                for (int j = 2; j < slice.Count; j++)
                {
                    List<long> subslice = slice.Take(j).ToList();

                    long sum = subslice.Sum();
                    if (sum > num)
                    {
                        break;
                    }
                    
                    if (sum == num)
                    {
                        long min = subslice.Min();
                        long max = subslice.Max();
                        return min + max;
                    }
                }
            }

            return -1;
        }
    }
}
