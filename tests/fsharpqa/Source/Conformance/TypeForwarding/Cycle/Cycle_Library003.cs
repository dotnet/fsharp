#if (FORWARD)
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))]



public class Baz
{
    public int getValue()
    {
        return 0;
    }
}
#endif

public class Foo
{
    public int getValue()
    {
        return 0;
    }
}


public class Baz
{
    public int getValue()
    {
        return 0;
    }
}

