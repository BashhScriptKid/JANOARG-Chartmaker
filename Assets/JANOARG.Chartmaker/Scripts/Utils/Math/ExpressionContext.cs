

using System.Collections.Generic;

namespace JANOARG.Chartmaker.Utils.Math
{
    public class ExpressionContext
    {
        public Dictionary<string, double> Variables { get; private set; } = new();

        public ExpressionContext SetVariable(string name, double value)
        {
            Variables[name] = value;
            return this;
        }
        public bool HasVariable(string name)
        {
            return Variables.ContainsKey(name);
        }
        public double GetVariable(string name)
        {
            return Variables[name];
        }
    }
}