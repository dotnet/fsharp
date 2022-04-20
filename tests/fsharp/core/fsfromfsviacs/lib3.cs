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
        public static long MethodTakingOptionalsInt64(long x = 3, string y = "abc", double d = 5.0)
        {
            return x + y.Length + (int) d;
        }
        public static int MethodTakingNullableOptionalsWithDefaults(int? x = 3, string y = "abc", double? d = 5.0)
        {
            return (x.HasValue ? x.Value : -100) + y.Length + (int) (d.HasValue ? d.Value : 0.0);
        }
        public static long MethodTakingNullableOptionalsWithDefaultsInt64(long? x = 3, string y = "abc", double? d = 5.0)
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
        public static long MethodTakingNullableOptionalsInt64(long? x = null, string y = null, double? d = null)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }
        public static int OverloadedMethodTakingOptionals(int x = 3, string y = "abc", double d = 5.0)
        {
            return x + y.Length + (int) d;
        }
        public static int OverloadedMethodTakingOptionals(int x = 3, string y = "abc", System.Single d = 5.0f)
        {
            return x + y.Length + (int) d + 7;
        }
        public static int OverloadedMethodTakingNullableOptionalsWithDefaults(int? x = 3, string y = "abc", double? d = 5.0)
        {
            return (x.HasValue ? x.Value : -100) + y.Length + (int) (d.HasValue ? d.Value : 0.0);
        }
        public static int OverloadedMethodTakingNullableOptionalsWithDefaults(long? x = 3, string y = "abc", double? d = 5.0)
        {
            return (x.HasValue ? (int) x.Value : -100) + y.Length + (int) (d.HasValue ? d.Value : 0.0) + 7;
        }
        public static int OverloadedMethodTakingNullableOptionals(int? x = null, string y = null, double? d = null)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }
        public static int OverloadedMethodTakingNullableOptionals(long? x = null, string y = null, double? d = null)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? (int) x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0) + 7;
        }
        public static int MethodTakingNullables(int? x, string y, double? d)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }

        public static long MethodTakingNullablesInt64(long? x, string y, double? d)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }
        public static int OverloadedMethodTakingNullables(int? x, string y, double? d)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0);
        }
        public static int OverloadedMethodTakingNullables(long? x, string y, double? d)
        {
            int length;
            if (y == null)
                length = -1;
            else
                length = y.Length;
            return (x.HasValue ? (int) x.Value : -1) + length + (int) (d.HasValue ? d.Value : -1.0) + 7;
        }
        public static int SimpleOverload(int? x = 3)
        {
            return (x.HasValue ? x.Value : 100);
        }

        public static int SimpleOverload(int x = 3)
        {
            return (x + 200);
        }
    }

}
