using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    /*
     * Huge thank you goes out to Immo Landwerth and his series on building your own compiler
     * https://www.youtube.com/playlist?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y
     */
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("input.txt");

            //Parser parser = new Parser("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2");

            long sum = 0;

            foreach (var line in lines)
            {
                SyntaxTree tree = Parser.Parse(line);
                long result = new Evaluator(tree.Root).Evaluate();
                sum += result;
                Console.WriteLine($"{line} => {result}");
            }

            Console.WriteLine($"Sum: {sum}");
        }

        static void PrettyPrint(SyntaxNode node, string indent = "")
        {
            Console.Write(indent);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" " + t.Value);
            }

            Console.WriteLine();

            indent += "    ";
            foreach (SyntaxNode child in node.GetChildren())
            {
                PrettyPrint(child, indent);
            }
        }
    }

    class SyntaxTree
    {
        public ExpressionSyntax Root { get; }
        public SyntaxToken EofToken { get; }

        public SyntaxTree(ExpressionSyntax root, SyntaxToken eofToken)
        {
            Root = root;
            EofToken = eofToken;
        }
    }

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode { }

    class NumberExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken Token { get; }
        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public NumberExpressionSyntax(SyntaxToken token)
        {
            Token = token;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Token;
        }
    }

    class BinaryExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        private SyntaxToken OpenParenthesesToken { get; }
        public ExpressionSyntax Expression { get; }
        private SyntaxToken CloseParenthesesToken { get; }
        public override SyntaxKind Kind => SyntaxKind.ParanthesizedExpression;

        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesesToken, ExpressionSyntax expression, SyntaxToken closeParenthesesToken)
        {
            OpenParenthesesToken = openParenthesesToken;
            Expression = expression;
            CloseParenthesesToken = closeParenthesesToken;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesesToken;
            yield return Expression;
            yield return CloseParenthesesToken;
        }
    }

    class Parser
    {
        private readonly SyntaxToken[] _syntaxTokens;
        private long _position;

        public Parser(string input)
        {
            Lexer lexer = new Lexer(input);
            _syntaxTokens = lexer.GetAllTokens()
                .Where(t => t.Kind != SyntaxKind.EndOfFile && t.Kind != SyntaxKind.Bad && t.Kind != SyntaxKind.WhiteSpace)
                .ToArray();
        }

        private SyntaxToken Peek(long offset)
        {
            long index = _position + offset;
            if (index >= _syntaxTokens.Length)
            {
                return _syntaxTokens.Last();
            }

            return _syntaxTokens[index];
        }

        private SyntaxToken CurrentToken => Peek(0);

        private SyntaxToken NextToken()
        {
            SyntaxToken current = CurrentToken;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (CurrentToken.Kind == kind)
            {
                return NextToken();
            }
            return new SyntaxToken(kind, CurrentToken.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken eofToken = Match(SyntaxKind.EndOfFile);
            return new SyntaxTree(expression, eofToken);
        }

        private ExpressionSyntax ParseExpression() => ParseFactor();

        private ExpressionSyntax ParseTerm()
        {
            ExpressionSyntax left = ParsePrimaryExpression();

            while (CurrentToken.Kind == SyntaxKind.Plus)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParseFactor()
        {
            ExpressionSyntax left = ParseTerm();

            while (CurrentToken.Kind == SyntaxKind.Asterisk)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParseTerm();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (CurrentToken.Kind == SyntaxKind.OpenParantheses)
            {
                SyntaxToken left = NextToken();
                ExpressionSyntax expression = ParseExpression();
                SyntaxToken right = Match(SyntaxKind.CloseParentheses);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            SyntaxToken numberToken = Match(SyntaxKind.Number);
            return new NumberExpressionSyntax(numberToken);
        }

        public static SyntaxTree Parse(string text)
        {
            Parser p = new Parser(text);
            return p.Parse();
        }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position;

        private char CurrentCharacter
        {
            get
            {
                if (_position >= _text.Length)
                {
                    return '\0';
                }

                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public Lexer(string input)
        {
            _text = input;
        }

        public SyntaxToken NextToken()
        {
            // Operators: + *
            // Numbers 0 1 2 3 4 5 6 7 8 9
            // Parantheses ( )

            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFile, _position, "\0", null);
            }

            int start = _position;
            if (char.IsDigit(CurrentCharacter))
            {
                while (char.IsDigit(CurrentCharacter))
                {
                    Next();
                }

                int length = _position - start;
                string text = _text.Substring(start, length);
                long.TryParse(text, out long value);
                return new SyntaxToken(SyntaxKind.Number, start, text, value);
            }

            if (char.IsWhiteSpace(CurrentCharacter))
            {
                while (char.IsWhiteSpace(CurrentCharacter))
                {
                    Next();
                }

                int length = _position - start;
                string text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpace, start, text, null);
            }

            if (CurrentCharacter == '+')
            {
                return new SyntaxToken(SyntaxKind.Plus, _position++, "+", null);
            }
            else if (CurrentCharacter == '*')
            {
                return new SyntaxToken(SyntaxKind.Asterisk, _position++, "*", null);
            }
            else if (CurrentCharacter == '(')
            {
                return new SyntaxToken(SyntaxKind.OpenParantheses, _position++, "(", null);
            }
            else if (CurrentCharacter == ')')
            {
                return new SyntaxToken(SyntaxKind.CloseParentheses, _position++, ")", null);
            }

            return new SyntaxToken(SyntaxKind.Bad, _position++, _text.Substring(_position - 1, 1), 1);
        }

        public IEnumerable<SyntaxToken> GetAllTokens()
        {
            SyntaxToken st;
            do
            {
                st = NextToken();
                yield return st;
            }
            while (st.Kind != SyntaxKind.EndOfFile);
        }
    }

    class SyntaxToken : SyntaxNode
    {
        public override SyntaxKind Kind { get; }
        public long Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxKind kind, long position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    enum SyntaxKind
    {
        WhiteSpace,
        Bad,
        Number,
        Plus,
        Asterisk,
        OpenParantheses,
        CloseParentheses,
        EndOfFile,
        NumberExpression,
        BinaryExpression,
        ParanthesizedExpression
    }

    class Evaluator
    {
        public ExpressionSyntax Root { get; }

        public Evaluator(ExpressionSyntax root)
        {
            Root = root;
        }

        public long Evaluate()
        {
            return EvaluateExpression(Root);
        }

        private long EvaluateExpression(ExpressionSyntax root)
        {
            if (root is NumberExpressionSyntax nes)
            {
                return (long)nes.Token.Value;
            }

            if (root is BinaryExpressionSyntax bes)
            {
                long left = EvaluateExpression(bes.Left);
                long right = EvaluateExpression(bes.Right);

                if (bes.OperatorToken.Kind == SyntaxKind.Plus)
                {
                    return left + right;
                }
                else if (bes.OperatorToken.Kind == SyntaxKind.Asterisk)
                {
                    return left * right;
                }
            }

            if (root is ParenthesizedExpressionSyntax pes)
            {
                return EvaluateExpression(pes.Expression);
            }

            throw new Exception("Unexpected operator node");
        }
    }
}