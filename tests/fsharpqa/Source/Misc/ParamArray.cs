using System;

namespace CSharpAssembly
{
    [AttributeUsage(AttributeTargets.All)]
    public class AttributeWithParamArray : Attribute
    {
        public object[] Parameters;

        public AttributeWithParamArray(params object[] x)
        {

            Parameters = x;
        }
    }

    public class CSParamArray
    {
        public static int Method(params int[] allArgs)
        {
            int total = 0;
            foreach (int i in allArgs)
                total += i;

            return total;
        }

        public static int Method<T>(params T[] args)
        {
            return args.Length;
        }
    }
}