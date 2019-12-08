using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static int width = 25, height = 6;

        static void Main(string[] args)
        {
            int[] input = File.ReadAllText(@"input.txt")
                .ToCharArray()
                .Select(s => int.Parse(s.ToString()))
                .ToArray();

            int count = input.Length;
            int countPerLayers = width * height;
            int layers = count / countPerLayers;

            int[,,] picture = new int[width, height, layers];

            FillArray(picture, countPerLayers, input);

            var p = GetPicture(picture, width, height, layers);
            PrettyPrint(p, width, height);
        }

        private static int GetCount(int[,,] picture, int countPerLayers, int layer, int v)
        {
            int count = 0;
            for (int i = 0; i < countPerLayers; i++)
            {
                int x = i % width;
                int y = i / width;

                if (picture[x, y, layer] == v)
                {
                    count++;
                }
            }

            return count;
        }

        private static int GetLayerWithFewestZeroes(int[,,] picture, int countPerLayers, int layers)
        {
            int? minLayer = null;
            int? minLayerCount = null;

            for (int z = 0; z < layers; z++)
            {
                int count = 0;
                for (int i = 0; i < countPerLayers; i++)
                {
                    int x = i % width;
                    int y = i / width;

                    if (picture[x, y, z] == 0)
                    {
                        count++;
                    }
                }

                if (minLayer == null || minLayerCount == null || count < minLayerCount)
                {
                    minLayer = z;
                    minLayerCount = count;
                }
            }

            return minLayer.Value;
        }

        static void FillArray(int[,,] picture, int countPerLayers, int[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int x = i % width;
                int y = (i / width) % height;
                int z = i / countPerLayers;

                picture[x, y, z] = input[i];
            }
        }

        static int GetPixelAtPosition(int[,,] picture, int x, int y, int layers)
        {
            for (int z = 0; z < layers; z++)
            {
                if (picture[x, y, z] != 2)
                    return picture[x, y, z];
            }

            return 2;
        }

        static int[,] GetPicture(int[,,] picture, int width, int height, int layers)
        {
            int[,] output = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    output[x, y] = GetPixelAtPosition(picture, x, y, layers);
                }
            }

            return output;
        }

        static void PrettyPrint(int[,] picture, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int p = picture[x, y];

                    if (p == 1) // white
                        Console.Write('░');
                    else
                        Console.Write('▓');
                }
                Console.WriteLine();
            }
        }
    }
}
