using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            IEnumerable<Instruction> instructions = lines.Select(s => new Instruction()
            {
                OpCode = s[0],
                Parameter = int.Parse(s[1..])
            });

            Navigation nav = new Navigation();
            Console.WriteLine(nav);
            foreach (var instruction in instructions)
            {
                Console.WriteLine();
                nav.Perform(instruction);
                Console.WriteLine(nav);
            }

            Console.WriteLine($"Manhattan Distance from Origin: {nav.ManhattanDistanceFromOrigin}");
        }
    }

    class Instruction
    {
        public char OpCode { get; set; }

        public int Parameter { get; set; }
    }

    class Navigation
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        private int _rotation = 90;

        public int Rotation
        {
            get => _rotation;
            private set
            {
                if (value < 0)
                {
                    value += 360;
                }
                _rotation = value % 360;
            }
        }

        public void Perform(Instruction ins)
        {
            Perform(ins.OpCode, ins.Parameter);
        }

        void Perform(char operation, int parameter)
        {
            Console.WriteLine($"Performing {operation}{parameter}");
            switch (operation)
            {
                case 'N':
                    Y += parameter;
                    break;

                case 'S':
                    Y -= parameter;
                    break;

                case 'E':
                    X += parameter;
                    break;

                case 'W':
                    X -= parameter;
                    break;

                case 'L':
                    Rotation -= parameter;
                    break;

                case 'R':
                    Rotation += parameter;
                    break;

                case 'F':
                    GoForward(parameter);
                    break;
            }
        }

        void GoForward(int parameter)
        {
            switch (Rotation)
            {
                case 0:
                    Perform('N', parameter);
                    break;

                case 90:
                    Perform('E', parameter);
                    break;

                case 180:
                    Perform('S', parameter);
                    break;

                case 270:
                    Perform('W', parameter);
                    break;
            }
        }

        public int ManhattanDistanceFromOrigin => Math.Abs(X) + Math.Abs(Y);

        public override string ToString() => $"X:{X} | Y:{Y} | Rot:{Rotation} | Dis:{ManhattanDistanceFromOrigin}";
    }
}
