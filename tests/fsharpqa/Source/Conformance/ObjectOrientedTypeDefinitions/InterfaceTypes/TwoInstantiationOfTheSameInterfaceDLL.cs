// Regression test for FSHARP1.0:5465

// A generic interface
interface I<T>
{
    void K(T x);    // Some generic method
}

// A class implementing 2 instantiation of the generic interface I<T>
public class C : I<int>, I<string>
{
    public void K(int x) { }
    public void K(string x) { }
}
