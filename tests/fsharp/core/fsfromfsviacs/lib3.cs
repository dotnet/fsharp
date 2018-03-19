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


namespace StructTests
{
    // Check This should be preferred over the same type in lib2.cs
    public struct SomeMutableStruct
    {
        public int x;
        public System.DateTime y;
        public void SetX(int v) { x = v;  }
        public void SetY(System.DateTime v) { y = v; }
        public int SetZ() { return 1; } // no warning
        public void set_X(int v) { x = v; }
    }
}
