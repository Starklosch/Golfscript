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

        Dictionary<string, Item> variables = new Dictionary<string, Item>();

        public Stack Stack { get; }

        public IEnumerable<string> Identifiers => variables.Keys;

        public Golfscript()
        {
            Stack = new Stack(this);

            SetVariable("n", new StringItem("\n"));
            SetVariable("p", new BlockItem("`puts"));
            SetVariable("and", new BlockItem("1$if"));
            SetVariable("or", new BlockItem("1$\\if"));
            SetVariable("xor", new BlockItem("\\!!{!}*"));
            SetVariable("puts", new BlockItem("print n print"));
        }

        public void SetVariable(string name, Item value)
        {
            variables[name] = value;
        }

        public Item? GetVariable(string name)
        {
            if (!variables.ContainsKey(name))
                return null;

            return variables[name];
        }

        public bool TryGetVariable(string name, out Item value)
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
        }

        public void Run(string code, bool reportErrors = false)
        {
            var tokenizer = new Tokenizer(this, code);
            if (reportErrors)
                tokenizer.Error += (sender, error) => Console.WriteLine(error);

            this.Parse(tokenizer.ScanTokens());
        }

        public Item? this[string variable]
        {
            get => GetVariable(variable);
            set => SetVariable(variable, value);
        }
    }
}
