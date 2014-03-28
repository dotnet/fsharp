//
// Test C# source file that defines interesting EMs
// Compile with: csc /t:library ...

public struct S
{
}

public class C
{
    public void M() { }
}

public class CDerived : C
{
}


public static class ExtMethodsCSharp
{
    // EM on a struct
    public static decimal M1(this S s, decimal d1, float f1)
    {
        return d1;
    }

    // EM on a class
    public static decimal M2(this C c, decimal d1, float f1)
    {
        return -d1;
    }

    // EM on a struct - return void
    public static void M3(this S s, decimal d1, float f1)
    {
    }

    // EM on a class - return void
    public static void M4(this C c, decimal d1, float f1)
    {
    }

    // EM on a struct - with params
    public static decimal[] M3(this S s, params decimal [] p)
    {
        return p;
    }

    // EM on a class - with params
    public static C M4(this C c, params decimal [] p)
    {
        return c;
    }

}
