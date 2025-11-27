using System.IO;
using System.Text;



class Program
{
    static bool esteOperator(char c)
    {
        if (c == '(' || c == ')' || c == '.' || c == '*' || c == '|')
            return true;
        return false;
    }

    static string addPuncte(string expresie)
    {
        for(int i =0;i < expresie.Length-1 ; i++)
        {
            if (!esteOperator(expresie[i])&&(!esteOperator(expresie[i+1])||expresie[i+1]=='('))
            {
                expresie = expresie.Insert(i+1, ".");
            }
        }
        return expresie;
    }

    static int PrioritateOperator(char c)
    {
        switch (c)
        {
            case '*':
                return 3;
            case '.':
                return 2;
            case '|':
                return 1;
            default:
                return 0;
        }
    }

    static string formaPolonezaPostfixata(string expresie)
    {
        var output = new StringBuilder();
        var stack = new Stack<char>();

        foreach (char c in expresie)
        {
            if (!esteOperator(c))
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
                while (stack.Count > 0 && PrioritateOperator(stack.Peek()) >= PrioritateOperator(c))
                {
                    output.Append(stack.Pop());
                }
                stack.Push(c);
            }
        }
        return output.ToString();
    }

    static void Main(string[] args)
    {
        FileMethods fileMethods = new FileMethods("..\\..\\..\\Read.txt");

        string expresie = addPuncte(fileMethods.fileContent);

        int input;

        //read from console
        Console.WriteLine("Alege o obtiune:\n" +
            "1 - afisarea formei poloneze postfixate a expresiei regulate r\n" +
            "2 - afisarea arborelui sintactic corespunzator expresiei regulate r\n" +
            "3 - afisarea automatului M \n" +
            "4 - verificarea unuia sau mai multor cuvinte ˆın automatul M\n");
        input = Convert.ToInt32(Console.ReadLine());

        switch (input)
        {
            case 1:
                string postfix = formaPolonezaPostfixata(expresie);
                Console.WriteLine("Forma poloneza postfixata: " + postfix);
                break;
            case 2:
                //afisarea arborelui sintactic
                break;
            case 3:
                //afisarea automatului M
                break;
            case 4:
                //verificarea unuia sau mai multor cuvinte ˆın automatul M
                break;
            default:
                Console.WriteLine("Optiune invalida");
                break;
        }


    }
}