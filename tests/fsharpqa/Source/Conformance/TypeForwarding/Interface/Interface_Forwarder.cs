#region non_generic
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

public class ITurnsToClass
{
    public int getValue()
    {
        return 1;
    }
}

#endregion

#region basic generic interface
public interface Basic001_GI<T>
{
    int getValue();
}



public interface Basic002_GI<U>
{
    int getValue();
}



#endregion




#region method
public interface Method_NotInForwarder<U>
{
    int getValue();
    
}

public interface Method_Non_Generic
{
    int getValue<T>();
}

#endregion