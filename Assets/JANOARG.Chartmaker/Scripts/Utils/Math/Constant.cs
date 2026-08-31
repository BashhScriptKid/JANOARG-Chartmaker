

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JANOARG.Chartmaker.Utils.Math
{
    internal class Constant
    {
        public string Name;
        public double Value;

        private Constant() { }

        public override string ToString()
        {
            return Name;
        }

        public static readonly Dictionary<string, Constant> Constants = new() {
            {
                "pi", new Constant
                {
                    Name = "pi",
                    Value = System.Math.PI
                }
            },
            {
                "e", new Constant
                {
                    Name = "e",
                    Value = System.Math.E
                }
            },
        };
    }
}