using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;


namespace AutomatFinitLFC
{
    internal class RegexToDFAClass
    {
        public RegexToDFAClass() { }

        public HashSet<char> DetermineAlphabet(string regex)
        {
            HashSet<char> alphabet = new HashSet<char>();
            foreach (char c in regex)
            {
                if (c >= 'a' && c <= 'z')
                {
                    alphabet.Add(c);
                }
            }
            return alphabet;
        }



        public Automat BuildNFA(string regex)
        {
            int index = 0;
            var stack = new Stack<Automat>();

            foreach (char c in regex)
            {
                if (c >= 'a' && c <= 'z')
                {
                    stack.Push(new Automat(index, index + 1, c));
                    index += 2;
                }
                else if (c == '*' || c == '+')
                {
                    if (stack.Count < 1) throw new FormatException($"Invalid postfix form: cannot have '{c}' without at least one alphabet element before it");
                    var node = stack.Pop();
                    if (c == '*')
                        stack.Push(node.Star());
                    else
                        stack.Push(node.Plus());

                    index = Math.Max(index, stack.Peek().findMaxStateName() + 1);
                }
                else if (c == '.' || c == '|')
                {
                    if (stack.Count < 2) throw new FormatException($"Invalid postfix form: cannot have '{c}' without at least 2 alphabet elements before it");
                    var right = stack.Pop();
                    var left = stack.Pop();
                    if (c == '.')
                        stack.Push(left.ConcatenateWith(right));
                    else
                        stack.Push(left.AlternateWith(right));

                    index = Math.Max(index, stack.Peek().findMaxStateName() + 1);
                }
                else
                {
                    throw new FormatException($"Unknown operation '{c}'");
                }
            }

            if (stack.Count != 1) throw new FormatException("Too many alphabet elements");

            PrintNFA(stack.Peek());

            return stack.Pop();

        }


        public void PrintNFA(Automat nfa)
        {
            if (nfa == null)
            {
                Console.WriteLine("NFA is null");
                return;
            }

            Console.WriteLine("NFA description:");
            Console.WriteLine();

            Console.WriteLine($"States ({nfa.States.Count}):");
            foreach (var state in nfa.States.OrderBy(s => s.Name))
            {
                Console.WriteLine($"  {state}");
            }

            Console.WriteLine();
            Console.WriteLine($"Start state: {nfa.StartState}");
            Console.WriteLine($"Final state: {nfa.FinalState}");
            Console.WriteLine();

            Console.WriteLine("Transitions:");
            foreach (var state in nfa.States.OrderBy(s => s.Name))
            {
                foreach (var trans in state.Transitions)
                {
                    var symbol = trans.Value == '\0' ? "e" : trans.Value.ToString();
                    Console.WriteLine($"  Tr({state}, {symbol}) -> {trans.Key}");
                }
            }
            Console.WriteLine();
        }

        public HashSet<State> LambdaClosure(HashSet<State> states)
        {
            var stack = new Stack<State>(states);
            var closure = new HashSet<State>(states);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var trans in current.Transitions)
                {
                    if (trans.Value == '\0' && !closure.Contains(trans.Key))
                    {
                        closure.Add(trans.Key);
                        stack.Push(trans.Key);
                    }
                }
            }
            return closure;
        }


        public HashSet<State> GetAllStatesForSymbol(HashSet<State> states, char c)
        {
            var result = new HashSet<State>();
            foreach (var state in states)
            {
                foreach (var trans in state.Transitions)
                {
                    if (trans.Value == c)
                    {
                        result.Add(trans.Key);
                    }
                }
            }
            return result;
        }

        public string FindExistingStateName(Dictionary<string, HashSet<State>> allStates, HashSet<State> newStateSet)
        {
            foreach (var existing in allStates)
            {
                if (existing.Value.SetEquals(newStateSet))
                    return existing.Key;
            }
            return "-1";
        }

        public DeterministicFiniteAutomaton RegexToDFA(string regex)
        {
            Automat nfa = BuildNFA(regex);
            HashSet<char> alphabet = DetermineAlphabet(regex);


            Dictionary<string, HashSet<State>> dfaStateMapping = new Dictionary<string, HashSet<State>>();
            var stack = new Stack<string>();

            HashSet<string> dfaStates = new HashSet<string>();
            Dictionary<(string, char), string> dfaFunctions = new Dictionary<(string, char), string>();
            HashSet<string> dfaFinalStates = new HashSet<string>();

            int index = 0;


            var startSet = new HashSet<State> { nfa.StartState };
            var startClosure = LambdaClosure(startSet);

            string startStateName = "Q" + index;
            dfaStateMapping.Add(startStateName, startClosure);
            stack.Push(startStateName);
            dfaStates.Add(startStateName);


            if (startClosure.Contains(nfa.FinalState))
                dfaFinalStates.Add(startStateName);


            while (stack.Count > 0)
            {
                string currentStateName = stack.Pop();
                HashSet<State> currentNFAStates = dfaStateMapping[currentStateName];

                foreach (char c in alphabet)
                {

                    var moveResult = GetAllStatesForSymbol(currentNFAStates, c);

                    if (moveResult.Count == 0) continue;

                    var nextStateSet = LambdaClosure(moveResult);

                    string existingName = FindExistingStateName(dfaStateMapping, nextStateSet);

                    if (existingName == "-1")
                    {
                        index++;
                        string newStateName = "Q" + index;

                        dfaStateMapping.Add(newStateName, nextStateSet);
                        stack.Push(newStateName);
                        dfaStates.Add(newStateName);

                        dfaFunctions.Add((currentStateName, c), newStateName);

                        foreach (var s in nextStateSet)
                        {
                            if (s.Equals(nfa.FinalState))
                            {
                                dfaFinalStates.Add(newStateName);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!dfaFunctions.ContainsKey((currentStateName, c)))
                        {
                            dfaFunctions.Add((currentStateName, c), existingName);
                        }
                    }
                }
            }

            return new DeterministicFiniteAutomaton(dfaStates, alphabet, dfaFunctions, startStateName, dfaFinalStates);
        }
    }
}

