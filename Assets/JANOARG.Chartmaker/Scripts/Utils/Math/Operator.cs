

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JANOARG.Chartmaker.Utils.Math
{
    internal class Operator
    {
        public string Name;
        public int LeftBindingPower;
        public Associativity Associativity;
        public Func<double, double> PrefixFunction;
        public Func<double, double, double> InfixFunction;

        private Operator() { }

        public override string ToString()
        {
            return Name;
        }

        public static readonly Dictionary<string, Operator> Operators = new() {
            {
                "+", new Operator
                {
                    Name = "+",
                    LeftBindingPower = (int)BindingPower.Addictive,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => x,
                    InfixFunction = (x, y) => x + y,
                }
            },
            {
                "-", new Operator
                {
                    Name = "-",
                    LeftBindingPower = (int)BindingPower.Addictive,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => -x,
                    InfixFunction = (x, y) => x - y,
                }
            },
            {
                "*", new Operator
                {
                    Name = "*",
                    LeftBindingPower = (int)BindingPower.Multiplicative,
                    Associativity = Associativity.Left,
                    InfixFunction = (x, y) => x * y,
                }
            },
            {
                "/", new Operator
                {
                    Name = "/",
                    LeftBindingPower = (int)BindingPower.Multiplicative,
                    Associativity = Associativity.Left,
                    InfixFunction = (x, y) => x / y,
                }
            },
            {
                "%", new Operator
                {
                    Name = "%",
                    LeftBindingPower = (int)BindingPower.Multiplicative,
                    Associativity = Associativity.Left,
                    InfixFunction = (x, y) => x % y,
                }
            },
            {
                "mod", new Operator
                {
                    Name = "mod",
                    LeftBindingPower = (int)BindingPower.Multiplicative,
                    Associativity = Associativity.Left,
                    InfixFunction = (x, y) => (x % y + y) % y,
                }
            },
            {
                "^", new Operator
                {
                    Name = "^",
                    LeftBindingPower = (int)BindingPower.Exponential,
                    Associativity = Associativity.Right,
                    InfixFunction = (x, y) => System.Math.Pow(x, y),
                }
            },
            {
                "~", new Operator
                {
                    Name = "~",
                    LeftBindingPower = (int)BindingPower.BitwiseOr,
                    Associativity = Associativity.Right,
                    PrefixFunction = (x) => ~Convert.ToInt32(x),
                    InfixFunction = (x, y) => Convert.ToInt32(x) ^ Convert.ToInt32(y),
                }
            },
            {
                "|", new Operator
                {
                    Name = "|",
                    LeftBindingPower = (int)BindingPower.BitwiseOr,
                    Associativity = Associativity.Right,
                    InfixFunction = (x, y) => Convert.ToInt32(x) | Convert.ToInt32(y),
                }
            },
            {
                "&", new Operator
                {
                    Name = "&",
                    LeftBindingPower = (int)BindingPower.BitwiseAnd,
                    Associativity = Associativity.Right,
                    InfixFunction = (x, y) => Convert.ToInt32(x) & Convert.ToInt32(y),
                }
            },
            // These are technically functions but can be implemented as prefix-only operators
            {
                "abs", new Operator
                {
                    Name = "abs",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Abs(x),
                }
            },
            {
                "rnd", new Operator
                {
                    Name = "rnd",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Round(x),
                }
            },
            {
                "flr", new Operator
                {
                    Name = "flr",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Floor(x),
                }
            },
            {
                "ceil", new Operator
                {
                    Name = "ceil",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Ceiling(x),
                }
            },
            {
                "sin", new Operator
                {
                    Name = "sin",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Sin(x * Mathf.Deg2Rad),
                }
            },
            {
                "cos", new Operator
                {
                    Name = "cos",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Cos(x * Mathf.Deg2Rad),
                }
            },
            {
                "tan", new Operator
                {
                    Name = "tan",
                    LeftBindingPower = (int)BindingPower.Prefix,
                    Associativity = Associativity.Left,
                    PrefixFunction = (x) => System.Math.Tan(x * Mathf.Deg2Rad),
                }
            },
        };
    }

    internal enum BindingPower : int
    {
        BitwiseOr,
        BitwiseAnd,
        Addictive,
        Multiplicative,
        Exponential,
        Prefix,
    }

    internal enum Associativity
    {
        Left = 0,
        Right = -1,
    }
}