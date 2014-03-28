//
// Test C# class library that exposes some types with "internal".
// Compile with: csc /t:library ...

// Make internals visible to friend F# assembly
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("InternalsConsumer")]

// public class with internal member
public class Greetings
{
    public string SayHelloTo(string name)
    {
        return "Hello, " + name + "!";
    }

    internal string SayHiTo(string name)
    {
        return "Hi, " + name + "!";
    }
}

// internal class with internal member
internal class Calc
{
    public int Add(int x, int y)
    {
        return x + y;
    }

    internal int Mult(int x, int y)
    {
        return x * y;
    }
}

// private class with internal member
class CalcBeta
{
    private int Diff(int x, int y)
    {
        return x - y;
    }

    public int Div(int x, int y)
    {
        return x / y;
    }

    internal int Mod(int x, int y)
    {
        return x % y;
    }
}