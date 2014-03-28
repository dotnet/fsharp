using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestLib
{
    public abstract class A : IComparable<A>
    {
        public A() { }
        public abstract int CompareTo(A other);
    }

    public abstract class B<T> : IComparable<B<T>>
    {
        public B() { }
        public abstract int CompareTo(B<T> other);
    }
}
