using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            char[,] matrix = ParseInputToMatrix(lines, out int maxX, out int maxY);
            char[,] initialMatrix = Init(matrix, maxX, maxY);

            char[,] latestRound = PerformRound(initialMatrix, maxX, maxY, out bool keepRolling);
            //PrettyPrintMatrix(latestRound, maxX + 2, maxY + 2);
            while (keepRolling)
            {
                latestRound = PerformRound(latestRound, maxX, maxY, out keepRolling);
                //PrettyPrintMatrix(latestRound, maxX + 2, maxY + 2);
            }

            Console.WriteLine($"Count of occupied seats: {CountSeats(latestRound, maxX, maxY)}");
        }

        static char[,] Init(char[,] matrix, int maxX, int maxY)
        {
            // Border Padding of 1
            char[,] newMatrix = new char[maxX + 2, maxY + 2];

            for (int y = 0; y < maxY + 2; y++)
            {
                for (int x = 0; x < maxX + 2; x++)
                {
                    if ((x >= 1 && x <= maxX) && (y >= 1 && y <= maxY))
                    {
                        char c = matrix[x - 1, y - 1];

                        newMatrix[x, y] = c;
                    }
                    else
                    {
                        newMatrix[x, y] = '.';
                    }
                }
            }

            return newMatrix;
        }

        static char[,] PerformRound(char[,] matrix, int maxX, int maxY, out bool hasAnythingChanged)
        {
            hasAnythingChanged = false;

            // Border Padding of 1
            char[,] newMatrix = new char[maxX + 2, maxY + 2];

            for (int y = 0; y < maxY + 2; y++)
            {
                for (int x = 0; x < maxX + 2; x++)
                {
                    char c = matrix[x, y];

                    if ((x >= 1 && x <= maxX) && (y >= 1 && y <= maxY) && c != '.')
                    {
                        int neighbours = CountNeighboursForPosition(matrix, x, y);

                        if (c == '#' && neighbours >= 4)
                        {
                            newMatrix[x, y] = 'L';
                            hasAnythingChanged = true;
                        }
                        else if (c == 'L' && neighbours == 0)
                        {
                            newMatrix[x, y] = '#';
                            hasAnythingChanged = true;
                        }
                        else
                        {
                            newMatrix[x, y] = matrix[x, y];
                        }
                    }
                    else
                    {
                        newMatrix[x, y] = '.';
                    }
                }
            }

            return newMatrix;
        }

        static int CountNeighboursForPosition(char[,] matrix, int x, int y)
        {
            int amountOfNeighbours = 0;

            for (int dy = -1; dy < 2; dy++)
            {
                for (int dx = -1; dx < 2; dx++)
                {
                    if (dy == 0 && dx == 0)
                    {
                        continue;
                    }

                    char c = matrix[x - dx, y + dy];
                    if (c == '#')
                    {
                        amountOfNeighbours++;
                    }
                }
            }

            return amountOfNeighbours;
        }

        static long CountSeats(char[,] matrix, int maxX, int maxY)
        {
            long count = 0;
            for (int y = 0; y < maxY + 2; y++)
            {
                for (int x = 0; x < maxX + 2; x++)
                {
                    if (matrix[x, y] == '#')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static char[,] ParseInputToMatrix(string[] lines, out int maxX, out int maxY)
        {
            maxX = lines[0].Length;
            maxY = lines.Length;
            char[,] matrix = new char[maxX, maxY];

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    matrix[x, y] = lines[y][x];
                }
            }

            return matrix;
        }

        private static void PrettyPrintMatrix(char[,] matrix, int maxX, int maxY)
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Console.Write(matrix[x, y]);
                }

                Console.WriteLine();
            }
        }
    }
}
