#if (FORWARD)

// non generic
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(NormalStruct))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N_003.Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(TurnsToClass))]

// basic generic
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_Normal<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffNum<,>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffName<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffName004<,>))]

// constraint
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_OnlyOrigin<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_OnlyForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_Both<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_BothViolated<>))]

// generic struct and generic struct test
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_NotInForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_Non_Generic))]



# else

#region non-generic
public struct NormalStruct
{
    public int getValue()
    {
        return 0;
    }
}



namespace N_003
{
    public struct Foo
    {
        public int getValue()
        {
            return 1;
        }

        public int getValue2()
        {
            return -1;
        }
    }
}


public struct TurnsToClass
{
    public int getValue()
    {
        return 0;
    }
}
#endregion

#region basic
public struct Basic_Normal<T>
{
    public int getValue()
    {
        return 0;
    }
}
public struct Basic_DiffNum<T>
{
    public int getValue()
    {
        return 0;
    }
}
public struct Basic_DiffName<T>
{
    public int getValue()
    {
        return 0;
    }
}

#endregion

#region constraint
public struct Constraint_OnlyOrigin<T> where T : struct
{
    public int getValue()
    {
        return 0;
    }
}

public struct Constraint_OnlyForwarder<T>
{
    public int getValue()
    {
        return 0;
    }
}
public struct Constraint_Both<T> where T : struct
{
    public int getValue()
    {
        return 0;
    }
}

public struct Constraint_BothViolated<T> where T : class
{
    public int getValue()
    {
        return 0;
    }

}

#endregion

#region method
public struct Method_NotInForwarder<T>
{
    public int getValue()
    {
        return 0;
    }
}

public struct Method_Non_Generic
{
    public int getValue<T>()
    {
        return 0;
    }
}

#endregion
#endif

#region non generic
namespace N_002
{
    public struct MethodParameter
    {
        public void Method(NormalStruct f) { }
    }
}

namespace N_003
{
    public class Bar
    {
        public int getValue()
        {
            Foo f = new Foo();
            return f.getValue2();
        }
    }
}

#endregion

#region  BASIC004


#if (BASIC004A)
public struct Basic_DiffName004<T, U>
{
    public int getValue()
    {
        return 0;
    }
}


#endif

#if (BASIC004B)
public struct Basic_DiffName004<A, B>
{
    public int getValue()
    {
        return 0;
    }
}

#endif

#if (BASIC004C)
public struct Basic_DiffName004<T, B>
{
    public int getValue()
    {
        return 0;
    }
}
#endif

#if (BASIC004D)
public struct Basic_DiffName004<A, U>
{
    public int getValue()
    {
        return 0;
    }
}

#endif

#if (BASIC004E)
public struct Basic_DiffName004<U, T>
{
    public int getValue()
    {
        return 0;
    }
}
#endif

#endregion