#if (FORWARD)
// basic cycle forwarding
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Bar))]

# else

public class Foo
{
    public int getValue()
    {
        return 0;
    }
}

public class Bar
{
    public int getValue()
    {
        return 0;
    }
}

#endif

public class Baz
{
    public int getValue()
    {
        return 0;
    }
}