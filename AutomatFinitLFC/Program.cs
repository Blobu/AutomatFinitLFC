using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using AutomatFinitLFC;

class Program
{
    static bool IsOperator(char c)
    {
        return c == '(' || c == ')' || c == '.' || c == '*' || c == '|' || c == '+';
    }

    static bool IsOperand(char c)
    {
        return c >= 'a' && c <= 'z';
    }

    static bool ValidateExpression(string expression)
    {
        foreach (char c in expression)
        {
            if (IsOperator(c) || IsOperand(c))
                continue;
            Console.WriteLine($"Invalid character in expression: '{c}'. Only lowercase letters a-z and operators () . * + | are allowed.");
            return false;
        }
        return true;
    }

    static string AddDots(string expression)
    {
        for (int i = 0; i < expression.Length - 1; i++)
        {
            if (!IsOperator(expression[i]) && (!IsOperator(expression[i + 1]) || expression[i + 1] == '('))
            {
                expression = expression.Insert(i + 1, ".");
            }
            else if (expression[i] == '*' || expression[i] == '+')
            {
                if (!IsOperator(expression[i + 1]) || expression[i + 1] == '(')
                {
                    expression = expression.Insert(i + 1, ".");
                }
            }
            else if (expression[i] == ')')
            {
                if (!IsOperator(expression[i + 1]) || expression[i + 1] == '(')
                {
                    expression = expression.Insert(i + 1, ".");
                }
            }
        }
        return expression;
    }

    static int OperatorPrecedence(char c)
    {
        return c switch
        {
            '*' => 3,
            '+' => 3,
            '.' => 2,
            '|' => 1,
            _ => 0,
        };
    }

    static string ToPostfix(string expression)
    {
        var output = new StringBuilder();
        var stack = new Stack<char>();

        foreach (char c in expression)
        {
            if (!IsOperator(c))
            {
                output.Append(c);
            }
            else if (c == '(')
            {
                stack.Push(c);
            }
            else if (c == ')')
            {
                while (stack.Count > 0 && stack.Peek() != '(')
                {
                    output.Append(stack.Pop());
                }
                stack.Pop();
            }
            else
            {
                while (stack.Count > 0 && OperatorPrecedence(stack.Peek()) >= OperatorPrecedence(c))
                {
                    output.Append(stack.Pop());
                }
                stack.Push(c);
            }
        }
        while (stack.Count > 0)
        {
            output.Append(stack.Pop());
        }
        return output.ToString();
    }

    public static RegexNode BuildSyntaxTree(string postfix)
    {
        Stack<RegexNode> stack = new Stack<RegexNode>();
        foreach (char c in postfix)
        {
            if (IsOperand(c))
            {
                stack.Push(new RegexNode(c));
            }
            else if (c == '*' || c == '+')
            {
                RegexNode node = stack.Pop();
                stack.Push(new RegexNode(c, node));
            }
            else if (c == '.' || c == '|')
            {
                RegexNode right = stack.Pop();
                RegexNode left = stack.Pop();
                stack.Push(new RegexNode(c, left, right));
            }
        }
        return stack.Pop();
    }

    public static void PrintTree(RegexNode node, string indent = "", bool isLeft = true)
    {
        if (node == null) return;

        Console.WriteLine(indent + (isLeft ? "├── " : "└── ") + node.Value);

        PrintTree(node.Left, indent + (isLeft ? "│   " : "    "), true);
        PrintTree(node.Right, indent + (isLeft ? "│   " : "    "), false);
    }


    static void Main(string[] args)
    {
        FileMethods fileMethods = new FileMethods("..\\..\\..\\Read.txt");

        string raw = fileMethods.FileContent ?? string.Empty;
        string normalized = new string(raw.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

        if (!ValidateExpression(normalized))
        {
            Console.WriteLine("Expression validation failed. Exiting.");
            return;
        }

        string expression = AddDots(normalized);
        string postfix = ToPostfix(expression);
        RegexToDFAClass regexToDFA = new RegexToDFAClass();
        DeterministicFiniteAutomaton dfa = regexToDFA.RegexToDFA(postfix);
        Console.WriteLine(expression + "\n");
        int input;

        Console.WriteLine("Choose an option:\n" +
            "1 - Show postfix form of the regular expression\n" +
            "2 - Print the syntax tree of the regular expression\n" +
            "3 - Print the automaton M\n" +
            "4 - Check one or more words in automaton M\n");
        input = Convert.ToInt32(Console.ReadLine());

        switch (input)
        {
            case 1:
                Console.WriteLine("Expression with dots: " + expression);
                Console.WriteLine("Postfix form: " + postfix);
                break;
            case 2:
                RegexNode root = BuildSyntaxTree(postfix);
                PrintTree(root);
                break;
            case 3:
                var writer = new StreamWriter("..\\..\\..\\output.txt", false);
                try
                {
                    dfa.PrintAutomaton(writer);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error writing to file: " + ex.Message);
                    throw;
                }
                dfa.PrintAutomaton();
                break;
            case 4:
                var words = new List<string>();
                Console.WriteLine("Enter words, one per line. Leave an empty line to finish:");
                while (true)
                {
                    string? line = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        break;
                    words.Add(line.Trim());
                }
                if (words.Count == 0)
                {
                    Console.WriteLine("No words to check.");
                }
                else
                {
                    foreach (var word in words)
                    {
                        Console.WriteLine($"Checking word: {word}");
                        dfa.CheckWord(word);
                        Console.WriteLine();
                    }
                }
                break;
            default:
                Console.WriteLine("Invalid option");
                break;
        }


    }
}