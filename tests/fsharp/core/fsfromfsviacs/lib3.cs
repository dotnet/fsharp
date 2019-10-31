using Microsoft.FSharp;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

namespace Newtonsoft.Json.Converters
{
    // This should be preferred over the same type in lib2.cs
    public class SomeClass
    {
        public SomeClass() { }
        public static void SomeMethod() { }
    }

    public class ContainerClass
    {
        public ContainerClass() { } 

        // This should be preferred over the same type in lib2.cs
        public class SomeClass
        {
            public SomeClass() { }
            public static void SomeMethod() { }
        }

    }
}
namespace CSharpOptionalParameters
{
    // This should be preferred over the same type in lib2.cs
    public class SomeClass
    {
        public SomeClass() { }
        public static int MethodTakingOptionals(int x = 3, string y = "abc", double d = 5.0)
        {
            return x + y.Length + (int) d;
        }
        public static int MethodTakingNullableOptionalsWithDefaults(int? x = 3, string y = "abc", double? d = 5.0)
        {
            return (x.HasValue ? x.Value : -100) + y.Length + (int) (d.HasValue ? d.Value : 0.0);
        }
        public static int MethodTakingNullableOptionals(int? x = null, string y = null, double? d = null)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }
    }

}
