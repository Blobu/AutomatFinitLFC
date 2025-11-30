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
                if (char.IsLetterOrDigit(c))
                {
                    alphabet.Add(c);
                }
            }
            return alphabet;
        }



        public Automat AFN(string regex)
        {
            int index = 0;
            var stack = new Stack<Automat>();

            foreach (char c in regex)
            {
                if (char.IsLetterOrDigit(c))
                {
                   stack.Push(new Automat(index, index + 1, c));
                     index += 2;
                }
                else if (c == '*' || c == '+')
                {
                    if (stack.Count < 1) throw new FormatException($"Este gresita forma postfixata. Nu pot sa am '{c}' fara sa am cel putin un element din alfabet inainte");
                    var node = stack.Pop();
                    if (c == '*')
                        stack.Push(node.stelat());
                    else
                        stack.Push(node.plus());

                    index = Math.Max(index, stack.Peek().findMaxStateName() + 1);
                }
                else if (c == '.' || c == '|')
                {
                    if (stack.Count < 2) throw new FormatException($"Este gresita forma postfixata. Nu pot sa am '{c}' fara sa am cel putin 2 elemente din alfabet inainte");
                    var right = stack.Pop();
                    var left = stack.Pop();
                    if( c == '.')
                        stack.Push(left.concatenat_cu(right));
                    else
                        stack.Push(left.alternat_cu(right));

                    index = Math.Max(index, stack.Peek().findMaxStateName() + 1);
                }
                else
                {
                    throw new FormatException($"Nu exista operatia '{c}'");
                }
            }

            if (stack.Count != 1) throw new FormatException("Prea multe elemente din alfabet");

            printAFN(stack.Peek());

            return stack.Pop();

        }


        public void printAFN(Automat afn)
        {
            if (afn == null)
            {
                Console.WriteLine("AFN is null");
                return;
            }

            Console.WriteLine("AFN description:");
            Console.WriteLine();

            Console.WriteLine($"States ({afn.states.Count}):");
            foreach (var state in afn.states.OrderBy(s => s.name))
            {
                Console.WriteLine($"  {state}"); // State.ToString -> Q{n}
            }

            Console.WriteLine();
            Console.WriteLine($"Start state: {afn.startState}");
            Console.WriteLine($"Final state: {afn.finalState}");
            Console.WriteLine();

            Console.WriteLine("Transitions:");
            foreach (var state in afn.states.OrderBy(s => s.name))
            {
                foreach (var trans in state.transitions)
                {
                    var symbol = trans.Value == '\0' ? "ε" : trans.Value.ToString();
                    Console.WriteLine($"  Tr({state}, {symbol}) -> {trans.Key}");
                }
            }
            Console.WriteLine();
        }




        public HashSet<State> LambdaExecution(HashSet<State> states)
        {
            var stack = new Stack<State>(states);
            var closure = new HashSet<State>(states);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var trans in current.transitions)
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

        
        public HashSet<State> getAllStates(HashSet<State> states, char c)
        {
            var result = new HashSet<State>();
            foreach (var state in states)
            {
                foreach (var trans in state.transitions)
                {
                    if (trans.Value == c)
                    {
                        result.Add(trans.Key);
                    }
                }
            }
            return result;
        }

        public string CheckForSameStates(Dictionary<string, HashSet<State>> AllStates, HashSet<State> NewStateSet)
        {
            foreach (var existing in AllStates)
            {
                if (existing.Value.SetEquals(NewStateSet))
                    return existing.Key;
            }
            return "-1"; 
        }

        public DeterministicFiniteAutomaton RegexToDFA(string regex)
        {
            Automat afn = AFN(regex);
            HashSet<char> alphabet = DetermineAlphabet(regex);

            
            Dictionary<string, HashSet<State>> DfaStateMapping = new Dictionary<string, HashSet<State>>();
            var stack = new Stack<string>(); 

            HashSet<string> DFA_states = new HashSet<string>();
            Dictionary<(string, char), string> DFA_functions = new Dictionary<(string, char), string>();
            HashSet<string> DFA_finalStates = new HashSet<string>();

            int index = 0;

            
            var startSet = new HashSet<State> { afn.startState };
            var startClosure = LambdaExecution(startSet);

            string startStateName = "Q" + index;
            DfaStateMapping.Add(startStateName, startClosure);
            stack.Push(startStateName);
            DFA_states.Add(startStateName);

            
            if (startClosure.Contains(afn.finalState))
                DFA_finalStates.Add(startStateName);

            
            while (stack.Count > 0)
            {
                string currentStateName = stack.Pop();
                HashSet<State> currentNFAStates = DfaStateMapping[currentStateName];

                foreach (char c in alphabet)
                {
                    
                    var moveResult = getAllStates(currentNFAStates, c);

                    
                    if (moveResult.Count == 0) continue;

                    
                    var nextStateSet = LambdaExecution(moveResult);

                    
                    string existingName = CheckForSameStates(DfaStateMapping, nextStateSet);

                    if (existingName == "-1")
                    {
                        
                        index++;
                        string newStateName = "Q" + index;

                        DfaStateMapping.Add(newStateName, nextStateSet);
                        stack.Push(newStateName); 
                        DFA_states.Add(newStateName);

                        
                        DFA_functions.Add((currentStateName, c), newStateName);

                        
                        foreach (var s in nextStateSet)
                        {
                            if (s.Equals(afn.finalState)) 
                            {
                                DFA_finalStates.Add(newStateName);
                                break;
                            }
                        }
                    }
                    else
                    {
                        
                        if (!DFA_functions.ContainsKey((currentStateName, c)))
                        {
                            DFA_functions.Add((currentStateName, c), existingName);
                        }
                    }
                }
            }

            return new DeterministicFiniteAutomaton(DFA_states, alphabet, DFA_functions, startStateName, DFA_finalStates);
        }
    }
}
    

