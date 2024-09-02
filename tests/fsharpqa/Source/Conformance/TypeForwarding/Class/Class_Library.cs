#if (FORWARD)
// non-generic test
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(NormalClass))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N_003.Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(TurnsToStruct))]

// basic generic type forwarding
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_Normal<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffNum<,>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffName<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic_DiffName004<,>))]

// constraint generic type forwarding
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_OnlyOrigin<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_OnlyForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_NonViolatedForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_Both<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_BothNonViolated<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Constraint_BothViolated<>))]

// generic class and generic method test
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_NotInForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_Non_Generic))]

// generic interface test
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Interface_Base<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(TurnToInterface_Base<>))]


# else

#region Non_Generic
public class NormalClass
{
    public int getValue()
    {
        return 0;
    }
}


namespace N_003
{
    public class Foo
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


public class TurnsToStruct
{   
    public int getValue()
    {
        return 0;
    }
}

#endregion


#region Basic
public class Basic_Normal<T>
{
    public int getValue()
    {
        return 0;
    }
}

public class Basic_DiffNum<T>
{
    public int getValue()
    {
        return 0;
    }
}

public class Basic_DiffName<T>
{
    public int getValue()
    {
        return 0;
    }
}
#endregion

#region Constraint
public class Constraint_OnlyOrigin<T> where T : class
{
    public int getValue()
    {
        return 0;
    }
}

public class Constraint_OnlyForwarder<T>
{
    public int getValue()
    {
        return 0;
    }
}

public class Constraint_NonViolatedForwarder<T>
{
    public int getValue()
    {
        return 0;
    }
}

public class Constraint_Both<T> where T : class
{
    public int getValue()
    {
        return 0;
    }
}

public class Constraint_BothNonViolated<T> where T : new()
{
    public int getValue()
    {
        return 0;
    }

}
public class Constraint_BothViolated<T> where T : class
{
    public int getValue()
    {
        return 0;
    }

}

#endregion

#region Method
public class Method_NotInForwarder<T>
{
    public int getValue()
    {
        return 0;
    }
}

public class Method_Non_Generic
{
    public int getValue<T>()
    {
        return 0;
    }
}

#endregion

#region Interface
public class Interface_Base<T>
{

}

public class TurnToInterface_Base<T>
{
    public int getValue()
    {
        return 0;
    }
}

#endregion

#endif

#region non-generic free code
namespace N_002
{
    public class MethodParameter
    {
        public void Method(NormalClass f) { }
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
public class Basic_DiffName004<T, U>
{
    public int getValue()
    {
        return 0;
    }
}


#endif

#if (BASIC004B)
public class Basic_DiffName004<A, B>
{
    public int getValue()
    {
        return 0;
    }
}

#endif

#if (BASIC004C)
public class Basic_DiffName004<T, B>
{
    public int getValue()
    {
        return 0;
    }
}
#endif

#if (BASIC004D)
public class Basic_DiffName004<A, U>
{
    public int getValue()
    {
        return 0;
    }
}

#endif

#if (BASIC004E)
public class Basic_DiffName004<U, T>
{
    public int getValue()
    {
        return 0;
    }
}
#endif

#endregion

public class Interface_Sub<T> : Interface_Base<T>
{
    public int getValue()
    {
#if (FORWARD)
        return -1;
#else
		return 0;
#endif		
    }    
}

public class TurnToInterface_Sub<T> : TurnToInterface_Base<T>
{
    public int getValue()
    {
        return 0;
    }
}