#region non_generic
public delegate int DeleNormalDelegate();
namespace N_003
{
    public delegate int DFoo();
}
public class DeleTurnsToClass { }

#endregion

#region basic generic
public delegate int Basic001_GDele<T>(T t);
public delegate int Basic002_GDele<T, U>(T t);
public delegate int Basic003_GDele<U>(U u);

#endregion