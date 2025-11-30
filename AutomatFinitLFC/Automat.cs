using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatFinitLFC
{
    public class State
    {
        public int Name { get; }
        public Dictionary<State, char> Transitions { get; }

        public State(int name)
        {
            this.Name = name;
            Transitions = new Dictionary<State, char>();
        }

        public override string ToString() => "Q" + Name;

        public override bool Equals(object? obj) =>
            obj is State s && s.Name == this.Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
    internal class Automat
    {
        public HashSet<State> States { get; set; }
        public State StartState { get; set; }
        public State FinalState { get; set; }

        public Automat(HashSet<State> states, State startState, State finalState)
        {
            this.States = states;
            this.StartState = startState;
            this.FinalState = finalState;
        }

        public Automat(int startName, int finalName, char transition)
        {
            State start = new State(startName);
            State final = new State(finalName);
            start.Transitions[final] = transition;
            HashSet<State> states = new HashSet<State> { start, final };
            this.States = states;
            this.StartState = start;
            this.FinalState = final;
        }

        public Automat(Automat a)
        {
            var map = new Dictionary<int, State>();
            foreach (var s in a.States)
            {
                map[s.Name] = new State(s.Name);
            }

            foreach (var s in a.States)
            {
                var newState = map[s.Name];
                foreach (var kv in s.Transitions)
                {
                    var oldTarget = kv.Key;
                    var symbol = kv.Value;
                    newState.Transitions[map[oldTarget.Name]] = symbol;
                }
            }

            this.States = new HashSet<State>(map.Values);
            this.StartState = map[a.StartState.Name];
            this.FinalState = map[a.FinalState.Name];
        }

        public void ReplaceState(State oldState, State newState)
        {
            foreach (var state in States)
            {
                var keys = state.Transitions.Keys.ToList();
                foreach (var key in keys)
                {
                    if (key.Name == oldState.Name)
                    {
                        var value = state.Transitions[key];
                        state.Transitions.Remove(key);
                        state.Transitions[newState] = value;
                    }
                }
            }
        }


        public int findMaxStateName()
        {
            int max = -1;
            foreach (var state in States)
            {
                if (state.Name > max)
                {
                    max = state.Name;
                }
            }
            return max;
        }

        public Automat ConcatenateWith(Automat b)
        {

            Automat bCopy = new Automat(b);

            this.FinalState.Transitions[bCopy.StartState] = '\0';

            var merged = new HashSet<State>(this.States);
            merged.UnionWith(bCopy.States);
            this.States = merged;
            this.FinalState = bCopy.FinalState;

            return this;

        }


        public Automat AlternateWith(Automat b)
        {
            Automat bCopy = new Automat(b);
            int index = Math.Max(this.findMaxStateName(), bCopy.findMaxStateName()) + 1;

            State newStart = new State(index);
            State newFinal = new State(index + 1);

            newStart.Transitions[this.StartState] = '\0';
            newStart.Transitions[bCopy.StartState] = '\0';
            this.FinalState.Transitions[newFinal] = '\0';
            bCopy.FinalState.Transitions[newFinal] = '\0';

            HashSet<State> newStates = new HashSet<State>(this.States);
            newStates.UnionWith(bCopy.States);
            newStates.Add(newStart);
            newStates.Add(newFinal);

            return new Automat(newStates, newStart, newFinal);
        }

        public Automat Star()
        {
            Automat a = new Automat(this);
            int index = a.findMaxStateName() + 1;
            State newStart = new State(index);
            State newFinal = new State(index + 1);
            newStart.Transitions[a.StartState] = '\0';
            newStart.Transitions[newFinal] = '\0';
            a.FinalState.Transitions[a.StartState] = '\0';
            a.FinalState.Transitions[newFinal] = '\0';
            HashSet<State> newStates = new HashSet<State>(a.States);
            newStates.Add(newStart);
            newStates.Add(newFinal);
            return new Automat(newStates, newStart, newFinal);
        }


        public Automat Plus()
        {
            Automat a = new Automat(this);
            int index = a.findMaxStateName() + 1;
            State newStart = new State(index);
            State newFinal = new State(index + 1);
            newStart.Transitions[a.StartState] = '\0';
            a.FinalState.Transitions[a.StartState] = '\0';
            a.FinalState.Transitions[newFinal] = '\0';
            HashSet<State> newStates = new HashSet<State>(a.States);
            newStates.Add(newStart);
            newStates.Add(newFinal);
            return new Automat(newStates, newStart, newFinal);
        }

    }
}
