using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            IEnumerable<Instruction> instructions = ParseToInstructions(lines);

            CPU cpu = new CPU(instructions.ToList());

            int exitCode = cpu.RunUntilLoop();
            Console.WriteLine($"Acc before loop: {exitCode}");
        }

        public static IEnumerable<Instruction> ParseToInstructions(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                string[] fragments = line.Split(' ');
                Instruction i = new Instruction
                {
                    OpCode = fragments[0],
                    Parameter = int.Parse(fragments[1])
                };
                yield return i;
            }
        }
    }

    class Instruction
    {
        public string OpCode { get; set; }

        public int Parameter { get; set; }

        public bool HasBeenRun { get; set; }
    }

    class CPU
    {
        public List<Instruction> Instructions { get; }

        public int Accumulator { get; private set; }

        public int InstructionPointer { get; private set; }

        public CPU(List<Instruction> instructions)
        {
            Instructions = instructions;
            Accumulator = 0;
        }

        public int RunUntilLoop()
        {
            Instruction ins;
            while (!(ins = Instructions[InstructionPointer]).HasBeenRun)
            {
                PerformSingleInstruction(ins);
            }

            return Accumulator;
        }

        private void PerformSingleInstruction(Instruction ins)
        {
            Console.WriteLine($"# {ins.OpCode} {ins.Parameter}");
            switch (ins.OpCode)
            {
                case "jmp":
                    InstructionPointer += ins.Parameter;
                    break;
                case "acc":
                    Accumulator += ins.Parameter;
                    goto default;
                default:
                    InstructionPointer += 1;
                    break;
            }

            ins.HasBeenRun = true;
        }
    }
}
