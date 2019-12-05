using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "172851-675869";

            var possibleValues = GetPasswords(input);
            Console.WriteLine(possibleValues.ToArray().Length);

            // Test Cases
            Console.WriteLine(IsValidPassword("112233"));
            Console.WriteLine(IsValidPassword("123444"));
            Console.WriteLine(IsValidPassword("111122"));
            Console.WriteLine(IsValidPassword("000133"));
        }

        private static IEnumerable<string> GetPasswords(string input)
        {
            var parts = input.Split('-').ToArray();
            int min = int.Parse(parts[0]), max = int.Parse(parts[1]);

            for (int pw = min; pw < max; pw++)
            {
                if (IsValidPassword(pw.ToString()))
                    yield return pw.ToString();
            }
        }

        private static bool IsValidPassword(string pw)
        {
            for (int i = 0; i < pw.Length - 1; i++)
            {
                int n1 = Convert.ToInt32(pw[i].ToString());
                int n2 = Convert.ToInt32(pw[i + 1].ToString());
                if (n1 <= n2)
                    continue;
                else
                    return false;
            }

            bool doubleDigitDetected = false;

            int idx = 0;

            while (idx < pw.Length - 1)
            {
                if (pw[idx] == pw[idx + 1])
                {
                    int count = 0;
                    int i = idx;
                    while (pw[i + count] == pw[idx])
                    {
                        count++;
                        if (i + count >= pw.Length)
                            break;
                    }

                    if (count == 2)
                    {
                        doubleDigitDetected = true;
                    }
                    idx += count - 1;

                }
                else
                    idx++;
            }

            return doubleDigitDetected;
        }
    }
}
