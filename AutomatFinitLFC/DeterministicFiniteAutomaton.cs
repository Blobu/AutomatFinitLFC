using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutomatFinitLFC
{
    public class DeterministicFiniteAutomaton
    {
        private HashSet<string> States { get; set; }
        private HashSet<char> Alphabet { get; set; }
        private Dictionary<(string, char), string> TransitionFunction { get; set; }
        private string StartState { get; set; }

        private HashSet<string> FinalStates { get; set; }

        public DeterministicFiniteAutomaton(HashSet<string> states, HashSet<char> alphabet,
            Dictionary<(string, char), string> transitionFunction, string startState,
            HashSet<string> finalStates)
        {
            this.States = states;
            this.Alphabet = alphabet;
            this.TransitionFunction = transitionFunction;
            this.StartState = startState;
            this.FinalStates = finalStates;
        }

        public bool VerifyAutomaton()
        {
            if (!States.Contains(StartState))
            {
                return false;
            }

            if (!FinalStates.All(state => States.Contains(state)))
            {
                return false;
            }

            foreach (var kvp in TransitionFunction)
            {
                var (state, symbol) = kvp.Key;
                var targetState = kvp.Value;

                if (!States.Contains(state) || !States.Contains(targetState))
                    return false;

                if (!Alphabet.Contains(symbol))
                    return false;
            }

            return true;
        }

        public void PrintAutomaton(TextWriter? writer = null)
        {
            writer ??= Console.Out;

            writer.WriteLine("States: " + string.Join(", ", States));
            writer.WriteLine("Alphabet: " + string.Join(", ", Alphabet));
            writer.WriteLine("Start State: " + StartState);
            writer.WriteLine("Final States: " + string.Join(", ", FinalStates));
            writer.WriteLine("Transition Function:");

            foreach (var kvp in TransitionFunction)
            {
                writer.WriteLine($" Tr({kvp.Key.Item1}, {kvp.Key.Item2}) = {kvp.Value}");
            }
        }

        public bool CheckWord(string word)
        {
            string currentState = StartState;
            foreach (char symbol in word)
            {
                if (!Alphabet.Contains(symbol))
                {
                    Console.WriteLine($"Symbol '{symbol}' not in alphabet.");
                    Console.WriteLine("Rejected.");
                    return false;
                }

                if (!TransitionFunction.TryGetValue((currentState, symbol), out var nextState))
                {
                    Console.WriteLine($"No transition from state '{currentState}' on symbol '{symbol}'.");
                    Console.WriteLine("Rejected.");
                    return false;
                }

                currentState = nextState;
            }

            if (FinalStates.Contains(currentState))
            {
                Console.WriteLine("Accepted.");
                return true;
            }
            else
            {
                Console.WriteLine("Rejected.");
                return false;
            }

        }

    }
}