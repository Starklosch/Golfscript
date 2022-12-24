using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    public class Golfscript
    {
        public delegate void Action(Stack context);

        SortedDictionary<string, object> variables = new SortedDictionary<string, object>();

        public Stack Stack { get; }

        public IEnumerable<string> Identifiers => variables.Keys;

        public Golfscript()
        {
            Stack = new Stack(this);

            ResetVariables();
        }

        public void SetVariable(string name, object value)
        {
            variables[name] = value;
        }

        public void SetVariable(string name, Action value)
        {
            variables[name] = value;
        }

        public object GetVariable(string name)
        {
            if (!variables.ContainsKey(name))
                return null;

            return variables[name];
        }

        public bool TryGetVariable(string name, out object value)
        {
            value = default;

            if (!variables.ContainsKey(name))
                return false;

            value = variables[name];
            return true;
        }

        public void UnsetVariable(string name)
        {
            variables.Remove(name);
        }

        public void ResetVariables()
        {
            variables.Clear();

            SetVariable("~", Operators.Evaluate);
            SetVariable("`", Operators.Inspect);
            SetVariable("!", Operators.Negate);
            SetVariable(".", Operators.Duplicate);
            SetVariable(";", Operators.Pop);
            SetVariable("\\", Operators.Swap);
            SetVariable("@", Operators.Rotate);
            SetVariable(")", Operators.Increment);
            SetVariable("(", Operators.Decrement);

            // Coerce
            SetVariable("+", Operators.Addition);
            SetVariable("-", Operators.Subtraction);
            SetVariable("|", Operators.Or);
            SetVariable("&", Operators.And);
            SetVariable("^", Operators.Xor);

            // Order
            SetVariable("*", Operators.Multiplication);
            SetVariable("/", Operators.Division);
            SetVariable("%", Operators.Modulus);
            SetVariable("<", Operators.Less);
            SetVariable(">", Operators.Greater);
            SetVariable("=", Operators.Equal);
            SetVariable("?", Operators.Pow);

            // Other
            SetVariable("$", Operators.Peek);
            SetVariable(",", Operators.Size);

            SetVariable("abs", Operators.Abs);
            SetVariable("base", Operators.Base);
            SetVariable("print", Operators.Print);
            SetVariable("until", Operators.Until);
            SetVariable("while", Operators.While);

            SetVariable("n", new StringItem("\n"));
            SetVariable("p", new BlockItem("`puts"));
            SetVariable("and", new BlockItem("1$if"));
            SetVariable("or", new BlockItem("1$\\if"));
            SetVariable("xor", new BlockItem("\\!!{!}*"));
            SetVariable("puts", new BlockItem("print n print"));
        }

        public void Run(string code, bool reportErrors = false)
        {
            var tokenizer = new Tokenizer(this, code);
            if (reportErrors)
                tokenizer.Error += (sender, error) => Console.WriteLine(error);

            this.Parse(tokenizer.ScanTokens());
        }

        public object this[string variable]
        {
            get => GetVariable(variable);
            set => SetVariable(variable, value);
        }
    }
}
