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
            string[] lines = File.ReadAllLines("input.txt");

            IEnumerable<Password> passwords = lines.Select(Password.CreateFromLine);

            int validPws = 0;
            foreach (var password in passwords)
            {
                if (password.CheckCondition())
                {
                    validPws++;
                }
            }

            Console.WriteLine($"Valid Passwords: {validPws}");
        }
    }

    class Password
    {
        public int MinimumAmount { get; set; }

        public int MaximumAmount { get; set; }

        public char Character { get; set; }

        public string PasswordString { get; set; }

        public static Password CreateFromLine(string line)
        {
            Password pw = new Password();

            string[] parts = line.Split(':');
            pw.PasswordString = parts[1].Trim();
            string condition = parts[0];

            string range = condition.Substring(0, condition.Length - 2);
            string[] ranges = range.Split('-');

            pw.MinimumAmount = int.Parse(ranges[0]);
            pw.MaximumAmount = int.Parse(ranges[1]);

            pw.Character = condition.Last();

            return pw;
        }

        public bool CheckCondition()
        {
            int amountOfCharacters = 0;
            foreach (var c in PasswordString)
            {
                if (c == Character)
                {
                    amountOfCharacters++;
                }
            }

            return (amountOfCharacters <= MaximumAmount && amountOfCharacters >= MinimumAmount);
        }
    }
}
