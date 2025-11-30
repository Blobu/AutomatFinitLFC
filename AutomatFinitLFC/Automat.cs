using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatFinitLFC
{
    public class State
    {
        public int name { get; }
        public Dictionary<State, char> transitions { get; }

        public State(int name)
        {
            this.name = name;
            transitions = new Dictionary<State, char>();
        }

        public override string ToString() => "Q" + name;

        public override bool Equals(object? obj) =>
            obj is State s && s.name == this.name;

        public override int GetHashCode() => name.GetHashCode();
    }
    internal class Automat
    {
        public HashSet<State> states { get; set; }
        public State startState { get; set; }
        public State finalState { get; set; }

        public Automat(HashSet<State> states, State startState, State finalState)
        {
            this.states = states;
            this.startState = startState;
            this.finalState = finalState;
        }

        public Automat(int startName,int finalName,char tranzitie)
        {
            State start = new State(startName);
            State final = new State(finalName);
            start.transitions[final] = tranzitie;
            HashSet<State> states = new HashSet<State> { start, final };
            this.states = states;
            this.startState = start;
            this.finalState = final;
        }

        public Automat(Automat a)
        {
            var map = new Dictionary<int, State>();
            foreach (var s in a.states)
            {
                map[s.name] = new State(s.name);
            }

            foreach (var s in a.states)
            {
                var newState = map[s.name];
                foreach (var kv in s.transitions)
                {
                    var oldTarget = kv.Key;
                    var symbol = kv.Value;
                    newState.transitions[map[oldTarget.name]] = symbol;
                }
            }

            this.states = new HashSet<State>(map.Values);
            this.startState = map[a.startState.name];
            this.finalState = map[a.finalState.name];
        }

        public void replaceState(State oldState, State newState)
        {
            foreach (var state in states)
            {
                var keys = state.transitions.Keys.ToList();
                foreach (var key in keys)
                {
                    if (key.name == oldState.name)
                    {
                        var value = state.transitions[key];
                        state.transitions.Remove(key);
                        state.transitions[newState] = value;
                    }
                }
            }
        }

        
        public int findMaxStateName()
        {
            int max = -1;
            foreach (var state in states)
            {
                if (state.name > max)
                {
                    max = state.name;
                }
            }
            return max;
        }

        //pentru operatorul .
        public Automat concatenat_cu(Automat b)
        {
            
            Automat bCopy = new Automat(b);

            // add epsilon from aCopy final to bCopy start
            this.finalState.transitions[bCopy.startState] = '\0';

            // update final state to bCopy final and merge sets
            var merged = new HashSet<State>(this.states);
            merged.UnionWith(bCopy.states);
            this.states = merged;
            this.finalState = bCopy.finalState;

            return this;

        }


        //pentru operatorul |
        public Automat alternat_cu(Automat b)
        {
            Automat bCopy = new Automat(b);
            int index = Math.Max(this.findMaxStateName(), bCopy.findMaxStateName()) + 1;

            State newStart = new State(index);
            State newFinal = new State(index + 1);

            newStart.transitions[this.startState] = '\0';
            newStart.transitions[bCopy.startState] = '\0';
            this.finalState.transitions[newFinal] = '\0';
            bCopy.finalState.transitions[newFinal] = '\0';

            HashSet<State> newStates = new HashSet<State>(this.states);
            newStates.UnionWith(bCopy.states);
            newStates.Add(newStart);
            newStates.Add(newFinal);

            return new Automat(newStates, newStart, newFinal);
        }

        //pentru operatorul *
        public Automat stelat()
        {
            Automat a = new Automat(this);
            int index = a.findMaxStateName() + 1;
            State newStart = new State(index);
            State newFinal = new State(index + 1);
            newStart.transitions[a.startState] = '\0';
            newStart.transitions[newFinal] = '\0';
            a.finalState.transitions[a.startState] = '\0';
            a.finalState.transitions[newFinal] = '\0';
            HashSet<State> newStates = new HashSet<State>(a.states);
            newStates.Add(newStart);
            newStates.Add(newFinal);
            return new Automat(newStates, newStart, newFinal);
        }


        //pentru operatorul +
        public Automat plus()
        {
            Automat a = new Automat(this);
            int index = a.findMaxStateName() + 1;
            State newStart = new State(index);
            State newFinal = new State(index + 1);
            newStart.transitions[a.startState] = '\0';
            a.finalState.transitions[a.startState] = '\0';
            a.finalState.transitions[newFinal] = '\0';
            HashSet<State> newStates = new HashSet<State>(a.states);
            newStates.Add(newStart);
            newStates.Add(newFinal);
            return new Automat(newStates, newStart, newFinal);
        }


    }
}
