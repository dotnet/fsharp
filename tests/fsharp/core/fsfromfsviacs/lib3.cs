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
