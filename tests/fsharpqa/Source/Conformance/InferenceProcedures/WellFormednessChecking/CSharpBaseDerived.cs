// Regression test for FSHARP1.0:6123

public abstract class Base
{
    public virtual int Foo { get { return 12; } }
}

public abstract class Derived : Base
{
    public abstract new int Foo { get; }
}
