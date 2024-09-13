// Verify consumption from F#

//Interface - empty
public interface I_000<T> { }

//Interface with inherits-decl 
public interface I_001<T> : I_000<T> { }

//Interface with type-defn-members 
public interface I_002<T>
{
    T Me();
}

//Interface with inherits-decl & type-defn-members 
public interface I_003<T> : I_001<T>
{
    T Home(T t);
}
