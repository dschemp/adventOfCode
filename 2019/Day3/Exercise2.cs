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

            int?[,,] cableTable = new int?[width + 2 * width_offset, height + 2 * height_offset, 2];

            int idx = 0;
            foreach (var cable in cables)
            {
                InsertCable(ref cableTable, cable, start_x, start_y, idx++);
            }

            cableTable[start_x, start_y, 0] = 0;
            cableTable[start_x, start_y, 1] = 0;

            //PrettyPrintToFile(ref cableTable, width + 2 * width_offset, height + 2 * height_offset);

            //Console.WriteLine($"MH Distance: {GetTaxicabDistance(cableTable, width + 2 * width_offset, height + 2 * height_offset, start_x, start_y)}");
            Console.WriteLine($"Min Distance: {GetMinDistance(cableTable, width + 2 * width_offset, height + 2 * height_offset, start_x, start_y).Value}");
        }

        #region Distance

        private static int? GetMinDistance(int?[,,] cableTable, int width, int height, int x, int y)
        {
            var locations = GetLocations(cableTable, width, height).ToArray();
            int? min = null;

            foreach (var location in locations)
            {
                if (location.x == x && location.y == y)
                    continue;
                if (min == null)
                    min = GetDistance(location, x, y, cableTable);
                else
                {
                    int distance = GetDistance(location, x, y, cableTable).Value;
                    min = Math.Min(min.Value, distance);
                }
            }

            return min;
        }

        private static int? GetDistance((int x, int y) location, int x, int y, int?[,,] cableTable)
        {
            int? n1 = cableTable[location.x, location.y, 0];
            int? n2 = cableTable[location.x, location.y, 1];

            return n1 + n2;
        }

        private static IEnumerable<(int x, int y)> GetLocations(int?[,,] cableTable, int width, int height)
        {
            // TODO: Parallelism
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (cableTable[x, y, 0] != null && cableTable[x, y, 1] != null)
                        yield return (x, y);
                }
            }
        }

        #endregion

        #region Building

        private static void InsertCable(ref int?[,,] cableTable, List<(char direction, int steps)> list, int x_start, int y_start, int indic)
        {
            int ptr_x = x_start, ptr_y = y_start;
            int steps = 0;

            foreach (var instr in list)
            {
                (ptr_x, ptr_y) = DrawLine(ref cableTable, instr, ptr_x, ptr_y, indic, ref steps);
            }
        }

        private static (int ptr_x, int ptr_y) DrawLine(ref int?[,,] cableTable, (char direction, int steps) instr, int ptr_x, int ptr_y, int indicator, ref int steps)
        {
            int delta_x = instr.direction == 'R' ? 1 : instr.direction == 'L' ? -1 : 0;
            int delta_y = instr.direction == 'U' ? 1 : instr.direction == 'D' ? -1 : 0;

            int new_ptr_x = ptr_x + delta_x * instr.steps;
            int new_ptr_y = ptr_y + delta_y * instr.steps;

            if (delta_x != 0)
            {
                if (delta_x == -1)
                {
                    // L
                    for (int x = ptr_x - 1; x >= ptr_x - instr.steps; x--)
                    {
                        steps++;
                        int? c = cableTable[x, ptr_y, indicator];
                        if (c == null)
                            cableTable[x, ptr_y, indicator] = steps;
                    }
                }
                else
                {
                    // R
                    for (int x = ptr_x + 1; x <= ptr_x + instr.steps; x++)
                    {
                        steps++;
                        int? c = cableTable[x, ptr_y, indicator];
                        if (c == null)
                            cableTable[x, ptr_y, indicator] = steps;
                    }
                }
            }

            if (delta_y != 0)
            {
                if (delta_y == -1)
                {
                    // D
                    for (int y = ptr_y - 1; y >= ptr_y - instr.steps; y--)
                    {
                        steps++;
                        int? c = cableTable[ptr_x, y, indicator];
                        if (c == null)
                            cableTable[ptr_x, y, indicator] = steps;
                    }
                }
                else
                {
                    // U
                    for (int y = ptr_y + 1; y <= ptr_y + instr.steps; y++)
                    {
                        steps++;
                        int? c = cableTable[ptr_x, y, indicator];
                        if (c == null)
                            cableTable[ptr_x, y, indicator] = steps;
                    }
                }
            }

            return (new_ptr_x, new_ptr_y);
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
