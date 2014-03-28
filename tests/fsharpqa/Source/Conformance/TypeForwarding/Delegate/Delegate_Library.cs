#if (FORWARD)

// non-generic
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(DeleNormalDelegate))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N_003.DFoo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(DeleTurnsToClass))]

// generic
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic001_GDele<>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic002_GDele<,>))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Basic003_GDele<>))]


# else

#region non-generic
public delegate int DeleNormalDelegate();
namespace N_003
{
    internal delegate int DFoo();
}
public delegate int DeleTurnsToClass();

#endregion

#region basic generic
public delegate int Basic001_GDele<T>(T t);
public delegate int Basic002_GDele<T>(T t);
public delegate int Basic003_GDele<T>(T t);

#endregion
#endif

#region non_generic
public struct NormalDelegate
{
    public int getValue()
    {
        return 0;
    }
}

namespace N_002
{
    public struct MethodParameter
    {
        public int Method(DeleNormalDelegate dele)
        {
            return dele();
        }
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

    public class Bar
    {
        public int getValue()
        {
            Foo f = new Foo();
            return f.getValue2();
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

#region basic generic
public class Basic001_Class
{
    public int getValue<T>(T t)
    {
        return 0;
    }
}

public class Basic002_Class
{
    public int getValue<T>(T t)
    {
        return 0;
    }
}

public class Basic003_Class
{
    public int getValue<T>(T t)
    {
        return 0;
    }
}

#endregion
