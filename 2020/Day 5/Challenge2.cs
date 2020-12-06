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

            IEnumerable<Seat> seats = lines.Select(ParseSeating).ToList();

            IEnumerable<Seat> orderedSeats = seats?.OrderBy(s => s.SeatId).ToList();
            
            for (int i = 0; i < orderedSeats.Count() - 2; i++)
            {
                int lastSeatId = orderedSeats.ElementAt(i).SeatId;
                int nextSeatId = orderedSeats.ElementAt(i + 1).SeatId;

                if (nextSeatId - lastSeatId == 2)
                {
                    Console.WriteLine($"Found candidate: {lastSeatId + 1}");
                }
            }
        }

        static Seat ParseSeating(string seat)
        {
            int row = GenerateRow(seat);
            int col = GenerateColumn(seat);

            return new Seat() {Column = col, Row = row};
        }

        static int GenerateRow(string seat)
        {
            string row = seat.Substring(0, 7);
            int rowNumber = 0;

            foreach (char r in row)
            {
                rowNumber <<= 1;
                if (r == 'B')
                {
                    rowNumber |= 1;
                }
            }

            return rowNumber;
        }

        static int GenerateColumn(string seat)
        {
            string col = seat.Substring(7);
            int colNumber = 0;

            foreach (char c in col)
            {
                colNumber <<= 1;
                if (c == 'R')
                {
                    colNumber |= 1;
                }
            }

            return colNumber;
        }
    }

    class Seat
    {
        public int Row { get; set; }

        public int Column { get; set; }

        private int? seatId;

        public int SeatId => seatId ??= Row * 8 + Column;
    }
}
