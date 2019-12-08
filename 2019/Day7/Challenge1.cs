using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    class Program
    {
        static string input = "3,8,1001,8,10,8,105,1,0,0,21,46,67,76,97,118,199,280,361,442,99999,3,9,1002,9,3,9,101,4,9,9,102,3,9,9,1001,9,3,9,1002,9,2,9,4,9,99,3,9,102,2,9,9,101,5,9,9,1002,9,2,9,101,2,9,9,4,9,99,3,9,101,4,9,9,4,9,99,3,9,1001,9,4,9,102,2,9,9,1001,9,4,9,1002,9,5,9,4,9,99,3,9,102,3,9,9,1001,9,2,9,1002,9,3,9,1001,9,3,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,99";

        static void Main(string[] args)
        {
            var permutations = GetPermutations(Enumerable.Range(0, 5), 5);

            IEnumerable<int> maxPermution = null;
            int? maxOutput = null;

            foreach (var currentPermutation in permutations)
            {
                int[] outputs = new int[5] { 0, 0, 0, 0, 0 };
                int[] inputs = currentPermutation.ToArray();

                int[] mem = input.Split(',').Select(int.Parse).ToArray();

                int input_ptr = 0;
                int output_ptr = 0;
                int subsquent_input = 0;

                for (int i = 0; i < 5; i++)
                {
                    Run(ref mem, inputs, outputs, ref subsquent_input, ref input_ptr, ref output_ptr);
                }

                if (maxPermution == null || maxOutput == null || outputs[4] > maxOutput)
                {
                    maxPermution = inputs;
                    maxOutput = outputs[4];
                }
            }

            Console.WriteLine($"MAX: {string.Join(",", maxPermution)} | {maxOutput}");
        }

        static void Run(ref int[] mem, int[] inputs, int[] outputs, ref int subsequent_input, ref int input_ptr, ref int output_ptr)
        {
            int instruction, ptr = 0;

            bool firstInput = true;

            while ((instruction = mem[ptr]) != 99) // Check for the opcode
            {
                int ptr_n1 = mem[ptr + 1], ptr_n2 = mem[ptr + 2], ptr_n3 = mem[ptr + 3];
                int opmode_n1 = (instruction % 1000) / 100, opmode_n2 = (instruction % 10000) / 1000, opmode_n3 = instruction / 10000;

                int val1, val2, val3;

                switch (instruction % 100)
                {
                    case 1:
                        PrintDebug(mem, ptr, 4);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        mem[ptr_n3] = val1 + val2;
                        ptr += 4;
                        break;
                    case 2:
                        PrintDebug(mem, ptr, 4);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        mem[ptr_n3] = val1 * val2;
                        ptr += 4;
                        break;
                    case 3:
                        PrintDebug(mem, ptr, 2);
                        int input;
                        if (firstInput)
                            input = inputs[input_ptr++];
                        else
                            input = subsequent_input;
                        firstInput = !firstInput;
                        //Console.WriteLine($"-- IN:  {input} --");
                        mem[ptr_n1] = input;
                        ptr += 2;
                        break;
                    case 4:
                        PrintDebug(mem, ptr, 2);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        outputs[output_ptr++] = val1;
                        subsequent_input = val1;
                        //Console.WriteLine($"-- OUT: {subsequent_input} --");
                        ptr += 2;
                        break;
                    case 5:
                        PrintDebug(mem, ptr, 3);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        if (val1 != 0)
                            ptr = val2;
                        else
                            ptr += 3;
                        break;
                    case 6:
                        PrintDebug(mem, ptr, 3);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        if (val1 == 0)
                            ptr = val2;
                        else
                            ptr += 3;
                        break;
                    case 7:
                        PrintDebug(mem, ptr, 4);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        val3 = ptr_n3;
                        mem[val3] = (val1 < val2) ? 1 : 0;
                        ptr += 4;
                        break;
                    case 8:
                        PrintDebug(mem, ptr, 4);
                        val1 = ((opmode_n1 == 0) ? mem[ptr_n1] : ptr_n1);
                        val2 = ((opmode_n2 == 0) ? mem[ptr_n2] : ptr_n2);
                        val3 = ptr_n3;
                        mem[val3] = (val1 == val2) ? 1 : 0;
                        ptr += 4;
                        break;
                }
            }

            Console.WriteLine($"{{{ptr.ToString("0000")}}} [ HLT ]");
        }

        static void PrintDebug(int[] mem, int ptr, int paramCount)
        {
            string s = $"{{{ptr.ToString("0000")}}} ";
            int instruction = mem[ptr];

            switch (instruction % 100)
            {
                case 1:
                    s += ($"[ ADD ]:");
                    break;
                case 2:
                    s += ($"[ MUL ]:");
                    break;
                case 3:
                    s += ($"[ IN  ]:");
                    break;
                case 4:
                    s += ($"[ OUT ]:");
                    break;
                case 5:
                    s += ($"[ JNZ ]:");
                    break;
                case 6:
                    s += ($"[ JZ  ]:");
                    break;
                case 7:
                    s += ($"[ JL  ]:");
                    break;
                case 8:
                    s += ($"[ JE  ]:");
                    break;
            }

            s += $" {instruction.ToString("00000")} | ";

            var opmodes = new int[] { (instruction % 1000) / 100, (instruction % 10000) / 1000, instruction / 10000 };

            foreach (var i in Enumerable.Range(ptr + 1, paramCount - 1))
            {
                if (opmodes[i - (ptr + 1)] == 0)
                {
                    if (i == ptr + paramCount - 1)
                        s += ($"${mem[i]}");
                    else
                        s += ($"${mem[i]} [#{mem[mem[i]]}]");
                }
                else
                {
                    s += ($"#{mem[i]}");
                }
                s += (", ");
            }

            s = s.Substring(0, s.Length - 2);

            Console.WriteLine(s);
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}
