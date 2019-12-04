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
            string[] lines = File.ReadLines(@"input.txt").ToArray();
            var cables = lines.Select(ParseList).ToArray();
            var dim = GetDimensions(cables);

            Console.WriteLine($"(X: [{dim.min_x}, {dim.max_x}] | Y: [{dim.min_y}, {dim.max_y}])");

            var width = Math.Abs(dim.min_x) + Math.Abs(dim.max_x);
            var height = Math.Abs(dim.min_y) + Math.Abs(dim.max_y);

            int width_offset = 5, height_offset = 5;

            int start_x = Math.Abs(dim.min_x) + width_offset, start_y = Math.Abs(dim.min_y) + height_offset;

            char?[,] cableTable = new char?[width + 2 * width_offset, height + 2 * height_offset];

            char[] indic = new char[] { 'A', 'B', 'C' };
            int idx = 0;
            foreach (var cable in cables)
            {
                InsertCable(ref cableTable, cable, start_x, start_y, indic[idx++]);
            }

            cableTable[start_x, start_y] = 'o';

            //PrettyPrintToFile(ref cableTable, width + 2 * width_offset, height + 2 * height_offset);

            Console.WriteLine($"MH Distance: {GetTaxicabDistance(cableTable, width + 2 * width_offset, height + 2 * height_offset, start_x, start_y)}");
        }

        #region Distance

        private static int? GetTaxicabDistance(char?[,] cableTable, int width, int height, int x, int y)
        {
            var locations = GetLocations(cableTable, width, height);
            int? min = null;

            foreach (var location in locations)
            {
                if (min == null)
                    min = GetDistance(location, x, y);
                else
                {
                    min = Math.Min(min.Value, GetDistance(location, x, y));
                }
            }

            return min;
        }

        private static int GetDistance((int x, int y) location, int x, int y)
        {
            int delta_x = Math.Abs(x - location.x);
            int delta_y = Math.Abs(y - location.y);

            return delta_x + delta_y;
        }

        private static IEnumerable<(int x, int y)> GetLocations(char?[,] cableTable, int width, int height)
        {
            // TODO: Parallelism
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (cableTable[x, y] == 'x')
                        yield return (x, y);
                }
            }
        }

        #endregion

        #region Building

        private static void InsertCable(ref char?[,] cableTable, List<(char direction, int steps)> list, int x_start, int y_start, char c)
        {
            int ptr_x = x_start, ptr_y = y_start;

            foreach (var instr in list)
            {
                (ptr_x, ptr_y) = DrawLine(ref cableTable, instr, ptr_x, ptr_y, c);
            }
        }

        private static (int ptr_x, int ptr_y) DrawLine(ref char?[,] cableTable, (char direction, int steps) instr, int ptr_x, int ptr_y, char indicator)
        {
            int delta_x = instr.direction == 'R' ? 1 : instr.direction == 'L' ? -1 : 0;
            int delta_y = instr.direction == 'U' ? 1 : instr.direction == 'D' ? -1 : 0;

            char lineChar = indicator;

            int new_ptr_x = ptr_x + delta_x * instr.steps;
            int new_ptr_y = ptr_y + delta_y * instr.steps;

            if (delta_x != 0)
            {
                if (delta_x == -1)
                {
                    // L
                    for (int x = ptr_x; x >= ptr_x - instr.steps; x--)
                    {
                        char? c = cableTable[x, ptr_y];
                        if (c == null)
                            cableTable[x, ptr_y] = lineChar;
                        else if (c != indicator && c != '+')
                            cableTable[x, ptr_y] = 'x';
                    }
                }
                else
                {
                    // R
                    for (int x = ptr_x; x <= ptr_x + instr.steps; x++)
                    {
                        char? c = cableTable[x, ptr_y];
                        if (c == null)
                            cableTable[x, ptr_y] = lineChar;
                        else if (c != indicator && c != '+')
                            cableTable[x, ptr_y] = 'x';
                    }
                }
            }

            if (delta_y != 0)
            {
                if (delta_y == -1)
                {
                    // D
                    for (int y = ptr_y; y >= ptr_y - instr.steps; y--)
                    {
                        char? c = cableTable[ptr_x, y];
                        if (c == null)
                            cableTable[ptr_x, y] = lineChar;
                        else if (c != indicator && c != '+')
                            cableTable[ptr_x, y] = 'x';
                    }
                }
                else
                {
                    // U
                    for (int y = ptr_y; y <= ptr_y + instr.steps; y++)
                    {
                        char? c = cableTable[ptr_x, y];
                        if (c == null)
                            cableTable[ptr_x, y] = lineChar;
                        else if (c != indicator && c != '+')
                            cableTable[ptr_x, y] = 'x';
                    }
                }
            }

            cableTable[new_ptr_x, new_ptr_y] = '+';

            return (new_ptr_x, new_ptr_y);
        }
        
        
        #endregion

        #region Pretty Print

        private static void PrettyPrintToFile(ref char?[,] cableTable, int width, int height)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = cableTable[x, y] ?? '.';
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            File.WriteAllText(@"prettyprint.txt", sb.ToString());
        }

        #endregion

        #region Metainformation

        static (int min_x, int max_x, int min_y, int max_y) GetDimensions(IEnumerable<List<(char, int)>> cables)
        {
            int min_x = 0,
                max_x = 0,
                min_y = 0,
                max_y = 0;

            foreach (var cable in cables)
            {
                var (_min_x, _max_x) = CalculateMinMax(cable, 'R', 'L');
                var (_min_y, _max_y) = CalculateMinMax(cable, 'U', 'D');

                min_x = Math.Min(min_x, _min_x);
                max_x = Math.Max(max_x, _max_x);
                min_y = Math.Min(min_y, _min_y);
                max_y = Math.Max(max_y, _max_y);
            }

            return (min_x, max_x, min_y, max_y);
        }

        static List<(char direction, int steps)> ParseList(string line)
        {
            var output = new List<(char, int)>();

            foreach (var item in line.Split(','))
            {
                output.Add((item[0], int.Parse(item.Substring(1))));
            }

            return output;
        }

        static (int min, int max) CalculateMinMax(List<(char direction, int steps)> instructions, char direction_plus, char direction_minus)
        {
            int path = 0;
            int min = 0, max = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.direction != direction_plus && instruction.direction != direction_minus)
                    continue;

                if (instruction.direction == direction_plus)
                    path += instruction.steps;
                else
                    path -= instruction.steps;

                min = Math.Min(min, path);
                max = Math.Max(max, path);
            }

            return (min, max);
        }

        #endregion
    }
}
