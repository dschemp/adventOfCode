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

            Console.WriteLine(IsValidPassword("111111"));
            Console.WriteLine(IsValidPassword("223450"));
            Console.WriteLine(IsValidPassword("123789"));
        }

        private static IEnumerable<string> GetPasswords(string input)
        {
            var parts = input.Split('-').ToArray();
            int min = int.Parse(parts[0]), max = int.Parse(parts[1]);

            for (int pw = min; pw < max + 1; pw++)
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

            for (int i = 0; i < pw.Length - 1; i++)
            {
                if (pw[i] == pw[i + 1])
                {
                    int count = 0;
                    while (pw[count++] != pw[i]) ;
                    if (count % 2 != 0)
                        return false;

                    doubleDigitDetected = true;
                }
            }

            return doubleDigitDetected;
        }

        private int GetAmountOfChars(char c, string str)
        {
            int count = 0;
            foreach (var s in str)
            {
                if (s == c)
                    count++;
            }

            return count;
        }
    }
}
