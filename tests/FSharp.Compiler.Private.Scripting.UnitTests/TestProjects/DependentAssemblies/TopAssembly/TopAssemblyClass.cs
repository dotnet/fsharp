using System;
using DependentAssembly;

namespace TopAssembly
{
    public class TopAssemblyClass
    {
        public string GetTheString(string s)
        {
            return new DependentAssemblyClass().GetTheString(s);
        }
    }
}
