using System;

namespace DependentAssembly
{
    public class DependentAssemblyClass
    {
        public string GetTheString(string passedIn)
        {
            return $"Hello {passedIn}";
        }
    }
}
