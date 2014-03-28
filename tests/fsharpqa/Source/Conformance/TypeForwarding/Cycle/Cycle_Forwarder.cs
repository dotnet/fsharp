#if (FORWARDFOO)
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))]
#endif
#if (FORWARDBAR)
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Bar))]
#endif
#if (BASIC001A)
public class Bar
{
    public int getValue()
    {
        return -2;
    }
}
#endif

#if (BASIC001B)
public class Foo
{
    public int getValue()
    {
        return 1;
    }
}
#endif

#if (BASIC002A)
 public class Foo
{
    public int getValue()
    {
        return 1;
    }
}
public class Bar
{
    public int getValue()
    {
        return -2;
    }
}
#endif

#if (BASIC002B)
 public class Foo
{
    public int getValue()
    {
        return 1;
    }
}
#endif

#if (BASIC003A)
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))] 
#endif

#if (BASIC003B)
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))] 
#endif

#if (BASIC004A)
public class Foo
{
    public int getValue()
    {
        System.Console.WriteLine("A.elseFoo");
        return -2;
    }
}
public class Bar
{
    public int getValue()
    {
        System.Console.WriteLine("A.Bar");
        return -2;
    }
} 
#endif

#if (BASIC004B)
public class Bar
{
    public int getValue()
    {
        System.Console.WriteLine("B.elseBar");
        return -1;
    }
}
public class Foo
{
    public int getValue()
    {
        System.Console.WriteLine("B.Foo");
        return -1;
    }
} 
#endif