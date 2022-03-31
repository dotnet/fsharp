using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadWriteLib
{
    public class MyClass { public readonly int ReadonlyFoo; }

    public struct MyStruct
    {
        public readonly int ReadonlyFoo;
        public int WriteableFoo;
    }
}