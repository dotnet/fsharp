#region non_generic
public struct NormalStruct
{
    public int getValue()
    {
        return -1;
    }
}

namespace N_002
{
    public struct MethodParameter
    {
        public void Method(NormalStruct f) { }
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
            return -2;
        }
    }
}


public class TurnsToClass
{
    public int getValue()
    {
        return -1;
    }
}

#endregion


#region basic generic interface
public struct Basic_Normal<T>
{
    public int getValue()
    {
        return -1;
    }
}

public struct Basic_DiffNum<T, U>
{
    public int getValue()
    {
        return -1;
    }
}

public struct Basic_DiffName<U>
{
    public int getValue()
    {
        return -1;
    }
}

public struct Basic_DiffName004<T, U>
{
    public int getValue()
    {
        return -1;
    }
}



#endregion

#region constraint
public struct Constraint_OnlyOrigin<T>
{
    public int getValue()
    {
        return -1;
    }
}

public struct Constraint_OnlyForwarder<T> where T : struct
{
    public int getValue()
    {
        return -1;
    }
}



public struct Constraint_Both<T> where T : struct
{
    public int getValue()
    {
        return -1;
    }
}



public struct Constraint_BothViolated<T> where T : struct
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