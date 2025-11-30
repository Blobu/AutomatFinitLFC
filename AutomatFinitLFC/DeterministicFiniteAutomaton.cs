using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class DeterministicFiniteAutomaton
{
    private HashSet<string> states { get; set; }
    private HashSet<char> alphabet { get; set; }
    private Dictionary<(string, char), string> transitionFunction { get; set; }
    private string startState { get; set; }

    private HashSet<string> finalStates { get; set; }

    public DeterministicFiniteAutomaton(HashSet<string> states, HashSet<char> alphabet,
        Dictionary<(string, char), string> transitionFunction, string startState,
        HashSet<string> finalStates)
    {
        this.states = states;
        this.alphabet = alphabet;
        this.transitionFunction = transitionFunction;
        this.startState = startState;
        this.finalStates = finalStates;
    }

    public bool VerifyAutomaton()
    {
        if(!states.Contains(startState))
        {
            return false;
        }

        if(!finalStates.All(state => states.Contains(state)))
        {
            return false;
        }

        foreach (var kvp in transitionFunction) //kvp = KeyValuePointer
        {
            var (state, symbol) = kvp.Key;
            var targetState = kvp.Value;

            if (!states.Contains(state) || !states.Contains(targetState))
                return false;

            if (!alphabet.Contains(symbol))
                return false;
        }

        return true;
    }

    public void PrintAutomaton(TextWriter? writer = null)
    {
        writer ??= Console.Out;

        writer.WriteLine("States: " + string.Join(", ", states));
        writer.WriteLine("Alphabet: " + string.Join(", ", alphabet));
        writer.WriteLine("Start State: " + startState);
        writer.WriteLine("Final States: " + string.Join(", ", finalStates));
        writer.WriteLine("Transition Function:");

        foreach (var kvp in transitionFunction)
        {
            writer.WriteLine($" Tr({kvp.Key.Item1}, {kvp.Key.Item2}) = {kvp.Value}");
        }
    }

    public bool CheckWord(string word)
    {
        string currentState = startState;
        foreach (char symbol in word)
        {
            if (!alphabet.Contains(symbol))
            {
                Console.WriteLine($"Symbol '{symbol}' not in alphabet.");
                Console.WriteLine("Rejected.");
                return false;
            }

            if (!transitionFunction.TryGetValue((currentState, symbol), out var nextState))
            {
                Console.WriteLine($"No transition from state '{currentState}' on symbol '{symbol}'.");
                Console.WriteLine("Rejected.");
                return false;
            }

            currentState = nextState;
        }

        if (finalStates.Contains(currentState))
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