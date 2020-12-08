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

            int exitCode = cpu.TryAutocorrectingRun(out bool hasFailed);
            Console.WriteLine($"Exit code of fixed instructions: {exitCode} | Has failed: {hasFailed}");
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

        public void Reset()
        {
            InstructionPointer = 0;
            Accumulator = 0;
        }

        public int Run(out bool hasLooped)
        {
            Instruction ins = Instructions[InstructionPointer];
            while (InstructionPointer < Instructions.Count && !(ins = Instructions[InstructionPointer]).HasBeenRun)
            {
                PerformSingleInstruction(ins);
            }

            hasLooped = ins.HasBeenRun && !(InstructionPointer >= Instructions.Count);

            return Accumulator;
        }

        public int TryAutocorrectingRun(out bool hasFailed)
        {
            int amountOfNops = Instructions.Sum(i => i.OpCode == "nop" ? 1 : 0);
            int amountOfJmps = Instructions.Sum(i => i.OpCode == "jmp" ? 1 : 0);

            Console.WriteLine($"Trying to autocorrect from {amountOfJmps} Jmps and {amountOfNops} Nops.");

            for (int i = 0; i < Instructions.Count; i++)
            {
                if (Instructions[i].OpCode != "nop" && Instructions[i].OpCode != "jmp")
                {
                    continue;
                }

                if (Instructions[i].OpCode == "nop")
                {
                    Instructions[i].OpCode = "jmp";
                } 
                else if (Instructions[i].OpCode == "jmp")
                {
                    Instructions[i].OpCode = "nop";
                }

                int exitCode = Run(out bool hasLooped);
                Reset();
                Instructions.ForEach(instruction => instruction.HasBeenRun = false);
                Console.WriteLine("---");

                if (hasLooped)
                {
                    if (Instructions[i].OpCode == "nop")
                    {
                        Instructions[i].OpCode = "jmp";
                    }
                    else if (Instructions[i].OpCode == "jmp")
                    {
                        Instructions[i].OpCode = "nop";
                    }
                }
                else
                {
                    hasFailed = false;
                    return exitCode;
                }
            }

            hasFailed = true;
            return -1;
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
