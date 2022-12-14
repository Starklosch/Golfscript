using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golfscript
{
    class Golfscript
    {
        Dictionary<string, Item> variables = new Dictionary<string, Item>();
        Stack stack = new Stack();

        public Stack Stack { get => stack; }

        public Dictionary<string, Item>.KeyCollection VariableNames => variables.Keys;

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

        public Tokenizer ScanTokens(string buffer) {
            var tokenizer = new Tokenizer(buffer);
            tokenizer.ScanTokens(this);
            return tokenizer;
        }

        public Item? this[string variable]
        {
            get => GetVariable(variable);
            set => SetVariable(variable, value);
        }
    }
}
