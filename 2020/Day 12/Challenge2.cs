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
        public HeadingCoordinates ShipHeadingCoordinates { get; set; }

        public HeadingCoordinates RelativeWaypoint { get; set; }

        public Navigation()
        {
            ShipHeadingCoordinates = new HeadingCoordinates();
            RelativeWaypoint = new HeadingCoordinates { X = 10, Y = 1 };
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
                    RelativeWaypoint.Y += parameter;
                    break;

                case 'S':
                    RelativeWaypoint.Y -= parameter;
                    break;

                case 'E':
                    RelativeWaypoint.X += parameter;
                    break;

                case 'W':
                    RelativeWaypoint.X -= parameter;
                    break;

                case 'L':
                case 'R':
                    Rotate(operation, parameter);
                    break;

                case 'F':
                    GoForward(parameter);
                    break;
            }
        }

        void Rotate(char direction, int degrees)
        {
            if (direction == 'R')
            {
                Rotate('L', 360 - degrees);
                return;
            }

            int oldX = RelativeWaypoint.X;
            int oldY = RelativeWaypoint.Y;

            // Left turns
            if (degrees == 90)
            {
                RelativeWaypoint.X = -oldY;
                RelativeWaypoint.Y = oldX;
            }
            else if (degrees == 180)
            {
                RelativeWaypoint.X = -oldX;
                RelativeWaypoint.Y = -oldY;
            }
            else if (degrees == 270)
            {
                RelativeWaypoint.X = oldY;
                RelativeWaypoint.Y = -oldX;
            }
        }

        void GoForward(int parameter)
        {
            ShipHeadingCoordinates.X += parameter * RelativeWaypoint.X;
            ShipHeadingCoordinates.Y += parameter * RelativeWaypoint.Y;
        }

        public int ManhattanDistanceFromOrigin => Math.Abs(ShipHeadingCoordinates.X) + Math.Abs(ShipHeadingCoordinates.Y);

        public override string ToString() => $"SHIP: X:{ShipHeadingCoordinates.X} | Y:{ShipHeadingCoordinates.Y} | Rot:{ShipHeadingCoordinates.Rotation} | Dis:{ManhattanDistanceFromOrigin}\n" +
                                             $"WAYP: X:{RelativeWaypoint.X} | Y:{RelativeWaypoint.Y}";
    }

    class HeadingCoordinates
    {
        public int X { get; set; }

        public int Y { get; set; }

        private int _rotation = 90;

        public int Rotation
        {
            get => _rotation;
            set
            {
                if (value < 0)
                {
                    value += 360;
                }
                _rotation = value % 360;
            }
        }
    }
}
