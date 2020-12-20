using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            long sum = 0;
            foreach (var line in lines)
            {
                IEnumerable<Token> tokens = Lex(line);
                var enumerable = tokens.ToList();
                
                IExpression ex = Parse(enumerable);
                long result = ex.Evaluate();
                Console.WriteLine($"{PrintTree(ex)} = {result}");
                sum += result;
            }

            Console.WriteLine();
            Console.WriteLine($"Sum of all expressions: {sum}");
        }

        static string PrintTree(IExpression ex)
        {
            if (ex is NumberExpression ne)
            {
                return ne.ToString();
            }
            if (ex is AdditionExpression ae)
            {
                return $"({PrintTree(ae.FirstParameter)} + {PrintTree(ae.SecondParameter)})";
            }
            if (ex is MulitplicationExpression me)
            {
                return $"({PrintTree(me.FirstParameter)} * {PrintTree(me.SecondParameter)})";
            }

            return string.Empty;
        }

        static IEnumerable<Token> Lex(string line)
        {
            foreach (var c in line)
            {
                if (!char.IsWhiteSpace(c))
                {
                    yield return new Token(c.ToString());
                }
            }
        }

        static IExpression Parse(IEnumerable<Token> tokens)
        {
            var enumerable = tokens.ToList();

            IExpression root = null;
            IExpression temp = null;

            for (int i = 0; i < enumerable.Count(); i++)
            {
                Token currentToken = enumerable[i];
                if (int.TryParse(currentToken.Value, out int result))
                {
                    NumberExpression ne = new NumberExpression(result);
                    temp = ne;

                    if (root == null)
                    {
                        root = ne;
                    }
                    else if (root is AdditionExpression ae)
                    {
                        ae.SecondParameter = ne;
                    }
                    else if (root is MulitplicationExpression me)
                    {
                        me.SecondParameter = ne;
                    }
                }
                else if (currentToken.Value == "+")
                {
                    if (root == null)
                    {
                        root = new AdditionExpression() { FirstParameter = temp };
                    }
                    else if (root is AdditionExpression ae)
                    {
                        root = new AdditionExpression() { FirstParameter = ae, SecondParameter = temp };
                    }
                    else if (root is MulitplicationExpression me)
                    {
                        root = new AdditionExpression() { FirstParameter = me, SecondParameter = temp };
                    }
                    else if (root is NumberExpression ne)
                    {
                        root = new AdditionExpression() { FirstParameter = ne, SecondParameter = temp };
                    }
                }
                else if (currentToken.Value == "*")
                {
                    if (root == null)
                    {
                        root = new MulitplicationExpression() { FirstParameter = temp };
                    }
                    else if (root is AdditionExpression ae)
                    {
                        root = new MulitplicationExpression() { FirstParameter = ae, SecondParameter = temp };
                    }
                    else if (root is MulitplicationExpression me)
                    {
                        root = new MulitplicationExpression() { FirstParameter = me, SecondParameter = temp };
                    }
                    else if (root is NumberExpression ne)
                    {
                        root = new MulitplicationExpression() { FirstParameter = ne, SecondParameter = temp };
                    }
                }
                else if (currentToken.Value == "(")
                {

                    IExpression subExpression = null;
                    int paranthesesCount = 0;
                    for (int j = i; j < enumerable.Count; j++)
                    {
                        if (enumerable[j].Value == "(")
                        {
                            paranthesesCount++;
                        }
                        else if (enumerable[j].Value == ")")
                        {
                            paranthesesCount--;
                        }

                        if (paranthesesCount == 0)
                        {
                            IEnumerable<Token> subTokenSequence = enumerable.Skip(i + 1).Take(j - i - 1);
                            subExpression = Parse(subTokenSequence);
                            i = j;
                            break;
                        }
                    }

                    if (root == null)
                    {
                        root = subExpression;
                    }
                    else if (root is AdditionExpression ae)
                    {
                        ae.SecondParameter = subExpression;
                    }
                    else if (root is MulitplicationExpression me)
                    {
                        me.SecondParameter = subExpression;
                    }
                }
            }

            return root;
        }
    }

    class Token
    {
        public string Value { get; set; }

        public Token(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }

    interface IExpression
    {
        public long Evaluate();
    }

    class NumberExpression : IExpression
    {
        public int Value { get; set; }

        public long Evaluate() => Value;

        public NumberExpression(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }

    class AdditionExpression : IExpression
    {
        public IExpression FirstParameter { get; set; }

        public IExpression SecondParameter { get; set; }

        public long Evaluate()
        {
            return FirstParameter.Evaluate() + SecondParameter.Evaluate();
        }
    }

    class MulitplicationExpression : IExpression
    {
        public IExpression FirstParameter { get; set; }

        public IExpression SecondParameter { get; set; }

        public long Evaluate()
        {
            return FirstParameter.Evaluate() * SecondParameter.Evaluate();
        }
    }
}
