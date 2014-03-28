//Interface - empty
public interface I_000<T> { }


//Interface with inherits-decl 
public interface I_001 : I_000<char>, I_000<string> { }


//Interface with type-defn-members 
public interface I_002<T>
{
    int Me(T t);
}

//Interface with inherits-decl & type-defn-members 
public interface I_003<T> : I_002<string>, I_002<char>
{
    T Home(T t);
}


public class T : I_003<int>
{
    static void Main()
    { }

    public int Home(int i) { return 0; }
    public int Me(char c) { return 1; }
    public int Me(string s) { return 2; }


}