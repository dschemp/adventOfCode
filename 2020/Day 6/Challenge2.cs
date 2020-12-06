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

            IEnumerable<string> cleanedInput = CleanUpInput(lines).ToList();
            IEnumerable<string> cleanedGroupInput = CombineGroupAnswers(cleanedInput);

            int sum = cleanedGroupInput.Sum(s => s.Length);
            Console.WriteLine($"Sum of all questions answered with yes: {sum}");
        }

        static IEnumerable<string> CleanUpInput(string[] lines)
        {
            string buffer = string.Empty;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) && !string.IsNullOrWhiteSpace(buffer))
                {
                    yield return buffer;
                    buffer = string.Empty;
                }
                else
                {
                    buffer = string.Join("|", line?.Trim(), buffer.Trim()).Trim('|');
                }
            }

            yield return buffer;
        }

        static IEnumerable<string> CombineGroupAnswers(IEnumerable<string> groups)
        {
            foreach (var group in groups)
            {
                string buffer = string.Empty;
                string[] individualAnswers = group.Split('|').ToArray();

                foreach (var c in individualAnswers[0])
                {
                    if (individualAnswers.All(t => t.Contains(c)))
                    {
                        if (!buffer.Contains(c))
                        {
                            buffer += c;
                        }
                    }
                }

                yield return buffer;
            }
        }
    }
}
