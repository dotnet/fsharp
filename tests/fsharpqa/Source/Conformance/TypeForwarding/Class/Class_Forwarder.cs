#region Non_Generic
public class NormalClass
{
    public int getValue()
    {
        return -1;
    }
}

namespace N_002
{
    public class MethodParameter
    {
        public void Method(NormalClass f) { }
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
            return -2;
        }
    }
}


public struct TurnsToStruct
{   
    public int getValue()
    {
        return -1;
    }
}
#endregion

#region Basic
public class Basic_Normal<T>
{
    public int getValue()
    {
        return -1;
    }
}

public class Basic_DiffNum<T, U>
{
    public int getValue()
    {
        return -1;
    }
}

public class Basic_DiffName<U>
{
    public int getValue()
    {
        return -1;
    }
}

public class Basic_DiffName004<T, U>
{
    public int getValue()
    {
        return -1;
    }
}
#endregion

#region Constraint
public class Constraint_OnlyOrigin<T>
{
    public int getValue()
    {
        return -1;
    }
}

public class Constraint_OnlyForwarder<T> where T : struct
{
    public int getValue()
    {
        return -1;
    }
}

public class Constraint_NonViolatedForwarder<T> where T : class
{
    public int getValue()
    {
        return -1;
    }
}

public class Constraint_Both<T> where T : class
{
    public int getValue()
    {
        return -1;
    }
}

public class Constraint_BothNonViolated<T> where T : class
{
    public int getValue()
    {
        return -1;
    }
}
public class Constraint_BothViolated<T> where T : struct
{
    public int getValue()
    {
        return -1;
    }
}


#endregion

#region Method
public class Method_NotInForwarder<T>
{
    public int notgetValue()
    {
        return -1;
    }
}

public class Method_Non_Generic
{
    public int getValue<T>()
    {
        return -1;
    }
}

#endregion

#region Interface
public interface Interface_Base<T>
{
}

public interface TurnToInterface_Base<T>
{
    int getValue();
}

#endregion