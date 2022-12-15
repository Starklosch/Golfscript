using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    class Golfscript
    {
        public delegate void Action(Stack context);

        Dictionary<string, Item> variables = new Dictionary<string, Item>();
        Stack stack;

        public Stack Stack { get => stack; }

        public Dictionary<string, Item>.KeyCollection VariableNames => variables.Keys;

        public Golfscript()
        {
            stack = new Stack(this);
        }

        public void SetVariable(string name, Item? value)
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

        public IEnumerable<Token> ScanTokens(string buffer) {
            var tokenizer = new Tokenizer(buffer);
            return tokenizer.ScanTokens(this);
        }

        public Item? this[string variable]
        {
            get => GetVariable(variable);
            set => SetVariable(variable, value);
        }
    }
}
