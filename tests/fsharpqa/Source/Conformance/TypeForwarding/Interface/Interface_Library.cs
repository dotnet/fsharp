#if (FORWARD)
// non-generic
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(INormal))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N_003.IFoo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(ITurnsToClass))]

// basic generic interface test
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic001_GI<>))]

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic002_GI<>))]




// method
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_NotInForwarder<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Method_Non_Generic))]

# else

#region non-generic
public interface INormal
{
    int getValue();
}

namespace N_003
{
    public interface IFoo
    {
        int getValue();

        int getValue2();
    }
}

public interface ITurnsToClass
{
    int getValue();
}

#endregion

#region basic generic
public interface Basic001_GI<T>
{
    int getValue();
}

public interface Basic002_GI<T>
{
    int getValue();
}



#endregion



#region method
public interface Method_NotInForwarder<T>
{
    int getValue();
}
public interface Method_Non_Generic
{
    int getValue<T>();
}

#endregion

#endif

#region non-generic
public class NormalInterface : INormal
{
    public int getValue()
    {
        return -1;
    }

    int INormal.getValue()
    {
        return 1;
    }
}

namespace N_002
{
    public class MethodParameter
    {
        public int Method(INormal i)
        {
            return i.getValue();
        }
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


namespace N_003
{
    public class Foo : N_003.IFoo
    {
        public int getValue()
        {
            return 1;
        }

        public int getValue2()
        {
            return -2;
        }
    }
}



public class TurnsToClass : ITurnsToClass
{
    public int getValue()
    {
        return 1;
    }
}

#endregion

#region basic generic
public class Basic001_Class<T> : Basic001_GI<T>
{
    public int getValue()
    {
        return 1;
    }

    int Basic001_GI<T>.getValue()
    {
        return -1;
    }

}


public class Basic002_Class<T> : Basic002_GI<T>
{
    public int getValue()
    {
        return 1;
    }

    int Basic002_GI<T>.getValue()
    {
        return -1;
    }

}


#endregion






#region method
public class GenericClass<T> : Method_NotInForwarder<T>
{
    public int getValue()
    {
        return 1;
    }

    int Method_NotInForwarder<T>.getValue()
    {
        System.Console.WriteLine("HIHIHIHI");
        return -1;
    }

}

public class NonGenericClass : Method_Non_Generic
{
    public int getValue<T>()
    {
        return 1;
    }

    int Method_Non_Generic.getValue<T>()
    {
        return -1;
    }

}

#endregion