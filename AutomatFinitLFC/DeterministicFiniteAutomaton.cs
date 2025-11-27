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



    // public bool VerifyAutomation();

    // public void PrintAutomation();

    // public bool ChackWord(string word);

}