using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MyClass<S>
{
}
public class MyClass
{
    public static T M<T>(object x)
    {
        return (T)x;
    }
    public static int M2()
    {
        return 12;
    }
}
