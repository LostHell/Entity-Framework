using System;
using System.Collections.Generic;
using System.Text;

namespace Prototype
{
    public class SandwichMenu
    {
        private readonly Dictionary<string, SandwichPrototype> _sandwiches =
            new Dictionary<string, SandwichPrototype>();

        public SandwichPrototype this[string name]
        {
            get { return _sandwiches[name]; }
            set { _sandwiches.Add(name, value); }
        }
    }
}
